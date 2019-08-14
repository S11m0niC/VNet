using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VNet.Assets;
using Boolean = System.Boolean;

namespace VNet
{
	public partial class Game : Window
	{
		/*
		 * Function processes next line in current script and returns list of commands and arguments from that line
		 */
		public List<string> ProcessScriptLine()
		{
			_lexical.Source = scripts[currentScriptIndex];

			Token token = _lexical.GetNextToken();
			if (token.Type == Type.Eof) return null;
			if (token.Type == Type.LexError)
			{
				// TODO implement lex error recovery
			}

			bool insideQuotes = false;
			string quotedString = "";

			List<string> lineComponents = new List<string>();

			while (true)
			{
				try
				{
					switch (token.Type)
					{
						// Sets the "command" for this line
						case Type.Keyword:
							// If the command already has a beginning keyword, just treat it as a regular word
							if (lineComponents.Count > 0)
							{
								if (insideQuotes)
								{
									quotedString += token.Lexem;
								}
								else
								{
									lineComponents.Add(token.Lexem);
								}
								break;
							}
							lineComponents.Add(token.Lexem);
							if (token.Lexem == "label")
								lineComponents.Add((token.Location.Line + 1).ToString());
							break;
						
						// If word is in quotes adds to string in quotes, otherwise puts the word in first empty spot of command
						case Type.Word:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							else lineComponents.Add(token.Lexem);
							break;

						// If number is in quotes adds to string in quotes, otherwise puts the number in first empty spot of command
						case Type.Number:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							else lineComponents.Add(token.Lexem);
							break;

						// If it is an opening quote starts adding following elements to quoted string, otherwise closes quote and adds whole string to first available variable slot
						case Type.Quote:
							insideQuotes = !insideQuotes;
							if (!insideQuotes)
							{
								lineComponents.Add(quotedString);
								quotedString = "";
							}
							break;

						// If punctuation is in quotes adds the character to string in quotes, otherwise puts it into first available spot of command
						case Type.Punctuation:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							else lineComponents.Add(token.Lexem);
							break;

						case Type.Whitespace:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							break;
						case Type.Comment:
							lineComponents.Add("comment");
							break;
					}
				}
				catch (Exception e)
				{
					MessageBox.Show("Error in script on line " + e.Message);
				}
				token = _lexical.GetNextToken();
				if (token.Type == Type.NewLine) break;

				if (token.Type == Type.Eof)
				{
					if (lineComponents.Count > 0)
					{
						break;
					}
					return null;
				}
				if (token.Type == Type.LexError)
				{
					MessageBox.Show("Error in script on line " + token.Location.Line + ", column " +
									token.Location.Column);
				}
			}

			return lineComponents;
		}

		/*
		 * Function executes a single line in the current script after it has already been processed into an array of arguments
		 * Parameter afterSave controls whether the line is the first line after loading a saved game, in which case it does not execute text lines (as the text is contained in the save)
		 */
		private void ExecuteCommand(List<String> command, bool afterSave)
		{
			int intValue;
			bool boolValue;
			// Runs function corresponding to the command with the variables given
			switch (command[0])
			{
				// Setup
				case "label":
					if (command.Count == 3)
					{
						_assets.CreateLabel(command[2], currentScriptIndex.ToString(), command[1]);
					}
					break;

				case "image":
					switch (command.Count)
					{
						case 3:
							_assets.CreateBackground(command[1], command[2], true);
							break;
						case 4:
							_assets.AddImageToCharacter(command[1], command[2], command[3]);
							break;
					}
					break;

				case "character":
					if (command.Count == 4 && command.Contains("abbreviation"))
					{
						_assets.AddAbbreviationToCharacter(command[1], command[3]);
						break;
					}
					switch (command.Count)
					{
						case 4:
							_assets.CreateCharacter(command[1], command[2], command[3]);
							break;
						case 3:
							_assets.CreateCharacter(command[1], command[2]);
							break;
						case 2:
							_assets.CreateCharacter(command[1]);
							break;
					}
					break;

				case "color":
					if (command.Count == 5)
					{
						_assets.SetCharacterColor(command[1], command[2], command[3], command[4]);
					}
					break;

				case "sound":
					if (command.Count == 3)
					{
						_assets.CreateSound(command[1], command[2], false);
					}
					break;

				case "music":
					if (command.Count == 3)
					{
						_assets.CreateSound(command[1], command[2], true);
					}
					break;

				case "video":
					if (command.Count == 3)
					{
						_assets.CreateVideo(command[1], command[2]);
					}
					break;

				case "choice":
					if (command.Count == 3 && command[1] == "create")
					{
						_assets.CreateChoice(command[2]);
					}
					else if (command.Count == 5)
					{
						string choiceName = command[1];
						if (command[2] == "set" && command[3] == "text")
						{
							_assets.EditChoiceText(choiceName, command[4]);
						}
						else if (command[2] == "add")
						{
							_assets.AddOptionToChoice(choiceName, command[3], command[4]);
						}
					}
					break;
				
				// Script navigation
				case "jump":
					if (command.Count == 2)
					{
						JumpToLabel(command[1]);
					}
					break;

				case "include":
					if (command.Count == 2)
					{
						scripts.Add(new Script(command[1], scripts.Count));
						ProcessScript(scripts.Count - 1);
					}
					break;

				case "comment":
					break;

				// Graphics
				case "show":
					// Additional options
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}
					int fadeDuration = 0;
					if (command.Contains("fade"))
					{
						int commandIndex = command.FindIndex(s => s == "fade");
						int.TryParse(command[commandIndex + 1], out fadeDuration);
					}

					if (command.Count >= 2)
					{
						// Try to show background with given name
						if (ShowBackground(command[1], fadeDuration))
						{
							break;
						}
						// Try to show choice with given name
						if (ShowChoice(command[1]))
						{
							Settings.executeNext = false;
							break;
						}
					}

					// Try to show character with given name
					if (command.Count >= 4)
					{
						if (command.Contains("left"))
						{
							if (ShowCharacter(command[1], command[2], fadeDuration, "left"))
							{
								break;
							}

						}
						if (command.Contains("right"))
						{
							if (ShowCharacter(command[1], command[2], fadeDuration, "right"))
							{
								break;
							}

						}
						else
						{
							if (ShowCharacter(command[1], command[2], fadeDuration))
							{
								break;
							}

						}
					}
					
					break;

				case "clear":
					int unprocessedParts = command.Count - 1;
					if (command.Contains("pause"))
					{
						unprocessedParts--;
						Settings.executeNext = false;
					}
					fadeDuration = 0;
					if (command.Contains("fade"))
					{
						unprocessedParts--;
						int commandIndex = command.FindIndex(s => s == "fade");
						if (int.TryParse(command[commandIndex + 1], out fadeDuration))
						{
							unprocessedParts--;
						}
					}

					if (command.Contains("background"))
					{
						ClearBackground(fadeDuration);
						break;
					}

					if (unprocessedParts > 0)
					{
						ClearCharacters(fadeDuration, command[1]);
						break;
					}
					ClearCharacters(fadeDuration);
					break;

				case "ui":
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}
					if (command.Contains("show"))
					{
						ManipulateUI(true);
					}
					else if (command.Contains("hide"))
					{
						ManipulateUI(false);
					}
					break;

