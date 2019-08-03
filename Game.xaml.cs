using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VNet.Assets;
using Boolean = System.Boolean;
using Label = System.Windows.Controls.Label;

namespace VNet
{
	/// <summary>
	/// Interaction logic for Game.xaml
	/// </summary>
	public partial class Game : Window
	{
		private Assets.Assets _assets;
		private Script _currentScript;
		private LexicalAnalysis _lexical;
		private GameEnvironment _environment;


		// Game area elements
		private TextBlock _textBlock;
		private TextBlock _nameBlock;

		private Image _blackBackgroundConstant;
		private Image _backgroundImage;

		private Image _leftCharacter;
		private Image _rightCharacter;
		private Image _centerCharacter;

		private DoubleAnimation _fadeIn;
		private DoubleAnimation _fadeOut;
		private DispatcherTimer _textTimer;

		private MediaPlayer _backgroundMusicPlayer;
		private MediaPlayer _soundEffectPlayer;

		public Game()
		{
			InitializeComponent();
		}

		/*
		 * Loaded event on window, runs startup procedure
		 */
		private void Game_OnLoaded(object sender, RoutedEventArgs e)
		{
			Startup();
		}

		private void Startup()
		{
			// Create global objects needed at this point
			_assets = new Assets.Assets();
			_environment = new GameEnvironment();
			_backgroundMusicPlayer = new MediaPlayer();
			_soundEffectPlayer = new MediaPlayer();

			// Create default black background
			WriteableBitmap black = BitmapFactory.New(1280, 720);
			black.Clear(Colors.Black);
			Background default_black = new Background("default_black", black);
			_assets.backgrounds.Add(default_black);

			_blackBackgroundConstant = new Image
			{
				Name = "blackBackground",
				Source = black,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_blackBackgroundConstant);
			Panel.SetZIndex(_blackBackgroundConstant, 0);

			// Create background element which will display chosen backgrounds
			_backgroundImage = new Image
			{
				Name = "backgroundImage",
				Source = null,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_backgroundImage);
			Panel.SetZIndex(_backgroundImage, 1);

			Settings.inGame = false;

			// Show splash screen while loading assets
			DoubleAnimation splashScreenFadeIn = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(1000))
			};
			splashScreenFadeIn.Completed += SplashScreenFadeInCompleted;

