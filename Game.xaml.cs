using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
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
		private List<Script> _scripts;
		public int currentScriptIndex;

		private LexicalAnalysis _lexical;
		private GameEnvironment _environment;

		// Game area elements
		private TextBlock _textBlock;
		private TextBlock _nameBlock;
		private Border _textBlockBorder;
		private Border _nameBlockBorder;
		private Border _buttonBorder;
		private Button _saveButton;
		private Button _menuButton;
		private Button _exitButton;

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
		private MediaElement _videoPlayer;

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

		/*
		 * Startup function, shows splash screen, loads assets and routes to main menu
		 */
		private void Startup()
		{
			// Create global objects needed at this point
			_scripts = new List<Script>();
			_assets = new Assets.Assets();
			_environment = new GameEnvironment();
			_backgroundMusicPlayer = new MediaPlayer();
			_soundEffectPlayer = new MediaPlayer();

			// Load default background
			try
			{
				bool bg_success = _assets.CreateBackground("default_back", ".\\assets\\default.png", false);
				if (!bg_success)
				{
					throw new Exception();
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("Error in loading default background image!\n" + e.Message);
			}

			_blackBackgroundConstant = new Image
			{
				Name = "blackBackground",
				Source = _assets.backgrounds.Find(i => i.name == "default_back").Image,
				Height = Settings.windowHeight,
				Width = Settings.windowWidth,
				Stretch = Stretch.UniformToFill
			};
			ViewportContainer.Children.Add(_blackBackgroundConstant);
			Panel.SetZIndex(_blackBackgroundConstant, 0);

			// Create background element which will display chosen backgrounds
			_backgroundImage = new Image
			{
				Name = "backgroundImage",
				Source = null,
				Height = Settings.windowHeight,
				Width = Settings.windowWidth,
				Stretch = Stretch.UniformToFill
			};
			ViewportContainer.Children.Add(_backgroundImage);
			Canvas.SetLeft(_backgroundImage, 0);
			Canvas.SetTop(_backgroundImage, 0);
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
			if (!success)
			{
				success = _assets.CreateBackground("splash_screen", ".\\assets\\splash.jpg", false);
			}
			if (!success)
			{
				success = _assets.CreateBackground("splash_screen", ".\\assets\\splash.jpeg", false);
			}
			if (!success)
			{
				success = _assets.CreateBackground("splash_screen", ".\\assets\\splash.tiff", false);
			}
			if (!success)
			{
				success = _assets.CreateBackground("splash_screen", ".\\assets\\splash.bmp", false);
			}
			if (!success)
			{
				success = _assets.CreateBackground("splash_screen", ".\\assets\\splash.gif", false);
			}
			if (success)
			{
				_backgroundImage.Source = new BitmapImage(_assets.backgrounds.Find(i => i.name == "splash_screen").imageUri);
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

			// Processing scripts
			 _scripts.Add(new Script(Settings.StartScriptUri));
			_lexical = new LexicalAnalysis(_scripts[0]);
			_scripts[0].currentLine = ProcessScript(0);

			// Load settings
			Settings.LoadSettings();

			// Load language
			_environment.currentLanguage = UILanguage.createLanguage(Settings.language);
			
			// Begin splash fade out animation
			_backgroundImage.BeginAnimation(OpacityProperty, splashScreenFadeOut);
		}
		private void SplashScreenFadeOutCompleted(object sender, EventArgs e)
		{
			Settings.gameLoaded = true;
			this.ResizeMode = ResizeMode.CanResize;
			MainMenu(true, true);
		}

		/*
		 * Clears the screen of all elements, if so specified keeps background
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
						backgroundImg.Effect = null;
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
			UpdateLayout();
		}

		/*
		 * Clears the screen of all elements except for specified ones
		 */
		private void ClearViewport(List<string> elementsToIgnore)
		{
			int allowedElementsCount = 0;
			while (ViewportContainer.Children.Count > allowedElementsCount)
			{
				if (!(ViewportContainer.Children[allowedElementsCount] is Image image))
				{
					if (!(ViewportContainer.Children[allowedElementsCount] is MediaElement element))
					{
						ViewportContainer.Children.RemoveAt(allowedElementsCount);
					}
					else
					{
						if (elementsToIgnore.Contains(element.Name))
						{
							allowedElementsCount++;
						}
						else
						{
							ViewportContainer.Children.RemoveAt(allowedElementsCount);
						}
					}
				}
				else
				{
					if (elementsToIgnore.Contains(image.Name))
					{
						allowedElementsCount++;
					}
					else
					{
						ViewportContainer.Children.RemoveAt(allowedElementsCount);
					}
				}
			}
			UpdateLayout();
		}

		/*
		 * Clears all UI elements off the screen with names contained in the temporaryUIElements list in the game environment.
		 * Used for elements which appear over normal gameplay elements (save dialog, etc.)
		 * Parameter layer: selects whether to clear UI elements in layer 1 or 2 of the UI
		 */
		private void ClearTemporaryUiElements(int layer)
		{
			if (layer == 1)
			{
				while (_environment.temporaryUIlayer1.Count > 0)
				{
					UIElement element = (UIElement)LogicalTreeHelper.FindLogicalNode(ViewportContainer, _environment.temporaryUIlayer1[0]);
					ViewportContainer.Children.Remove(element);
					_environment.temporaryUIlayer1.RemoveAt(0);
				}
				
			}
			else if (layer == 2)
			{
				while (_environment.temporaryUIlayer2.Count > 0)
				{
					UIElement element = (UIElement)LogicalTreeHelper.FindLogicalNode(ViewportContainer, _environment.temporaryUIlayer2[0]);
					ViewportContainer.Children.Remove(element);
					_environment.temporaryUIlayer2.RemoveAt(0);
				}
			}
		}

		/*
		 * Create all onscreen elements for the game and start gameplay
		 * parameter newGame: controls whether to start the game from new or just prepare environment for a loaded game
		 */
		private void NewGame(bool newGame)
		{
			_leftCharacter = new Image
			{
				Name = "leftCharacter",
				Opacity = 0.0,
				Height = Settings.windowHeight * 0.75,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_leftCharacter);
			Panel.SetZIndex(_leftCharacter, 2);

			_centerCharacter = new Image
			{
				Name = "centerCharacter",
				Opacity = 0.0,
				Height = Settings.windowHeight * 0.75,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_centerCharacter);
			Panel.SetZIndex(_centerCharacter, 2);

			_rightCharacter = new Image
			{
				Name = "rightCharacter",
				Opacity = 0.0,
				Height = Settings.windowHeight * 0.75,
				Stretch = Stretch.Uniform
			};
			ViewportContainer.Children.Add(_rightCharacter);
			Panel.SetZIndex(_rightCharacter, 2);

			_nameBlock = new TextBlock
			{
				Name = "nameBlock",
				FontSize = 24,
				FontWeight = FontWeights.ExtraBold,
				FontFamily = new FontFamily(Settings.fontNames),
				MinWidth = 180,
				Height = 40,
				Padding = new Thickness(30,5,5,5),
				Background = new SolidColorBrush(Color.FromArgb(148, 0, 0, 0))
			};
			_nameBlockBorder = new Border
			{
				BorderThickness = new Thickness(0, 3, 3, 0),
				BorderBrush = new SolidColorBrush(Colors.White),
				Child = _nameBlock,
			};
			ViewportContainer.Children.Add(_nameBlockBorder);
			Canvas.SetLeft(_nameBlockBorder, 0);
			Canvas.SetTop(_nameBlockBorder, Settings.windowHeight - 160);
			Panel.SetZIndex(_nameBlockBorder, 4);

			_textBlock = new TextBlock
			{
				Name = "textBlock",
				Width = Settings.windowWidth,
				Height = 120,
				Padding = new Thickness(30, 10, 30, 10),
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				FontFamily = new FontFamily(Settings.fontText),
				Foreground = new SolidColorBrush(Colors.AliceBlue),
				Background = new SolidColorBrush(Color.FromArgb(148, 0, 0, 0)),
				TextWrapping = TextWrapping.Wrap
			};
			_textBlockBorder = new Border
			{
				BorderThickness = new Thickness(0, 3, 3, 0),
				BorderBrush = new SolidColorBrush(Colors.White),
				Child = _textBlock,
			};
			ViewportContainer.Children.Add(_textBlockBorder);
			Canvas.SetLeft(_textBlockBorder, 0);
			Canvas.SetTop(_textBlockBorder, Settings.windowHeight - 120);
			Panel.SetZIndex(_textBlockBorder, 4);

			_textTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(Settings.textDisplaySpeedInMiliseconds)
			};
			_textTimer.Tick += TextTimerOnTick;

			// Save, menu and exit buttons
			StackPanel buttonPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};

			_saveButton = new Button
			{
				Name = "saveButton",
				Width = 30,
				Height = 30,
				Margin = new Thickness(10, 10, 10, 10),
				Background = new SolidColorBrush(Color.FromArgb(192, 192, 192, 192))
			};
			var saveIcon = new Image
			{
				Source = new BitmapImage(new Uri(_assets.ConvertToAbsolutePath(".\\assets\\icons\\saveIcon.png"), UriKind.Absolute))
			};
			_saveButton.Content = saveIcon;
			_saveButton.Click += SaveButtonOnClick;
			buttonPanel.Children.Add(_saveButton);

			_menuButton = new Button
			{
				Name = "menuButton",
				Width = 30,
				Height = 30,
				Margin = new Thickness(10, 10, 10, 10),
				Background = new SolidColorBrush(Color.FromArgb(192, 192, 192, 192))
			};
			Image menuIcon = new Image
			{
				Source = new BitmapImage(new Uri(_assets.ConvertToAbsolutePath(".\\assets\\icons\\menuIcon.png"), UriKind.Absolute))
			};
			_menuButton.Content = menuIcon;
			_menuButton.Click += MenuButtonOnClick;
			buttonPanel.Children.Add(_menuButton);

			_exitButton = new Button
			{
				Name = "exitButton",
				Width = 30,
				Height = 30,
				Margin = new Thickness(10, 10, 10, 10),
				Background = new SolidColorBrush(Color.FromArgb(192, 192, 192, 192))
			};
			Image exitIcon = new Image
			{
				Source = new BitmapImage(new Uri(_assets.ConvertToAbsolutePath(".\\assets\\icons\\exitIcon.png"), UriKind.Absolute))
			};
			_exitButton.Content = exitIcon;
			_exitButton.Click += ExitButtonOnClick;
			buttonPanel.Children.Add(_exitButton);

			_buttonBorder = new Border
			{
				BorderThickness = new Thickness(3, 3, 0, 0),
				BorderBrush = new SolidColorBrush(Colors.White),
				Background = new SolidColorBrush(Color.FromArgb(148, 0, 0, 0)),
				Child = buttonPanel
			};
			ViewportContainer.Children.Add(_buttonBorder);
			Canvas.SetLeft(_buttonBorder, Settings.windowWidth - 169);
			Canvas.SetTop(_buttonBorder, Settings.windowHeight - 173);
			Panel.SetZIndex(_buttonBorder, 4);

			Settings.UIvisible = true;
			if (newGame)
			{
				Settings.inGame = true;
				Settings.allowProgress = true;
				currentScriptIndex = 0;
				_scripts[0].currentLine = _scripts[0].firstGameplayLine;
				TriggerNextCommand();
			}
		}

		/*
		 * Function processes script from start to finish, executing setup commands (creating characters, backgrounds, labels..).
		 * At the end it sets current position in script to the first non-setup line
		 */
		private int ProcessScript(int scriptIndex)
		{
			Script oldScriptSource = _lexical.Source;
			int oldScriptIndex = oldScriptSource.index;
			currentScriptIndex = scriptIndex;
			while(true)
			{
				List<string> command = new List<string>();
				try
				{
					command = ProcessScriptLine();
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
				// End of file
				if (command == null)
				{
					_lexical.Source = oldScriptSource;
					currentScriptIndex = oldScriptIndex;
					return _scripts[scriptIndex].firstGameplayLine;
				}
				// Empty line
				if (command.Count == 0)
				{
					continue;
				}
				if (Settings.SetupKeywordList.Contains(command[0]) || Settings.SetupAndGameKeywordList.Contains(command[0]))
				{
					ExecuteCommand(command, false);
				}
				else if (_scripts[scriptIndex].firstGameplayLine == 0)
				{
					_scripts[scriptIndex].firstGameplayLine = _scripts[scriptIndex].currentLine - 1;
				}
			}
		}

		/*
		 * Function changes the currently showing background to the one with the given name
		 */
		private bool ShowBackground(string bgName, int fadeInDuration)
		{
			_fadeIn = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(fadeInDuration)),
			};

			Background selectedBackground = _assets.backgrounds.Find(i => i.name == bgName);
			if (selectedBackground == null) return false;

			_environment.currentBackgroundName = selectedBackground.name;
			if (selectedBackground.imageUri == null)
			{
				_backgroundImage.Source = selectedBackground.Image;
			}
			else
			{
				_backgroundImage.Source = new BitmapImage(selectedBackground.imageUri);
			}
			_backgroundImage.BeginAnimation(OpacityProperty, null);
			_backgroundImage.BeginAnimation(OpacityProperty, _fadeIn);
			return true;
		}

		/*
		 * Clears the background. This shows the default black background element, but the original background UI element remains as it is constant.
		 */
		private void ClearBackground(int fadeDuration)
		{
			_environment.currentBackgroundName = null;

			if (fadeDuration > 0)
			{
				_fadeOut = new DoubleAnimation
				{
					From = 1,
					To = 0,
					Duration = new Duration(TimeSpan.FromMilliseconds(fadeDuration)),
				};
				_fadeOut.Completed += (sender, args) =>
				{
					if (_environment.currentBackgroundName == null)
					{
						_backgroundImage.Source = null;
					}
				};

				_backgroundImage.BeginAnimation(OpacityProperty, null);
				_backgroundImage.BeginAnimation(OpacityProperty, _fadeOut);
			}
			else
			{
				_backgroundImage.Source = null;
			}
		}

		/*
		 * Function shows a given character with the given sprite (mood) in the given position
		 */
		private bool ShowCharacter(string chName, string moodName, int fadeInDuration, string position = "")
		{
			_fadeIn = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(fadeInDuration)),
			};

			Character selectedCharacter = _assets.characters.Find(i => i.name == chName);
			Mood selectedMood = selectedCharacter?.moods.Find(i => i.name == moodName);
			if (selectedMood == null) return false;
			
			if (position == "left")
			{
				_environment.leftCharacterName = selectedCharacter.name;
				_environment.leftCharacterMood = selectedMood.name;
				_leftCharacter.Source = new BitmapImage(selectedMood.imageUri);
				_leftCharacter.Opacity = 0;
				_leftCharacter.Height = selectedCharacter.heightCoefficient * Settings.windowHeight;
				_leftCharacter.BeginAnimation(OpacityProperty, _fadeIn);
				Canvas.SetLeft(_leftCharacter, selectedCharacter.horizontalOffset);
				Canvas.SetTop(_leftCharacter, Settings.windowHeight - _leftCharacter.Height + selectedCharacter.verticalOffset);
			}
			else if (position == "right")
			{
				_environment.rightCharacterName = selectedCharacter.name;
				_environment.rightCharacterMood = selectedMood.name;
				_rightCharacter.Opacity = 0;
				_rightCharacter.Height = selectedCharacter.heightCoefficient * Settings.windowHeight;
				_rightCharacter.Source = new BitmapImage(selectedMood.imageUri);
				_rightCharacter.BeginAnimation(OpacityProperty, _fadeIn);
				Canvas.SetLeft(_rightCharacter, Settings.windowWidth * 0.7 + selectedCharacter.horizontalOffset);
				Canvas.SetTop(_rightCharacter, Settings.windowHeight - _rightCharacter.Height + selectedCharacter.verticalOffset);
			}
			else
			{
				_environment.centerCharacterName = selectedCharacter.name;
				_environment.centerCharacterMood = selectedMood.name;
				_centerCharacter.Opacity = 0;
				_centerCharacter.Height = selectedCharacter.heightCoefficient * Settings.windowHeight;
				_centerCharacter.Source = new BitmapImage(selectedMood.imageUri);
				_centerCharacter.BeginAnimation(OpacityProperty, _fadeIn);
				Canvas.SetLeft(_centerCharacter, Settings.windowWidth * 0.4 + selectedCharacter.horizontalOffset);
				Canvas.SetTop(_centerCharacter, Settings.windowHeight - _centerCharacter.Height + selectedCharacter.verticalOffset);
			}

			UpdateLayout();
			return true;
		}

		/*
		 * Function clears the image of character specified in argument; if no character is specified clears all characters.
		 * If fade out is specified, starts the fade out animation.
		 */
		private void ClearCharacters(int fadeDuration, string nameOrPosition = "")
		{
			// Get the actual position if a character name is given
			string position = "";
			if (nameOrPosition != "left" && nameOrPosition != "right" && nameOrPosition != "center")
			{
				Character selectedCharacter = _assets.characters.Find(i => i.name == nameOrPosition);
				if (selectedCharacter != null)
				{
					string charName = selectedCharacter.name;
					if (_environment.leftCharacterName == charName)
					{
						position = "left";
					}
					else if (_environment.centerCharacterName == charName)
					{
						position = "center";
					}
					else if (_environment.rightCharacterName == charName)
					{
						position = "right";
					}
				}
			}
			else
			{
				position = nameOrPosition;
			}

			// Use animation
			if (fadeDuration > 0)
			{
				_fadeOut = new DoubleAnimation
				{
					Name = position,
					From = 1,
					To = 0,
					Duration = new Duration(TimeSpan.FromMilliseconds(fadeDuration)),
				};
				_fadeOut.Completed += (sender, args) =>
				{	
					if (_environment.leftCharacterName == null)
					{
						_leftCharacter.Source = null;
					}
					if (_environment.rightCharacterName == null)
					{
						_rightCharacter.Source = null;
					}
					if (_environment.centerCharacterName == null)
					{
						_centerCharacter.Source = null;
					}
				};

				if (position == "left")
				{
					if (_environment.leftCharacterName != null)
					{
						_leftCharacter.BeginAnimation(OpacityProperty, _fadeOut);
						_environment.leftCharacterName = null;
						_environment.leftCharacterMood = null;
					}
				}
				else if (position == "center")
				{
					if (_environment.centerCharacterName != null)
					{
						_centerCharacter.BeginAnimation(OpacityProperty, _fadeOut);
						_environment.centerCharacterName = null;
						_environment.centerCharacterMood = null;
					}
				}
				else if (position == "right")
				{
					if (_environment.rightCharacterName != null)
					{
						_rightCharacter.BeginAnimation(OpacityProperty, _fadeOut);
						_environment.rightCharacterName = null;
						_environment.rightCharacterMood = null;
					}
				}
				else
				{
					if (_environment.leftCharacterName != null)
					{
						_leftCharacter.BeginAnimation(OpacityProperty, _fadeOut);
						_environment.leftCharacterName = null;
						_environment.leftCharacterMood = null;
					}
					if (_environment.centerCharacterName != null)
					{
						_centerCharacter.BeginAnimation(OpacityProperty, _fadeOut);
						_environment.centerCharacterName = null;
						_environment.centerCharacterMood = null;
					}
					if (_environment.rightCharacterName != null)
					{
						_rightCharacter.BeginAnimation(OpacityProperty, _fadeOut);
						_environment.rightCharacterName = null;
						_environment.rightCharacterMood = null;
					}
				}
			}

			// Do not use animation
			else
			{
				if (position == "left")
				{
					if (_environment.leftCharacterName != null)
					{
						_leftCharacter.Source = null;
						_environment.leftCharacterName = null;
						_environment.leftCharacterMood = null;
					}
				}
				else if (position == "center" && _environment.centerCharacterName != null)
				{
					if (_environment.centerCharacterName != null)
					{
						_centerCharacter.Source = null;
						_environment.centerCharacterName = null;
						_environment.centerCharacterMood = null;
					}
				}
				else if (position == "right" && _environment.rightCharacterName != null)
				{
					if (_environment.rightCharacterName != null)
					{
						_rightCharacter.Source = null;
						_environment.rightCharacterName = null;
						_environment.rightCharacterMood = null;
					}
				}
				else
				{
					if (_environment.leftCharacterName != null)
					{
						_leftCharacter.Source = null;
						_environment.leftCharacterName = null;
						_environment.leftCharacterMood = null;
					}
					if (_environment.centerCharacterName != null)
					{
						_centerCharacter.Source = null;
						_environment.centerCharacterName = null;
						_environment.centerCharacterMood = null;
					}
					if (_environment.rightCharacterName != null)
					{
						_rightCharacter.Source = null;
						_environment.rightCharacterName = null;
						_environment.rightCharacterMood = null;
					}
				}
			}
		}

		/*
		 * Function saves current line text contents and starts a timer to display the text character by character
		 */
		private void ShowText(string characterName, string content, bool thought = false)
		{
			if (content.Length == 0)
			{
				return;
			}
			Character selChar = _assets.characters.Find(i => i.name == characterName);
			if (selChar != null)
			{
				if (Settings.colorCharacterNames)
				{
					_nameBlock.Foreground = new SolidColorBrush(selChar.color);
				}
				else
				{
					_nameBlock.Foreground = new SolidColorBrush(Colors.White);
				}

				if (Settings.colorTextBorders)
				{
					_nameBlockBorder.BorderBrush = new SolidColorBrush(selChar.color);
					_textBlockBorder.BorderBrush = new SolidColorBrush(selChar.color);
					_buttonBorder.BorderBrush = new SolidColorBrush(selChar.color);
				}
				else
				{
					_nameBlockBorder.BorderBrush = new SolidColorBrush(Colors.White);
					_textBlockBorder.BorderBrush = new SolidColorBrush(Colors.White);
					_buttonBorder.BorderBrush = new SolidColorBrush(Colors.White);
				}
			}
			else
			{
				var gradientBrush = new LinearGradientBrush(
					Color.FromRgb(32, 32, 32),
					Color.FromRgb(64, 64, 64),
					30.0);
				var solidBrush = new SolidColorBrush(Color.FromRgb(48, 48, 48));
				if (Settings.colorCharacterNames)
				{
					_nameBlock.Foreground = gradientBrush;
				}
				else
				{
					_nameBlock.Foreground = new SolidColorBrush(Colors.White);
				}

				if (Settings.colorTextBorders)
				{
					_nameBlockBorder.BorderBrush = solidBrush;
					_textBlockBorder.BorderBrush = solidBrush;
					_buttonBorder.BorderBrush = solidBrush;
				}
				else
				{
					_nameBlockBorder.BorderBrush = new SolidColorBrush(Colors.White);
					_textBlockBorder.BorderBrush = new SolidColorBrush(Colors.White);
					_buttonBorder.BorderBrush = new SolidColorBrush(Colors.White);
				}
			}
			
			_environment.nameOfCharacterTalking = thought ? "" : characterName;
			_nameBlock.Text = thought ? "" : characterName;
			if (thought)
				_environment.fullText = content;
			else if (content.Substring(0, 1) == "\"" && content.Substring(content.Length - 1, 1) == "\"")
			{
				_environment.fullText = content;
			}
			else
				_environment.fullText = "\"" + content + "\"";

			_environment.displayedText = "";
			Settings.textDisplayedFully = false;
			_textTimer.Start();
		}

		/*
		 * Sets the font for the entire application
		 */
		private void SetFont(string place, string fontName)
		{
			if (place == "ui")
			{
				this.FontFamily = new FontFamily(fontName);
			}

			if (place == "name")
			{
				Settings.fontNames = fontName;
			}

			if (place == "text")
			{
				Settings.fontText = fontName;
			}
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
		private bool PlaySound(string soundName, double volume, bool repeat = false)
		{
			var snd = _assets.sounds.Find(i => i.name == soundName);
			if (snd == null)
			{
				snd = _assets.music.Find(i => i.name == soundName);
				if (snd == null) return false;
				_environment.currentSongName = snd.name;
				_backgroundMusicPlayer.Open(new Uri(snd.location));
				if (repeat)
				{
					_backgroundMusicPlayer.MediaEnded -= EndOfSong;
					_backgroundMusicPlayer.MediaEnded += RepeatPlayback;
					_environment.currentSongRepeating = true;
				}
				else
				{
					_backgroundMusicPlayer.MediaEnded -= RepeatPlayback;
					_backgroundMusicPlayer.MediaEnded += EndOfSong;
					_environment.currentSongRepeating = false;
				}
				_backgroundMusicPlayer.Volume = volume * Settings.musicVolumeMultiplier;
				_environment.currentSongVolume = volume;
				_backgroundMusicPlayer.Play();
				return true;
			}
			_environment.currentSoundName = snd.name;
			_soundEffectPlayer.Open(new Uri(snd.location));
			if (repeat)
			{
				_soundEffectPlayer.MediaEnded -= EndOfSound;
				_soundEffectPlayer.MediaEnded += RepeatPlayback;
				_environment.currentSoundRepeating = true;
			}
			else
			{
				_soundEffectPlayer.MediaEnded += EndOfSound;
				_soundEffectPlayer.MediaEnded -= RepeatPlayback;
				_environment.currentSoundRepeating = false;
			}
			_soundEffectPlayer.Volume = volume * Settings.soundVolumeMultiplier;
			_environment.currentSoundVolume = volume;
			_soundEffectPlayer.Play();
			return true;
		}
		private void RepeatPlayback(object sender, EventArgs e)
		{
			var player = (MediaPlayer) sender;
			player.Position = TimeSpan.Zero;
			player.Play();
		}
		private void EndOfSong(object sender, EventArgs e)
		{
			_environment.currentSongName = null;
			_environment.currentSongRepeating = false;
		}
		private void EndOfSound(object sender, EventArgs e)
		{
			_environment.currentSoundName = null;
			_environment.currentSoundRepeating = false;
		}

		/*
		 * Stops current sound playback
		 */
		private void StopSound(bool clearFromEnvironment)
		{
			_soundEffectPlayer.Stop();
			if (clearFromEnvironment)
			{
				_environment.currentSoundName = null;
				_environment.currentSoundRepeating = false;
			}
		}

		/*
		 * Stops current music playback
		 */
		private void StopMusic(bool clearFromEnvironment)
		{
			_backgroundMusicPlayer.Stop();
			if (clearFromEnvironment)
			{
				_environment.currentSongName = null;
				_environment.currentSongRepeating = false;
			}
		}

		/*
		 * Creates media player and plays requested video covering whole window
		 */
		private bool PlayVideo(string name, double volume, bool allowProgress, bool hideUI)
		{
			var video = _assets.videos.Find(vid => vid.name == name);
			if (video == null) return false;

			if (allowProgress)
			{
				Settings.allowProgress = true;
			}
			else
			{
				Settings.allowProgress = false;
			}

			if (hideUI)
			{
				ManipulateUI(false);
			}

			_videoPlayer = new MediaElement
			{
				Name = hideUI.ToString(),
				Source = new Uri(video.location),
				LoadedBehavior = MediaState.Manual,
				Height = Settings.windowHeight,
				Width = Settings.windowWidth
			};
			ViewportContainer.Children.Add(_videoPlayer);
			Canvas.SetLeft(_videoPlayer, 0);
			Canvas.SetTop(_videoPlayer, 0);
			Panel.SetZIndex(_videoPlayer, 3);
			_environment.temporaryUIlayer1.Add(_videoPlayer.Name);

			_videoPlayer.Volume = volume * Settings.soundVolumeMultiplier;
			_videoPlayer.Play();
			_videoPlayer.MediaEnded += (sender, args) =>
			{
				var player = (MediaElement) sender;
				if (player.Name == "True")
				{
					ManipulateUI(true);
				}
				_videoPlayer.Stop();
				if (Settings.allowProgress == false)
				{
					Settings.allowProgress = true;
				}
				ClearTemporaryUiElements(1);
			};
			return true;
		}

		/*
		 * Stops a video playback if playing
		 */
		private bool StopVideo()
		{
			if (_videoPlayer != null)
			{
				_videoPlayer.Stop();
				ClearTemporaryUiElements(1);
				return true;
			}

			return false;
		}
		/*
		 * Pauses the gameplay and shows multiple buttons corresponding to choice options
		 */
		private bool ShowChoice(string choiceName)
		{
			Choice ch = _assets.choices.Find(i => i.name == choiceName);
			if (ch == null) return false;

			int textTop = 100;
			Settings.allowProgress = false;
			_textBlock.Text = ch.text;

			foreach (Option opt in ch.options)
			{
				textTop += 60;
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
				button.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ChoiceButtonClick));
				ViewportContainer.Children.Add(button);
				Canvas.SetLeft(button, Settings.windowWidth / 2 - button.Width / 2);
				Canvas.SetTop(button, textTop);
				Panel.SetZIndex(button, 4);
				_environment.choiceButtonNames.Add(button.Name);
			}

			return true;
		}
		
		/*
		 * Event handler for clicking a button in choice, clears the choice, resumes gameplay and jumps to specified label
		 */
		private void ChoiceButtonClick(object sender, RoutedEventArgs e)
		{
			JumpToLabel(((Button)sender).Name);
			
			while (_environment.choiceButtonNames.Count > 0)
			{
				Button btn = (Button)LogicalTreeHelper.FindLogicalNode(ViewportContainer, _environment.choiceButtonNames[0]);
				ViewportContainer.Children.Remove(btn);
				_environment.choiceButtonNames.RemoveAt(0);
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
			Assets.Label label = _assets.labels.Find(i => i.name == labelName);
			if (label == null)
			{
				MessageBox.Show("Label " + labelName + " was not found.");
				return;
			}
			currentScriptIndex = label.scriptIndex;
			_scripts[currentScriptIndex].currentLine = label.lineNumber;
		}

		/*
		 * Ends game and returns to main menu
		 */
		private void EndGame()
		{
			ClearTemporaryUiElements(1);
			ClearViewport(true);
			currentScriptIndex = 0;
			_scripts[currentScriptIndex].currentLine = _scripts[currentScriptIndex].firstGameplayLine;
			MainMenu(true, true);
		}

		/*
		 * Shows or hides UI elements ingame
		 */
		private void ManipulateUI(bool showUI)
		{
			try
			{
				if (!showUI)
				{
					_nameBlock.Visibility = Visibility.Hidden;
					_textBlock.Visibility = Visibility.Hidden;
					_nameBlockBorder.Visibility = Visibility.Hidden;
					_textBlockBorder.Visibility = Visibility.Hidden;
					_buttonBorder.Visibility = Visibility.Hidden;
					_saveButton.Visibility = Visibility.Hidden;
					_saveButton.IsEnabled = false;
					_menuButton.Visibility = Visibility.Hidden;
					_menuButton.IsEnabled = false;
					_exitButton.Visibility = Visibility.Hidden;
					_exitButton.IsEnabled = false;
					Settings.UIvisible = false;
				}
				else
				{
					_nameBlock.Visibility = Visibility.Visible;
					_textBlock.Visibility = Visibility.Visible;
					_nameBlockBorder.Visibility = Visibility.Visible;
					_textBlockBorder.Visibility = Visibility.Visible;
					_buttonBorder.Visibility = Visibility.Visible;
					_saveButton.Visibility = Visibility.Visible;
					_saveButton.IsEnabled = true;
					_menuButton.Visibility = Visibility.Visible;
					_menuButton.IsEnabled = true;
					_exitButton.Visibility = Visibility.Visible;
					_exitButton.IsEnabled = true;
					Settings.UIvisible = true;
				}
			}
			catch (Exception)
			{
			}
		}

		/*
		 * Executes next line of script. Can execute multiple lines if they are non-stopping
		 */
		private void TriggerNextCommand()
		{
			Settings.executeNext = true;
			while (Settings.executeNext)
			{
				List<string> command;
				try
				{
					command = ProcessScriptLine();
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
					return;
				}

				if (command == null)
				{
					EndGame();
					return;
				}
				
				if (command.Count == 0 || Settings.SetupKeywordList.Contains(command[0])) continue;

				if (Settings.afterLoad)
				{
					ExecuteCommand(command, true);
					Settings.afterLoad = false;
				}
				else
				{
					ExecuteCommand(command, false);
				}
				
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

		private void Game_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!Settings.inGame || !Settings.allowProgress)
			{
				return;
			}
			ManipulateUI(!Settings.UIvisible);
		}

		private void Game_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!Settings.gameLoaded)
			{
				return;
			}

			Settings.windowWidth = this.ActualWidth;
			Settings.windowHeight = this.ActualHeight;

			// Change global image settings
			_blackBackgroundConstant.Width = Settings.windowWidth;
			_blackBackgroundConstant.Height = Settings.windowHeight;
			_backgroundImage.Width = Settings.windowWidth;
			_backgroundImage.Height = Settings.windowHeight;

			// Reload screen elements
			if (Settings.inGame)
			{
				ClearViewport(false);
				NewGame(false);
				LoadSurroundingsFromEnvironment(_environment);
			}
			else
			{
				MainMenu(true, false);
			}
		}
	}
}