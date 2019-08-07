using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VNet.Assets;
using Boolean = System.Boolean;

namespace VNet
{
	public partial class Game : Window
	{
		/*
		 * Function processes next line in current script and returns array of commands and arguments from that line
		 */
		private string[] ProcessScriptLine()
		{
			Token token = _lexical.GetNextToken();
			if (token.Type == Type.Eof) return null;
			if (token.Type == Type.LexError)
			{
				// TODO implement lex error recovery
			}

			bool insideQuotes = false;
			string quotedString = "";

			string[] lineComponents = new string[7];

			while (true)
			{
				try
				{
					switch (token.Type)
					{
						// Sets the "command" for this line
						case Type.Keyword:
							if (lineComponents[0] != null)
							{
								if (insideQuotes)
								{
									quotedString += token.Lexem;
								}
								else if (lineComponents[0] == null)
									lineComponents[0] = token.Lexem;
								else if (lineComponents[1] == null)
									lineComponents[1] = token.Lexem;
								else if (lineComponents[2] == null)
									lineComponents[2] = token.Lexem;
								else if (lineComponents[3] == null)
									lineComponents[3] = token.Lexem;
								else if (lineComponents[4] == null)
									lineComponents[4] = token.Lexem;
								else if (lineComponents[5] == null)
									lineComponents[5] = token.Lexem;
								else if (lineComponents[6] == null)
									lineComponents[6] = token.Lexem;
								break;
							}
							lineComponents[0] = token.Lexem;
							if (token.Lexem == "label")
								lineComponents[1] = (token.Location.Line + 1).ToString();
							break;
						// If word is in quotes adds to string in quotes, otherwise puts the word in first empty spot of command
						case Type.Word:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							else if (lineComponents[0] == null)
								lineComponents[0] = token.Lexem;
							else if (lineComponents[1] == null)
								lineComponents[1] = token.Lexem;
							else if (lineComponents[2] == null)
								lineComponents[2] = token.Lexem;
							else if (lineComponents[3] == null)
								lineComponents[3] = token.Lexem;
							else if (lineComponents[4] == null)
								lineComponents[4] = token.Lexem;
							else if (lineComponents[5] == null)
								lineComponents[5] = token.Lexem;
							else if (lineComponents[6] == null)
								lineComponents[6] = token.Lexem;
							break;

						// If number is in quotes adds to string in quotes, otherwise puts the number in first empty spot of command
						case Type.Number:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							else if (lineComponents[0] == null)
								lineComponents[0] = token.Lexem;
							else if (lineComponents[1] == null)
								lineComponents[1] = token.Lexem;
							else if (lineComponents[2] == null)
								lineComponents[2] = token.Lexem;
							else if (lineComponents[3] == null)
								lineComponents[3] = token.Lexem;
							else if (lineComponents[4] == null)
								lineComponents[4] = token.Lexem;
							else if (lineComponents[5] == null)
								lineComponents[5] = token.Lexem;
							else if (lineComponents[6] == null)
								lineComponents[6] = token.Lexem;
							break;

						// If it is an opening quote starts adding following elements to quoted string, otherwise closes quote and adds whole string to first available variable slot
						case Type.Quote:
							insideQuotes = !insideQuotes;
							if (!insideQuotes)
							{
								if (lineComponents[0] == null)
									lineComponents[0] = quotedString;
								else if (lineComponents[1] == null)
									lineComponents[1] = quotedString;
								else if (lineComponents[2] == null)
									lineComponents[2] = quotedString;
								else if (lineComponents[3] == null)
									lineComponents[3] = quotedString;
								else if (lineComponents[4] == null)
									lineComponents[4] = quotedString;
								else if (lineComponents[5] == null)
									lineComponents[5] = quotedString;
								else if (lineComponents[6] == null)
									lineComponents[6] = quotedString;
								quotedString = "";
							}
							break;

						// If punctuation is in quotes adds the character to string in quotes, otherwise puts it into first available spot of command
						case Type.Punctuation:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							else if (lineComponents[0] == null)
								lineComponents[0] = token.Lexem;
							else if (lineComponents[1] == null)
								lineComponents[1] = token.Lexem;
							else if (lineComponents[2] == null)
								lineComponents[2] = token.Lexem;
							else if (lineComponents[3] == null)
								lineComponents[3] = token.Lexem;
							else if (lineComponents[4] == null)
								lineComponents[4] = token.Lexem;
							else if (lineComponents[5] == null)
								lineComponents[5] = token.Lexem;
							else if (lineComponents[6] == null)
								lineComponents[6] = token.Lexem;
							break;

						case Type.Whitespace:
							if (insideQuotes)
							{
								quotedString += token.Lexem;
							}
							break;
					}
				}
				catch (Exception e)
				{
					MessageBox.Show("Error in script on line " + e.Message);
				}
				token = _lexical.GetNextToken();
				if (token.Type == Type.NewLine) break;
				if (token.Type == Type.Eof) break;
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
		private void ExecuteCommand(string[] command, bool afterSave)
		{
			int intValue;
			bool boolValue;
			// Runs function corresponding to the command with the variables given
			switch (command[0])
			{
				case "execute":

					break;

				case "label":
					_assets.CreateLabel(command[2], command[1]);
					break;

				case "character":
					if (command[3] != null)
						_assets.CreateCharacter(command[1], command[2], command[3]);
					else if (command[2] != null)
						_assets.CreateCharacter(command[1], command[2]);
					else
						_assets.CreateCharacter(command[1]);
					break;

				case "image":
					if (command[3] == null)
					{
						_assets.CreateBackground(command[1], command[2], true);
					}
					else
					{
						_assets.AddImageToCharacter(command[1], command[2], command[3]);
					}
					break;

				case "color":
					_assets.SetCharacterColor(command[1], command[2], command[3], command[4]);
					break;

				case "sound":
					_assets.CreateSound(command[1], command[2], false);
					break;

				case "music":
					_assets.CreateSound(command[1], command[2], true);
					break;

				case "choice":
					if (command[1] == "create")
					{
						_assets.CreateChoice(command[2]);
					}
					else
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

				case "name":
					Settings.gameName = command[1];
					break;

				case "jump":
					JumpToLabel(command[1]);
					break;

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

				case "show":
					if (command[1] == "choice")
					{
						Settings.executeNext = false;
						ShowChoice(command[2]);
					}
					if (command.Contains("pause")) Settings.executeNext = false;
					// Only parameter is name, therefore show as background
					if (command[2] == null)
					{
						ShowBackground(command[1], 500);
					}
					// More parameters, show as character
					else if (command[2] != null)
					{
						if (command[3] != null)
							ShowCharacter(command[1], command[2], 500, command[3]);
						else
							ShowCharacter(command[1], command[2], 500);
					}
					break;

				case "clear":
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}

					if (command[1] == "background")
					{
						ClearBackground();
					}
					else if (command[1] != null)
					{
						ClearCharacters(command[1]);
					}
					else
					{
						ClearCharacters();
					}
					break;

				case "play":
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}
					PlaySound(command[1], Double.TryParse(command[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var volume) ? volume : 1.0, command.Contains("r") || command.Contains("repeat"));
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
						default:
							StopSound(true);
							StopMusic(true);
							break;
					}
					break;

				case "save":
					SaveGame(0);
					break;

				default:
					if (!afterSave)
					{
						ExecuteTextCommand(command);
						Settings.executeNext = false;
					}
					break;
			}
		}
	}
}