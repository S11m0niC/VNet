using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VNet.Assets;

namespace VNet
{
	/// <summary>
	/// Interaction logic for Game.xaml
	/// </summary>
	public partial class Game : Window
	{
		private const string _ScriptExtension = "vnets";
		private const string _StartScriptURI = "./game." + _ScriptExtension;

		private Assets.Assets _assets = new Assets.Assets();
		private Script _currentScript;
		private WriteableBitmap viewport;

		public Game()
		{
			InitializeComponent();
			Startup();
		}

		private void Startup()
		{
			// Set rendering image
			viewport = BitmapFactory.New((int)ViewportContainer.ActualWidth, (int)ViewportContainer.ActualHeight);
			ViewportContainer.Source = viewport;

			// Execute starting script
			_currentScript = new Script(_StartScriptURI);
			ProcessScript(_currentScript);
			ExecuteScript(_currentScript, true);
		}

		private void ExecuteScript(Script script, bool startupMode)
		{
			LexicalAnalysis lexical = new LexicalAnalysis(script);

			// Loop for processing lines
			while (true)
			{
				Token token = lexical.GetNextToken();
				if (token.Type == Type.Eof) break;
				if (token.Type == Type.LexError)
				{
					// TODO implement lex error recovery
				}

				bool insideQuotes = false;
				string quotedString = "";

				string[] lineComponents = new string[5];

				while (true)
				{
					try
					{
						switch (token.Type)
						{
							// Sets the "command" for this line
							case Type.Keyword:
								if (lineComponents[0] != null)
									throw new Exception(token.Location.Line.ToString());
								lineComponents[0] = token.Lexem;
								break;

							// If word is in quotes adds to string in quotes, otherwise puts the word in first empty spot of command
							case Type.Word:
								if (insideQuotes)
								{
									quotedString += token.Lexem;
								}
								else if (lineComponents[1] == null)
									lineComponents[1] = token.Lexem;
								else if (lineComponents[2] == null)
									lineComponents[2] = token.Lexem;
								else if (lineComponents[3] == null)
									lineComponents[3] = token.Lexem;
								else if (lineComponents[4] == null)
									lineComponents[4] = token.Lexem;
								break;

							// If number is in quotes adds to string in quotes
							case Type.Number:
								if (insideQuotes)
								{
									quotedString += token.Lexem;
								}
								break;

							// If it is an opening quote starts adding following elements to quoted string, otherwise closes quote and adds whole string to first available variable slot
							case Type.Quote:
								insideQuotes = !insideQuotes;
								if (!insideQuotes)
								{
									if (lineComponents[2] == null)
										lineComponents[2] = quotedString;
									else if (lineComponents[3] == null)
										lineComponents[3] = quotedString;
									else if (lineComponents[4] == null)
										lineComponents[4] = quotedString;
									quotedString = "";
								}
								break;

							// If punctuation is in quotes adds the character to string in quotes
							case Type.Punctuation:
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
					token = lexical.GetNextToken();
					if (token.Type == Type.NewLine) break;
					if (token.Type == Type.Eof) break;
					if (token.Type == Type.LexError)
					{
						//TODO implement lex error recovery
					}
				}

				ExecuteScriptLine(lineComponents);
			}
		}

		/*
		 * Function executes a single line in the current script after it has already been processed into an array of arguments
		 */
		private void ExecuteScriptLine(string[] commands)
		{
			// Runs function corresponding to the command with the variables given
			switch (commands[0])
			{
				case "execute":
					break;
				case "label":
					break;
				case "jump":
					break;
				case "character":
					if (commands[3] != null)
						_assets.CreateCharacter(commands[1], commands[2], commands[3]);
					if (commands[2] != null)
						_assets.CreateCharacter(commands[1], commands[2]);
					else
						_assets.CreateCharacter(commands[1]);
					break;
				case "image":
					if (commands[3] == null)
					{
						_assets.CreateBackground(commands[1], commands[2]);
					}
					else
					{
						_assets.AddImageToCharacter(commands[1], commands[2], commands[3]);
					}
					break;
				case "show":
					// Only parameter is name, therefore show as background
					if (commands[2] == null)
					{
						ShowBackground(commands[1]);
					}
					// 
					else if (commands[2] != null)
					{
						if (commands[3] != null)
							ShowCharacter(commands[1], commands[2], commands[3]);
						else
							ShowCharacter(commands[1], commands[2]);
					}
					break;
				case "play":
					break;
				case "choice":
					break;
			}
		}

		private void ProcessScript(Script script)
		{

		}

		private void ShowBackground(string bgName)
		{
			Background selectedBackground = _assets.backgrounds.Find(i => i.name == bgName);
			if (selectedBackground != null)
			{
				ViewportContainer.Source = selectedBackground.image;
				_assets.SetBackgroundToShowing(selectedBackground.name);
			}
		}

		private void ShowCharacter(string chName, string moodName, string position = "")
		{
			Point spriteLocation;
			if (position == "left")
			{
				spriteLocation = new Point(320, 0);
			}
			else if (position == "right")
			{
				spriteLocation = new Point(640, 0);
			}
			else
			{
				spriteLocation = new Point(960, 0);
			}

			Character selectedCharacter = _assets.characters.Find(i => i.name == chName);
			Mood selectedMood = selectedCharacter?.moods.Find(i => i.name == moodName);
			if (selectedMood != null)
			{
				// TODO CREATE SPRITE IMAGE AND INSERT INTO WINDOW AT CORRECT POSITION
			}
		}

	}
}
