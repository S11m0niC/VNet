using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VNet
{
	public partial class Game : Window
	{
		/*
		 * Show save game screen
		 */
		private void SaveButtonOnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		/*
		 * Show exit confirmation dialogue
		 */
		private void ExitButtonOnClick(object sender, RoutedEventArgs e)
		{
			Settings.inGame = false;

			Border darkOverlay = new Border()
			{
				Name = "darkOverlay",
				Width = Settings.windowWidth,
				Height = Settings.windowHeight,
				Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))
			};
			ViewportContainer.Children.Add(darkOverlay);
			Panel.SetZIndex(darkOverlay, 6);
			_environment.temporaryUIElementNames.Add(darkOverlay.Name);

			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = "Exit game?\n(Unsaved progress will be lost)",
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 10)
			};
			_environment.temporaryUIElementNames.Add(questionTextBlock.Name);

			TextBlock exitTextBlock = new TextBlock
			{
				Name = "exitTextBlock",
				Text = "Exit",
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIElementNames.Add(exitTextBlock.Name);

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = "Cancel",
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIElementNames.Add(cancelTextBlock.Name);

			Button exitButton = new Button
			{
				Name = "exitButton",
				IsDefault = true,
				Content = exitTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			exitButton.Click += SecondExitButtonOnClick;
			_environment.temporaryUIElementNames.Add(exitButton.Name);

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			cancelButton.Click += CancelButtonOnClick;
			_environment.temporaryUIElementNames.Add(cancelButton.Name);

			Grid buttonGrid = new Grid
			{
				Name = "buttonGrid",
				Margin = new Thickness(10, 10, 10, 10),
				MinWidth = 400
			};
			ColumnDefinition col1 = new ColumnDefinition();
			col1.Width = new GridLength(50, GridUnitType.Star);
			ColumnDefinition col2 = new ColumnDefinition();
			col2.Width = new GridLength(50, GridUnitType.Star);
			buttonGrid.ColumnDefinitions.Add(col1);
			buttonGrid.ColumnDefinitions.Add(col2);
			buttonGrid.Children.Add(exitButton);
			buttonGrid.Children.Add(cancelButton);
			Grid.SetColumn(exitButton, 0);
			Grid.SetColumn(cancelButton, 1);
			_environment.temporaryUIElementNames.Add(buttonGrid.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
				MinHeight = 150
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(buttonGrid);
			_environment.temporaryUIElementNames.Add(verticalStackPanel.Name);

			Border exitMenuBorder = new Border
			{
				Name = "exitMenuBorder",
				BorderThickness = new Thickness(5, 5, 5, 5),
				BorderBrush = new SolidColorBrush(Colors.White),
				Background = new SolidColorBrush(Color.FromArgb(148, 0, 0, 0)),
				Child = verticalStackPanel
			};
			ViewportContainer.Children.Add(exitMenuBorder);
			Canvas.SetLeft(exitMenuBorder, (Settings.windowWidth / 2) - 200);
			Canvas.SetTop(exitMenuBorder, (Settings.windowHeight / 2) - 75);
			Panel.SetZIndex(exitMenuBorder, 7);

			_environment.temporaryUIElementNames.Add(exitMenuBorder.Name);
		}
		private void SecondExitButtonOnClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		/*
		 * Show return to menu confirmation dialogue
		 */
		private void MenuButtonOnClick(object sender, RoutedEventArgs e)
		{
			Settings.inGame = false;

			Border darkOverlay = new Border()
			{
				Name = "darkOverlay",
				Width = Settings.windowWidth,
				Height = Settings.windowHeight,
				Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))
			};
			ViewportContainer.Children.Add(darkOverlay);
			Panel.SetZIndex(darkOverlay, 6);
			_environment.temporaryUIElementNames.Add(darkOverlay.Name);

			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = "Return to main menu?\n(Unsaved progress will be lost)",
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 10)
			};
			_environment.temporaryUIElementNames.Add(questionTextBlock.Name);

			TextBlock menuTextBlock = new TextBlock
			{
				Name = "menuTextBlock",
				Text = "Return",
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIElementNames.Add(menuTextBlock.Name);

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = "Cancel",
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIElementNames.Add(cancelTextBlock.Name);

			Button menuButton = new Button
			{
				Name = "menuButton",
				IsDefault = true,
				Content = menuTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			menuButton.Click += SecondMenuButtonOnClick;
			_environment.temporaryUIElementNames.Add(menuButton.Name);

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			cancelButton.Click += CancelButtonOnClick;
			_environment.temporaryUIElementNames.Add(cancelButton.Name);

			Grid buttonGrid = new Grid
			{
				Name = "buttonGrid",
				Margin = new Thickness(10, 10, 10, 10),
				MinWidth = 400
			};
			ColumnDefinition col1 = new ColumnDefinition();
			col1.Width = new GridLength(50, GridUnitType.Star);
			ColumnDefinition col2 = new ColumnDefinition();
			col2.Width = new GridLength(50, GridUnitType.Star);
			buttonGrid.ColumnDefinitions.Add(col1);
			buttonGrid.ColumnDefinitions.Add(col2);
			buttonGrid.Children.Add(menuButton);
			buttonGrid.Children.Add(cancelButton);
			Grid.SetColumn(menuButton, 0);
			Grid.SetColumn(cancelButton, 1);
			_environment.temporaryUIElementNames.Add(buttonGrid.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
				MinHeight = 150
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(buttonGrid);
			_environment.temporaryUIElementNames.Add(verticalStackPanel.Name);

			Border exitMenuBorder = new Border
			{
				Name = "exitMenuBorder",
				BorderThickness = new Thickness(5, 5, 5, 5),
				BorderBrush = new SolidColorBrush(Colors.White),
				Background = new SolidColorBrush(Color.FromArgb(148, 0, 0, 0)),
				Child = verticalStackPanel
			};
			ViewportContainer.Children.Add(exitMenuBorder);
			Canvas.SetLeft(exitMenuBorder, (Settings.windowWidth / 2) - 200);
			Canvas.SetTop(exitMenuBorder, (Settings.windowHeight / 2) - 75);
			Panel.SetZIndex(exitMenuBorder, 7);

			_environment.temporaryUIElementNames.Add(exitMenuBorder.Name);
		}
		private void SecondMenuButtonOnClick(object sender, RoutedEventArgs e)
		{
			ClearTemporaryUiElements();
			ClearViewport(true);
			_currentScript.currentLine = _currentScript.firstGameplayLine;
			MainMenu(true);
		}

		private void CancelButtonOnClick(object sender, RoutedEventArgs e)
		{
			ClearTemporaryUiElements();
			Settings.inGame = true;
		}
	}
}
