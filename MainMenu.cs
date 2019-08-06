using System;
using System.Collections.Generic;
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
using System.Xml.Serialization;

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
				Text = "VNet",
				FontSize = 48,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.AliceBlue)
			};
			ViewportContainer.Children.Add(gameNameTextBlock);
			Canvas.SetLeft(gameNameTextBlock, 100);
			Canvas.SetTop(gameNameTextBlock, 50);
			Panel.SetZIndex(gameNameTextBlock, 2);

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
			loadGameButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(LoadGameButtonClick));
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
			optionsButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OptionsButtonClick));
			ViewportContainer.Children.Add(optionsButton);
			Canvas.SetLeft(optionsButton, 100);
			Canvas.SetTop(optionsButton, 400);
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
			Canvas.SetTop(exitButton, 500);
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
		 * Main menu button event handlers
		 */
		private void NewGameButtonClick(object sender, RoutedEventArgs e)
		{
			ClearViewport(false);
			StopSound();
			StopMusic();
			NewGame();
		}
		private void LoadGameButtonClick(object sender, RoutedEventArgs e)
		{
			ClearViewport(true);
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
			// TODO
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
