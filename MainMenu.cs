using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using VNet.Assets;

namespace VNet
{
	public partial class Game : Window
	{
		/*
		 * Loads main menu elements
		 */
		private void MainMenu(bool restartBackgroundAndMusic)
		{
			ClearViewport(true);

			TextBlock gameNameTextBlock = new TextBlock
			{
				Name = "gameNameTextBlock",
				Text = Settings.gameName,
				FontSize = 48,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.AliceBlue)
			};
			ViewportContainer.Children.Add(gameNameTextBlock);
			Canvas.SetLeft(gameNameTextBlock, 100);
			Canvas.SetTop(gameNameTextBlock, 50);
			Panel.SetZIndex(gameNameTextBlock, 2);

			Savegame lastSave = FindLastSave();
			if (lastSave != null)
			{
				Button continueButton = new Button
				{
					Name = "continueButton",
					Width = 320,
					Height = 48,
					Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
				};
				TextBlock continueTextBlock = new TextBlock
				{
					Name = "continueTextBlock",
					Text = "Continue",
					FontSize = 20,
					FontWeight = FontWeights.Bold,
					Foreground = new SolidColorBrush(Colors.White)
				};
				continueButton.Content = continueTextBlock;
				continueButton.Click += (sender, args) =>
				{
					LoadGame(lastSave);
				};
				ViewportContainer.Children.Add(continueButton);
				Canvas.SetLeft(continueButton, 100);
				Canvas.SetTop(continueButton, 200);
				Panel.SetZIndex(continueButton, 2);
			}

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
			newGameButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(NewGameButtonClick));
			ViewportContainer.Children.Add(newGameButton);
			Canvas.SetLeft(newGameButton, 100);
			Canvas.SetTop(newGameButton, 280);
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
			loadGameButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(LoadGameButtonClick));
			ViewportContainer.Children.Add(loadGameButton);
			Canvas.SetLeft(loadGameButton, 100);
			Canvas.SetTop(loadGameButton, 360);
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
			optionsButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OptionsButtonClick));
			ViewportContainer.Children.Add(optionsButton);
			Canvas.SetLeft(optionsButton, 100);
			Canvas.SetTop(optionsButton, 440);
			Panel.SetZIndex(optionsButton, 2);

			Button exitButton = new Button
			{
				Name = "exitButton",
				Width = 320,
				Height = 48,
				Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
			};
			TextBlock exitTextBlock = new TextBlock
			{
				Name = "exitTextBlock",
				Text = "Exit",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			exitButton.Content = exitTextBlock;
			exitButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ExitButtonClick));
			ViewportContainer.Children.Add(exitButton);
			Canvas.SetLeft(exitButton, 100);
			Canvas.SetTop(exitButton, 520);
			Panel.SetZIndex(exitButton, 2);

			// Menu background image and music
			if (restartBackgroundAndMusic)
			{
				_backgroundMusicPlayer.Stop();
				_soundEffectPlayer.Stop();

				ShowBackground("menu_background", 10);
				PlaySound("menu_music", 1, true);
			}
		}

		/*
		 * Returns the save game with the latest date
		 */
		private Savegame FindLastSave()
		{
			Savegame mostRecentSave = new Savegame();
			DateTime mostRecentDate = DateTime.MinValue;
			bool saveFound = false;
			for (int i = 0; i < 10; i++)
			{
				Savegame save = FindSaveGame(i);
				if (save != null && save.currentTime > mostRecentDate)
				{
					mostRecentSave = save;
					saveFound = true;
				}
			}

			return saveFound ? mostRecentSave : null;
		}

		/*
		 * Main menu button event handlers
		 */
		private void NewGameButtonClick(object sender, RoutedEventArgs e)
		{
			ClearViewport(false);
			StopSound(true);
			StopMusic(true);
			NewGame(true);
		}
		private void LoadGameButtonClick(object sender, RoutedEventArgs e)
		{
			ClearViewport(true);
			Settings.deleteGamesOnLoadScreen = false;
			Settings.deletedSaveSlot = -1;
			ShowLoadGameScreen();
		}
		private void OptionsButtonClick(object sender, RoutedEventArgs e)
		{
			ClearViewport(true);
			ShowOptionsScreen();
		}
		private void ExitButtonClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
		
		/*
		 * Show screen containing all saved games found, allow user to load a game
		 */
		private void ShowLoadGameScreen()
		{
			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = Settings.deleteGamesOnLoadScreen ? "Select slot to delete:" : "Select slot to load:",
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 20)
			};

			// Create grid for save slots
			Grid slotGrid = new Grid
			{
				Name = "slotGrid",
				Margin = new Thickness(10, 10, 10, 10),
			};
			slotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			slotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			slotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			slotGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			slotGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			slotGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

			// Create save slots
			for (var i = 0; i < 3; i++)
			{
				for (var j = 1; j <= 3; j++)
				{
					// Gets the save file for this slot
					Savegame save = Savegame.DeserializeSaveGame(3 * i + j);

					// Creates UI content
					Button slot = new Button()
					{
						Name = "save_0" + (3 * i + j),
						Margin = new Thickness(10, 10, 10, 10),
						Width = 300,
						Height = 100
					};
					slotGrid.Children.Add(slot);
					Grid.SetColumn(slot, j - 1);
					Grid.SetRow(slot, i);

					if (save == null)
					{
						TextBlock emptySlotText = new TextBlock
						{
							Name = "emptySlotText",
							Text = "* Empty slot *",
							FontSize = 21,
							FontWeight = FontWeights.Bold
						};
						slot.Content = emptySlotText;
					}
					else if (Settings.deletedSaveSlot == (3 * i + j))
					{
						TextBlock emptySlotText = new TextBlock
						{
							Name = "emptySlotText",
							Text = "* Empty slot *",
							FontSize = 21,
							FontWeight = FontWeights.Bold
						};
						slot.Content = emptySlotText;
						Settings.deletedSaveSlot = -1;
					}
					else
					{
						slot.Click += LoadSlotOnClick;

						Background selBackground =
							_assets.backgrounds.Find(bg => bg.name == save.currentEnvironment.currentBackgroundName);
						Image slotImage = new Image
						{
							Source = new BitmapImage(selBackground.imageUri)
						};

						TextBlock slotText = new TextBlock
						{
							Name = "slotText",
							Text = save.currentEnvironment.fullText,
							TextWrapping = TextWrapping.Wrap,
							TextTrimming = TextTrimming.CharacterEllipsis,
							MaxHeight = 70,
							FontSize = 14,
							FontWeight = FontWeights.DemiBold
						};

						TextBlock slotDate = new TextBlock
						{
							Name = "slotDate",
							Text = save.currentTime.ToString(CultureInfo.CurrentCulture),
							TextTrimming = TextTrimming.CharacterEllipsis,
							FontSize = 14,
							Margin = new Thickness(0, 10, 0, 0)
						};

						Grid innerRightSlotGrid = new Grid
						{
							Name = "innerRightSlotGrid",
							Margin = new Thickness(5, 0, 0, 0)
						};
						innerRightSlotGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
						innerRightSlotGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
						innerRightSlotGrid.Children.Add(slotText);
						innerRightSlotGrid.Children.Add(slotDate);
						Grid.SetRow(slotText, 0);
						Grid.SetRow(slotDate, 1);

						Grid innerSlotGrid = new Grid
						{
							Name = "innerSlotGrid",
						};
						innerSlotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
						innerSlotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
						innerSlotGrid.Children.Add(innerRightSlotGrid);
						innerSlotGrid.Children.Add(slotImage);
						Grid.SetColumn(slotImage, 0);
						Grid.SetColumn(innerRightSlotGrid, 1);
						slot.Content = innerSlotGrid;
					}
				}
			}

			TextBlock changeModeTextBlock = new TextBlock
			{
				Name = "changeModeTextBlock",
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Text = Settings.deleteGamesOnLoadScreen ? "Load" : "Delete"
			};

			Button changeModeButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = changeModeTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5),
				Width = 240
			};
			changeModeButton.Click += (sender, args) =>
			{
				Settings.deleteGamesOnLoadScreen = !Settings.deleteGamesOnLoadScreen;
				ClearViewport(true);
				ShowLoadGameScreen();
			};

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = "Cancel",
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5),
				Width = 240
			};
			cancelButton.Click += (sender, args) =>
			{
				MainMenu(false);
			};

			Grid buttonGrid = new Grid
			{
				Name = "buttonGrid",
				Margin = new Thickness(10, 10, 10, 10),
			};
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			buttonGrid.Children.Add(changeModeButton);
			buttonGrid.Children.Add(cancelButton);
			Grid.SetColumn(changeModeButton, 0);
			Grid.SetColumn(cancelButton, 1);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(slotGrid);
			verticalStackPanel.Children.Add(buttonGrid);

			Border saveMenuBorder = new Border
			{
				Name = "saveMenuBorder",
				BorderThickness = new Thickness(5, 5, 5, 5),
				BorderBrush = new SolidColorBrush(Colors.White),
				Background = new SolidColorBrush(Color.FromArgb(148, 0, 0, 0)),
				Child = verticalStackPanel,
				MinWidth = 960,
				MinHeight = 540
			};
			ViewportContainer.Children.Add(saveMenuBorder);
			Canvas.SetLeft(saveMenuBorder, 160);
			Canvas.SetTop(saveMenuBorder, 90);
			Panel.SetZIndex(saveMenuBorder, 7);

			// Background blur
			BlurEffect blur = new BlurEffect { Radius = 0 };
			DoubleAnimation blurAnimation = new DoubleAnimation(0.0, 5.0, new Duration(TimeSpan.FromMilliseconds(300)));
			_backgroundImage.Effect = blur;
			blur.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
		}

		/*
		 * Triggers loading or deleting of chosen slot on click
		 */
		private void LoadSlotOnClick(object sender, RoutedEventArgs e)
		{
			if (Settings.deleteGamesOnLoadScreen)
			{
				Button btn = (Button)sender;
				if (Int32.TryParse(btn.Name.Substring(btn.Name.Length - 1, 1), out int saveIndex))
				{
					DeleteSavedGame(saveIndex);
					Settings.deletedSaveSlot = saveIndex;
					ClearViewport(true);
					ShowLoadGameScreen();
				}
			}
			else
			{
				Button btn = (Button)sender;
				if (Int32.TryParse(btn.Name.Substring(btn.Name.Length - 1, 1), out int saveIndex))
				{
					LoadGame(FindSaveGame(saveIndex));
				}
			}
		}

		/*
		 * Deletes save game with chosen index
		 */
		private void DeleteSavedGame(int saveIndex)
		{
			File.Delete(Settings.SaveFilePath(saveIndex));
		}

		/*
		 * Loads and starts game based on the provided save index (0-9)
		 */
		private Savegame FindSaveGame(int saveGameIndex)
		{
			Savegame save = Savegame.DeserializeSaveGame(saveGameIndex);
			return save;
		}

		private void LoadGame(Savegame save)
		{
			// Set required gameplay elements
			currentScriptIndex = save.currentScriptIndex;
			scripts[currentScriptIndex].currentLine = save.currentScriptLine;
			scripts[currentScriptIndex].currentPositionInLine = 0;
			_environment = save.currentEnvironment;
			_assets.variables = save.currentVariables;

			// Start game
			StopSound(false);
			StopMusic(false);
			ClearViewport(false);
			NewGame(false);

			// Load environment from save
			LoadSurroundingsFromEnvironment(_environment);

			Settings.inGame = true;
			Settings.allowProgress = true;
			Settings.afterLoad = true;
		}

		/*
		 * Loads game surroundings from given environment object.
		 */
		private void LoadSurroundingsFromEnvironment(GameEnvironment environment)
		{
			if (environment.currentBackgroundName != null)
			{
				ShowBackground(environment.currentBackgroundName, 100);
			}
			if (environment.leftCharacterName != null)
			{
				ShowCharacter(environment.leftCharacterName, environment.leftCharacterMood, 100, "left");
			}
			if (environment.centerCharacterName != null)
			{
				ShowCharacter(environment.centerCharacterName, environment.centerCharacterMood, 100, "center");
			}
			if (environment.rightCharacterName != null)
			{
				ShowCharacter(environment.rightCharacterName, environment.rightCharacterMood, 100, "right");
			}
			if (environment.nameOfCharacterTalking != null)
			{
				ShowText(environment.nameOfCharacterTalking, environment.fullText, environment.nameOfCharacterTalking == "");
			}
			if (environment.currentSongName != null)
			{
				PlaySound(environment.currentSongName, environment.currentSongVolume, environment.currentSongRepeating);
			}
			if (environment.currentSoundName != null)
			{
				PlaySound(environment.currentSoundName, environment.currentSoundVolume, environment.currentSoundRepeating);
			}
		}

		/*
		 * Show screen containing various options like text speed, music/sound volume...
		 */
		private void ShowOptionsScreen()
		{
			TextBlock optionsTextBlock = new TextBlock
			{
				Name = "gameNameTextBlock",
				Text = "Options",
				FontSize = 48,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.AliceBlue)
			};
			ViewportContainer.Children.Add(optionsTextBlock);
			Canvas.SetLeft(optionsTextBlock, 100);
			Canvas.SetTop(optionsTextBlock, 50);
			Panel.SetZIndex(optionsTextBlock, 2);

			// Text speed setting
			TextBlock textSpeedTextBlock = new TextBlock
			{
				Name = "textSpeedTextBlock",
				Text = "Text speed",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			ViewportContainer.Children.Add(textSpeedTextBlock);
			Canvas.SetLeft(textSpeedTextBlock, 100);
			Canvas.SetTop(textSpeedTextBlock, 200);
			Panel.SetZIndex(textSpeedTextBlock, 2);

			Slider textSpeedSlider = new Slider
			{
				Width = 320,
				TickPlacement = TickPlacement.BottomRight,
				Minimum = 0.01,
				Maximum = 0.2,
				Value = 2.0 / Settings.textDisplaySpeedInMiliseconds,
				TickFrequency = 0.01,
				IsSnapToTickEnabled = true
			};
			textSpeedSlider.ValueChanged += (sender, args) =>
			{
				Settings.textDisplaySpeedInMiliseconds = (int)Math.Round(2.0 / textSpeedSlider.Value);
			};
			ViewportContainer.Children.Add(textSpeedSlider);
			Canvas.SetLeft(textSpeedSlider, 400);
			Canvas.SetTop(textSpeedSlider, 200);
			Panel.SetZIndex(textSpeedSlider, 2);

			// Music volume setting
			TextBlock musicVolumeTextBlock = new TextBlock
			{
				Name = "musicVolumeTextBlock",
				Text = "Music volume",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			ViewportContainer.Children.Add(musicVolumeTextBlock);
			Canvas.SetLeft(musicVolumeTextBlock, 100);
			Canvas.SetTop(musicVolumeTextBlock, 250);
			Panel.SetZIndex(musicVolumeTextBlock, 2);

			Slider musicVolumeSlider = new Slider
			{
				Width = 320,
				TickPlacement = TickPlacement.BottomRight,
				Minimum = 0,
				Maximum = 1,
				Value = Settings.musicVolumeMultiplier,
				TickFrequency = 0.05,
				IsSnapToTickEnabled = true
			};
			musicVolumeSlider.ValueChanged += (sender, args) =>
			{
				Settings.musicVolumeMultiplier = musicVolumeSlider.Value;
				_backgroundMusicPlayer.Volume = musicVolumeSlider.Value;
			};
			ViewportContainer.Children.Add(musicVolumeSlider);
			Canvas.SetLeft(musicVolumeSlider, 400);
			Canvas.SetTop(musicVolumeSlider, 250);
			Panel.SetZIndex(musicVolumeSlider, 2);

			// Sound volume setting
			TextBlock soundVolumeTextBlock = new TextBlock
			{
				Name = "soundVolumeTextBlock",
				Text = "Sound effects volume",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			ViewportContainer.Children.Add(soundVolumeTextBlock);
			Canvas.SetLeft(soundVolumeTextBlock, 100);
			Canvas.SetTop(soundVolumeTextBlock, 300);
			Panel.SetZIndex(soundVolumeTextBlock, 2);

			Slider soundVolumeSlider = new Slider
			{
				Width = 320,
				TickPlacement = TickPlacement.BottomRight,
				Minimum = 0,
				Maximum = 1,
				Value = Settings.soundVolumeMultiplier,
				TickFrequency = 0.05,
				IsSnapToTickEnabled = true
			};
			soundVolumeSlider.ValueChanged += (sender, args) =>
			{
				Settings.soundVolumeMultiplier = soundVolumeSlider.Value;
				_soundEffectPlayer.Volume = soundVolumeSlider.Value;
			};
			ViewportContainer.Children.Add(soundVolumeSlider);
			Canvas.SetLeft(soundVolumeSlider, 400);
			Canvas.SetTop(soundVolumeSlider, 300);
			Panel.SetZIndex(soundVolumeSlider, 2);

			Button backButton = new Button
			{
				Name = "backButton",
				Width = 320,
				Height = 48,
				Background = new SolidColorBrush(Color.FromArgb(192, 16, 16, 16))
			};
			TextBlock backTextBlock = new TextBlock
			{
				Name = "backTextBlock",
				Text = "Back",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			backButton.Content = backTextBlock;
			backButton.Click += (sender, args) =>
			{
				MainMenu(false);
			};
			ViewportContainer.Children.Add(backButton);
			Canvas.SetLeft(backButton, 100);
			Canvas.SetTop(backButton, 600);
			Panel.SetZIndex(backButton, 2);

			// Color character names setting
			TextBlock colorNamesTextBlock = new TextBlock
			{
				Name = "colorNamesTextBlock",
				Text = "Color character names",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			ViewportContainer.Children.Add(colorNamesTextBlock);
			Canvas.SetLeft(colorNamesTextBlock, 100);
			Canvas.SetTop(colorNamesTextBlock, 350);
			Panel.SetZIndex(colorNamesTextBlock, 2);

			CheckBox colorNamesCheckBox = new CheckBox
			{
				Name = "colorNamesCheckBox",
				IsChecked = Settings.colorCharacterNames
			};
			colorNamesCheckBox.Checked += (sender, args) => { Settings.colorCharacterNames = true; };
			colorNamesCheckBox.Unchecked += (sender, args) => { Settings.colorCharacterNames = false; };
			ViewportContainer.Children.Add(colorNamesCheckBox);
			Canvas.SetLeft(colorNamesCheckBox, 400);
			Canvas.SetTop(colorNamesCheckBox, 350);
			Panel.SetZIndex(colorNamesCheckBox, 2);

			// Color text box border setting
			TextBlock colorBordersTextBlock = new TextBlock
			{
				Name = "colorBordersTextBlock",
				Text = "Color text borders",
				FontSize = 20,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White)
			};
			ViewportContainer.Children.Add(colorBordersTextBlock);
			Canvas.SetLeft(colorBordersTextBlock, 100);
			Canvas.SetTop(colorBordersTextBlock, 400);
			Panel.SetZIndex(colorBordersTextBlock, 2);

			CheckBox colorBordersCheckBox = new CheckBox
			{
				Name = "colorBordersCheckBox",
				IsChecked = Settings.colorTextBorders
			};
			colorBordersCheckBox.Checked += (sender, args) => { Settings.colorTextBorders = true; };
			colorBordersCheckBox.Unchecked += (sender, args) => { Settings.colorTextBorders = false; };
			ViewportContainer.Children.Add(colorBordersCheckBox);
			Canvas.SetLeft(colorBordersCheckBox, 400);
			Canvas.SetTop(colorBordersCheckBox, 400);
			Panel.SetZIndex(colorBordersCheckBox, 2);

			// Background blur
			BlurEffect blur = new BlurEffect { Radius = 0 };
			DoubleAnimation blurAnimation = new DoubleAnimation(0.0, 5.0, new Duration(TimeSpan.FromMilliseconds(300)));
			_backgroundImage.Effect = blur;
			blur.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
		}
	}
}