			_backgroundImage.Opacity = 0;
			bool success = _assets.CreateBackground("splash_screen", ".\\assets\\splash.png", false);
			if (success)
			{
				_backgroundImage.Source = _assets.backgrounds.Find(i => i.name == "splash_screen").image;
			}
			_backgroundImage.BeginAnimation(OpacityProperty, splashScreenFadeIn);
		}

		private void SplashScreenFadeInCompleted(object sender, EventArgs e)
		{
			DoubleAnimation splashScreenFadeOut = new DoubleAnimation
			{
				From = 1,
				To = 0,
				Duration = new Duration(TimeSpan.FromMilliseconds(1000))
			};
			splashScreenFadeOut.Completed += SplashScreenFadeOutCompleted;

			_currentScript = new Script(Settings.StartScriptUri);
			_lexical = new LexicalAnalysis(_currentScript);
			_currentScript.currentLine = ProcessScript();

			_backgroundImage.BeginAnimation(OpacityProperty, splashScreenFadeOut);
		}
		private void SplashScreenFadeOutCompleted(object sender, EventArgs e)
		{
			MainMenu();
		}

		/*
		 * Loads main menu elements
		 */
		private void MainMenu()
		{
			ClearViewport(true);

			Button newGameButton = new Button
			{
				Name = "newGameButton",
				Width = 320,
				Height = 48,
				Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
			};
			TextBlock newGameTextBlock = new TextBlock
			{
				Name = "newGameTextBlock",
				Text = "New Game",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			newGameButton.Content = newGameTextBlock;
			newGameButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(NewGameButtonClick));
			ViewportContainer.Children.Add(newGameButton);
			Canvas.SetLeft(newGameButton, 100);
			Canvas.SetTop(newGameButton, 200);
			Panel.SetZIndex(newGameButton, 2);

			Button loadGameButton = new Button
			{
				Name = "newGameButton",
				Width = 320,
				Height = 48,
				Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
			};
			TextBlock loadGameTextBlock = new TextBlock
			{
				Name = "loadGameTextBlock",
				Text = "Load Game",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			loadGameButton.Content = loadGameTextBlock;
			loadGameButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(LoadGameButtonClick));
			ViewportContainer.Children.Add(loadGameButton);
			Canvas.SetLeft(loadGameButton, 100);
			Canvas.SetTop(loadGameButton, 300);
			Panel.SetZIndex(loadGameButton, 2);

			Button optionsButton = new Button
			{
				Name = "optionsButton",
				Width = 320,
				Height = 48,
				Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
			};
			TextBlock optionsTextBlock = new TextBlock
			{
				Name = "optionsTextBlock",
				Text = "Options",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			optionsButton.Content = optionsTextBlock;
			optionsButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(OptionsButtonClick));
			ViewportContainer.Children.Add(optionsButton);
			Canvas.SetLeft(optionsButton, 100);
			Canvas.SetTop(optionsButton, 400);
			Panel.SetZIndex(optionsButton, 2);

			// Menu background image and music
			ShowBackground("menu_background", 10);
			PlaySound("menu_music", 1, true);
		}

		private void NewGameButtonClick(object sender, RoutedEventArgs e)
		{
			ClearViewport(false);
			StopSound();
			StopMusic();
			NewGame();
		}
		
		private void LoadGameButtonClick(object sender, RoutedEventArgs e)
		{
			ShowLoadGameScreen();
		}
		
		private void OptionsButtonClick(object sender, RoutedEventArgs e)
		{
			ShowOptionsScreen();
		}

		/*
		 * Clears the screen of all elements, if so specified keep background
		 */
		private void ClearViewport(bool keepBackground)
		{
			int allowedElementsCount = 0;
			while (ViewportContainer.Children.Count > allowedElementsCount)
			{
				// If child is not an image remove it
				if (!(ViewportContainer.Children[allowedElementsCount] is Image backgroundImg))
				{
					ViewportContainer.Children.RemoveAt(allowedElementsCount);
				}
				// If child is an image, check the name and remove if not background.
				// If it is background and keepBackground is false, set source to null (keep UI element as backgrounds are constant)
				else
				{
					if ((backgroundImg.Name == "blackBackground") || ( keepBackground && (backgroundImg.Name == "backgroundImage") ))
					{
						allowedElementsCount++;
					}
					else if (!keepBackground && backgroundImg.Name == "backgroundImage")
					{
						backgroundImg.Source = null;
						allowedElementsCount++;
					}
					else
					{
						ViewportContainer.Children.RemoveAt(allowedElementsCount);
					}
				}
			}
			this.UpdateLayout();
		}

		/*
		 * Create all onscreen elements for the game and enable gameplay
		 */
		private void NewGame()
		{
			_leftCharacter = new Image
			{
				Name = "leftCharacter",
				Opacity = 0.0,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_leftCharacter);
			Panel.SetZIndex(_leftCharacter, 2);

			_centerCharacter = new Image
			{
				Name = "centerCharacter",
				Opacity = 0.0,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_centerCharacter);
			Panel.SetZIndex(_centerCharacter, 2);

			_rightCharacter = new Image
			{
				Name = "rightCharacter",
				Opacity = 0.0,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_rightCharacter);
			Panel.SetZIndex(_rightCharacter, 2);

			_nameBlock = new TextBlock
			{
				Name = "nameBlock",
				FontSize = 24,
				FontWeight = FontWeights.ExtraBold,
			};
			ViewportContainer.Children.Add(_nameBlock);
			Canvas.SetLeft(_nameBlock, 50);
			Canvas.SetTop(_nameBlock, 560);
			Panel.SetZIndex(_nameBlock, 3);

			_textBlock = new TextBlock
			{
				Name = "textBlock",
				Width = 1180.0,
				Height = 120.0,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.AliceBlue),
				Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
				TextWrapping = TextWrapping.Wrap
			};
			ViewportContainer.Children.Add(_textBlock);
			Canvas.SetLeft(_textBlock, 50);
			Canvas.SetTop(_textBlock, 600);
			Panel.SetZIndex(_textBlock, 3);

			_textTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(Settings.TextDisplaySpeedInMiliseconds)
			};
			_textTimer.Tick += TextTimerOnTick;

			Settings.inGame = true;
			Settings.allowProgress = true;
			TriggerNextCommand();
		}

		/*
		 * Show screen containing all saved games found allow user to load a game
		 */
		private void ShowLoadGameScreen()
		{
			ClearViewport(true);

		}

		/*
		 * Show screen containing various options like text speed, music/sound volume...
		 */
		private void ShowOptionsScreen()
		{
			ClearViewport(true);

		}

		/*
		 * Function processes script from start to finish, executing setup commands (creating characters, backgrounds, labels..).
		 * At the end it sets current position in script to the first non-setup line
		 */
		private int ProcessScript()
		{
			int startOfGameLine = 0;
			while(true)
			{
				string[] command = ProcessScriptLine();
				if (command == null) return startOfGameLine;
				if (Settings.SetupKeywordList.Contains(command[0]))
				{
					ExecuteCommand(command);
				}
				else if (startOfGameLine == 0)
				{
					startOfGameLine = _currentScript.currentLine - 1;
				}
			}
		}

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
		 */
		private void ExecuteCommand(string[] command)
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

				case "jump":
					JumpToLabel(command[1]);
					break;

				case "character":
					if (command[3] != null)
						_assets.CreateCharacter(command[1], command[2], command[3]);
					else if (command[2] != null)
						_assets.CreateCharacter(command[1], command[2]);
					else
						_assets.CreateCharacter(command[1]);
					break;

				case "color":
					_assets.SetCharacterColor(command[1], command[2], command[3], command[4]);
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

				case "sound":
					_assets.CreateSound(command[1], command[2], false);
					break;

				case "music":
					_assets.CreateSound(command[1], command[2], true);
					break;

				case "int":
					if (int.TryParse(command[2], out intValue))
					{
						_assets.CreateVariable(command[1], intValue);
					}
					break;

				case "bool":
					if (bool.TryParse(command[2], out boolValue))
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
							if (bool.TryParse(command[3], out boolValue))
							{
								if (boolValue == boolVal.value)
								{
									JumpToLabel(command[4]);
								}
							}
							break;
						case Assets.Integer intVal:
							if (int.TryParse(command[3], out intValue))
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
					if (int.TryParse(command[2], out intValue))
					{
						_assets.IntegerAdd(command[1], intValue);
					}
					break;

				case "subtract":
					if (int.TryParse(command[2], out intValue))
					{
						_assets.IntegerSubtract(command[1], intValue);
					}
					break;

				case "set":
					if (int.TryParse(command[2], out intValue))
					{
						_assets.IntegerSet(command[1], intValue);
					}
					else if (bool.TryParse(command[2], out boolValue))
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
					PlaySound(command[1], double.TryParse(command[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var volume) ? volume : 1.0, command.Contains("r") || command.Contains("repeat"));
					break;

				case "stop":
					if (command.Contains("pause"))
					{
						Settings.executeNext = false;
					}
					switch (command[1])
					{
						case "sound":
							_soundEffectPlayer.Stop();
							break;
						case "music":
							_backgroundMusicPlayer.Stop();
							break;
						default:
							_soundEffectPlayer.Stop();
							_backgroundMusicPlayer.Stop();
							break;
					}
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

				default:
					Settings.executeNext = false;
					ExecuteTextCommand(command);
					break;
			}
		}

		/*
		 * Function executes a single line in the current script if it is a text line
		 */
		private void ExecuteTextCommand(string[] command)
		{
			// For protagonist
			if (command[0] == "PC")
			{
				if (command[2] == "thought")
				{
					ShowText("", command[1], true);
				}
				else
				{
					ShowText("", command[1]);
				}
				return;
			}

			// For other characters
			Character selectedCharacter = _assets.characters.Find(i => i.name == command[0]);
			ShowText(selectedCharacter != null ? selectedCharacter.name : command[0], command[1]);
		}

		/*
		 * Function changes the currently showing background to the one with the given name
		 */
		private void ShowBackground(string bgName, int fadeInDuration)
		{
			_fadeIn = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(fadeInDuration)),
			};
			_fadeIn.Completed += (sender, args) => { Settings.allowProgress = true; };

			Background selectedBackground = _assets.backgrounds.Find(i => i.name == bgName);
			if (selectedBackground != null)
			{
				Settings.allowProgress = false;
				_environment.currentBackgroundName = selectedBackground.name;
				_backgroundImage.Source = selectedBackground.image;
				_backgroundImage.BeginAnimation(OpacityProperty, _fadeIn);
			}
		}

		/*
		 * Clears the background. This shows the default black background element, but the original background UI element remains as it is constant.
		 */
		private void ClearBackground()
		{
			_environment.currentBackgroundName = null;
			_backgroundImage.Source = null;
		}

		/*
		 * Function shows a given character with the given sprite (mood) in the given position
		 */
		private void ShowCharacter(string chName, string moodName, int fadeInDuration, string position = "")
		{
			_fadeIn = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(fadeInDuration)),
			};
			_fadeIn.Completed += (sender, args) => { Settings.allowProgress = true; };

			Character selectedCharacter = _assets.characters.Find(i => i.name == chName);
			Mood selectedMood = selectedCharacter?.moods.Find(i => i.name == moodName);
			if (selectedMood != null)
			{
				Settings.allowProgress = false;
				double xOffset;
				double yOffset;
				if (position == "left")
				{
					_environment.leftCharacterName = selectedCharacter.name;
					xOffset = 0;
					yOffset = 0;
					_leftCharacter.Source = selectedMood.image;
					_leftCharacter.BeginAnimation(OpacityProperty, _fadeIn);
					Canvas.SetLeft(_leftCharacter, xOffset);
					Canvas.SetTop(_leftCharacter, yOffset);
				}
				else if (position == "right")
				{
					_environment.rightCharacterName = selectedCharacter.name;
					xOffset = 760;
					yOffset = 0;
					_rightCharacter.Source = selectedMood.image;
					_rightCharacter.BeginAnimation(OpacityProperty, _fadeIn);
					Canvas.SetLeft(_rightCharacter, xOffset);
					Canvas.SetTop(_rightCharacter, yOffset);
				}
				else
				{
					_environment.centerCharacterName = selectedCharacter.name;
					xOffset = 380;
					yOffset = 0;
					_centerCharacter.Source = selectedMood.image;
					_centerCharacter.BeginAnimation(OpacityProperty, _fadeIn);
					Canvas.SetLeft(_centerCharacter, xOffset);
					Canvas.SetTop(_centerCharacter, yOffset);
				}
			}
		}

		/*
		 * Function clears the image of character specified in argument; if no character is specified clears all characters
		 */
		private void ClearCharacters(string position = "")
		{
			if (position == "left" && _environment.leftCharacterName != null)
			{
				_leftCharacter.Source = null;
				_environment.leftCharacterName = null;

			}
			else if (position == "center" && _environment.centerCharacterName != null)
			{
				_centerCharacter.Source = null;
				_environment.centerCharacterName = null;
			}
			else if (position == "right" && _environment.rightCharacterName != null)
			{
				_rightCharacter.Source = null;
				_environment.rightCharacterName = null;
			}
			else
			{
				if (_environment.leftCharacterName != null)
				{
					_leftCharacter.Source = null;
					_environment.leftCharacterName = null;
				}
				if (_environment.centerCharacterName != null)
				{
					_centerCharacter.Source = null;
					_environment.centerCharacterName = null;
				}
				if (_environment.rightCharacterName != null)
				{
					_rightCharacter.Source = null;
					_environment.rightCharacterName = null;
				}
			}
		}

		/*
		 * Function saves current line text contents and starts a timer to display the text character by character
		 */
		private void ShowText(string characterName, string content, bool thought = false)
		{
			Character selChar = _assets.characters.Find(i => i.name == characterName);
			if (selChar != null)
			{
				_nameBlock.Foreground = new SolidColorBrush(selChar.color);
			}
			else
			{
				_nameBlock.Foreground = new LinearGradientBrush(
					Color.FromRgb(32, 32, 32),
					Color.FromRgb(64, 64, 64),
					45.0);
			}
			
			_nameBlock.Text = thought ? "" : characterName;
			if (thought)
				_environment.fullText = content;
			else
				_environment.fullText = "\"" + content + "\"";

			_environment.displayedText = "";
			Settings.textDisplayedFully = false;
			_textTimer.Start();
		}

		/*
		 * Function is called on each tick of the text timer, adds one character to the text
		 */
		private void TextTimerOnTick(object sender, EventArgs e)
		{
			if (_environment.displayedText.Length >= _environment.fullText.Length)
			{
				Settings.textDisplayedFully = true;
				_textTimer.Stop();
				return;
			}
			_environment.displayedText += _environment.fullText[_environment.DisplayedTextLength];
			_textBlock.Text = _environment.displayedText;
		}

		/*
		 * Shows entire line of text at once, interrupting the "typing out" animation.
		 */
		private void FinishShowingText()
		{
			_environment.displayedText = _environment.fullText;
			_textBlock.Text = _environment.displayedText;
			_textTimer.Stop();
		}

		/*
		 * Checks if requested sound is an effect or song and plays in in appropriate sound player. If sound and song have the same name the sound takes precedence.
		 */
		private void PlaySound(string soundName, double volume, bool repeat = false)
		{
			var snd = _assets.sounds.Find(i => i.name == soundName);
			if (snd == null)
			{
				snd = _assets.music.Find(i => i.name == soundName);
				if (snd == null) return;
				_environment.currentSongName = snd.name;
				_backgroundMusicPlayer.Open(new Uri(snd.location));
				if (repeat)
				{
					_backgroundMusicPlayer.MediaEnded -= EndSong;
					_backgroundMusicPlayer.MediaEnded += RepeatPlayback;
				}
				else
				{
					_backgroundMusicPlayer.MediaEnded -= RepeatPlayback;
					_backgroundMusicPlayer.MediaEnded += EndSong;
				}
				_backgroundMusicPlayer.Volume = volume * Settings.MusicVolumeMultiplier;
				_backgroundMusicPlayer.Play();
				return;
			}
			_soundEffectPlayer.Open(new Uri(snd.location));
			if (repeat)
			{
				_soundEffectPlayer.MediaEnded += RepeatPlayback;
			}
			else
			{
				_soundEffectPlayer.MediaEnded -= RepeatPlayback;
			}
			_soundEffectPlayer.Volume = volume * Settings.SoundVolumeMultiplier;
			_soundEffectPlayer.Play();
		}
		private void RepeatPlayback(object sender, EventArgs e)
		{
			var player = (MediaPlayer) sender;
			player.Position = TimeSpan.Zero;
			player.Play();
		}
		private void EndSong(object sender, EventArgs e)
		{
			_environment.currentSongName = null;
		}

		/*
		 * Stops current sound playback
		 */
		private void StopSound()
		{
			_soundEffectPlayer.Stop();
		}

		/*
		 * Stops current music playback
		 */
		private void StopMusic()
		{
			_backgroundMusicPlayer.Stop();
			_environment.currentSongName = null;
		}

		/*
		 * Pauses the gameplay and shows multiple buttons corresponding to choice options
		 */
		private void ShowChoice(string choiceName)
		{
			Settings.allowProgress = false;
			int textTop = 100;
			Choice ch = _assets.choices.Find(i => i.name == choiceName);
			_textBlock.Text = ch.text;

			foreach (Option opt in ch.options)
			{
				textTop += 50;
				Button button = new Button
				{
					Name = opt.destinationLabel,
					Width = 960,
					Height = 48,
					Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
				};
				TextBlock optionTextBlock = new TextBlock
				{
					Name = "optionTextBlock",
					Text = opt.text,
					FontSize = 20,
					FontWeight = FontWeights.Bold,
					Foreground = new SolidColorBrush(Colors.White)
				};
				button.Content = optionTextBlock;
				button.AddHandler(Button.ClickEvent, new RoutedEventHandler(ChoiceButtonClick));
				ViewportContainer.Children.Add(button);
				Canvas.SetLeft(button, Settings.windowWidth / 2 - button.Width / 2);
				Canvas.SetTop(button, textTop);
				Panel.SetZIndex(button, 4);
				_environment.onscreenButtonNames.Add(button.Name);
			}
		}

		/*
		 * Event handler for clicking a button in choice, clears the choice, resumes gameplay and jumps to specified label
		 */
		private void ChoiceButtonClick(object sender, RoutedEventArgs e)
		{
			JumpToLabel(((Button)sender).Name);
			
			while (_environment.onscreenButtonNames.Count > 0)
			{
				Button btn = (Button)LogicalTreeHelper.FindLogicalNode(ViewportContainer, _environment.onscreenButtonNames[0]);
				ViewportContainer.Children.Remove(btn);
				_environment.onscreenButtonNames.RemoveAt(0);
			}

			Settings.allowProgress = true;
			TriggerNextCommand();
		}

		/*
		 * Function sets the current line in script to location of given label
		 */
		private void JumpToLabel(string labelName)
		{
			Settings.executeNext = true;
			_currentScript.currentLine = _assets.labels.Find(i => i.name == labelName).lineNumber;
		}

		/*
		 * Executes next line of script. Can execute multiple lines if they are non-stopping
		 */
		private void TriggerNextCommand()
		{
			Settings.executeNext = true;
			while (Settings.executeNext)
			{
				string[] command = ProcessScriptLine();
				if (command == null) return;
				if (Settings.SetupKeywordList.Contains(command[0]) || command[0] == null) continue;
				
				ExecuteCommand(command);
			}
		}

		/*
		 * Click event on window, triggers next line when ingame
		 */
		private void Game_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// Disable if not in game or game currently not allowing progress (in choice, in animation...)
			if (!Settings.inGame || !Settings.allowProgress)
			{
				return;
			}
			// If the textBlock is still being filled
			if (Settings.textDisplayedFully == false)
			{
				FinishShowingText();
				Settings.textDisplayedFully = true;
				return;
			}
			// If text is displayed completely
			TriggerNextCommand();
		}
	}
}