				// Sound and video
				case "play":
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}

					bool allowProgress = command.Contains("progress");
					bool hideUI = command.Contains("hide");
					bool repeat = command.Contains("repeat") || command.Contains("r");

					double volume = 1.0;
					if (command.Count >= 3)
					{
						double.TryParse(command[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out volume);
					}

					if (PlaySound(command[1], volume, repeat))
					{
						break;
					}

					PlayVideo(command[1], volume, allowProgress, hideUI);
					break;

				case "stop":
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}
					switch (command[1])
					{
						case "sound":
							StopSound(true);
							break;
						case "music":
							StopMusic(true);
							break;
						case "video":
							StopVideo();
							break;
						default:
							StopSound(true);
							StopMusic(true);
							StopVideo();
							break;
					}
					break;

				case "save":
					SaveGame(0);
					break;

				case "end":
					EndGame();
					break;

				// Variable manipulation
				case "int":
					if (Int32.TryParse(command[2], out intValue))
					{
						_assets.CreateVariable(command[1], intValue);
					}
					break;

				case "bool":
					if (Boolean.TryParse(command[2], out boolValue))
					{
						_assets.CreateVariable(command[1], boolValue);
					}
					break;

				case "if":
					Variable var = _assets.variables.Find(i => i.name == command[1]);
					if (var == null) break;
					switch (var)
					{
						case Assets.Boolean boolVal:
							if (Boolean.TryParse(command[3], out boolValue))
							{
								if (boolValue == boolVal.value)
								{
									JumpToLabel(command[4]);
								}
							}
							break;
						case Integer intVal:
							if (Int32.TryParse(command[3], out intValue))
							{
								switch (command[2])
								{
									case "<" when intVal.value < intValue:
									case ">" when intVal.value > intValue:
									case "=" when intVal.value == intValue:
										JumpToLabel(command[4]);
										break;
								}
							}
							break;
					}
					break;

				case "add":
					if (Int32.TryParse(command[2], out intValue))
					{
						_assets.IntegerAdd(command[1], intValue);
					}
					break;

				case "subtract":
					if (Int32.TryParse(command[2], out intValue))
					{
						_assets.IntegerSubtract(command[1], intValue);
					}
					break;

				case "set":
					if (Int32.TryParse(command[2], out intValue))
					{
						_assets.IntegerSet(command[1], intValue);
					}
					else if (Boolean.TryParse(command[2], out boolValue))
					{
						_assets.BooleanSet(command[1], boolValue);
					}
					break;

				// Settings
				case "name":
					if (command.Count == 3)
					{
						if (command[1] == "game")
						{
							Settings.gameName = command[2];
							this.Title = command[2];
						}
						else if (command[1] == "protagonist")
						{
							Settings.protagonistName = command[2];
						}
					}
					break;

				// Text
				default:
					if (!afterSave)
					{
						if (!ExecuteTextCommand(command))
						{
							MessageBox.Show("Error in displaying text on line " + scripts[currentScriptIndex].currentLine + "!");
						}
						Settings.executeNext = false;
					}
					break;
			}
		}

		/*
		 * Function executes a single line in the current script if it is a text line
		 */
		private bool ExecuteTextCommand(List<string> command)
		{
			if (command.Count < 2) return false;

			// For protagonist
			if (command[0] == "PC" || command[0] == Settings.protagonistName)
			{
				if (command.Contains("thought"))
				{
					ShowText(Settings.protagonistName, command[1], true);
				}
				else
				{
					ShowText(Settings.protagonistName, command[1]);
				}

				return true;
			}

			// For other characters
			Character selectedCharacter = _assets.characters.Find(i => i.name == command[0]) ?? _assets.characters.Find(i => i.abbreviation == command[0]);
			ShowText(selectedCharacter != null ? selectedCharacter.name : command[0], command[1]);
			
			return true;
		}
	}
}