using System;
using System.Collections.Generic;
using System.Globalization;
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
			Settings.inGame = false;

			Border darkOverlay = new Border()
			{
				Name = "darkOverlay",
				Width = Settings.windowWidth,
				Height = Settings.windowHeight,
				Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0))
			};
			ViewportContainer.Children.Add(darkOverlay);
			Panel.SetZIndex(darkOverlay, 6);
			_environment.temporaryUIElementNames.Add(darkOverlay.Name);

			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = "Select save slot:",
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 20)
			};
			_environment.temporaryUIElementNames.Add(questionTextBlock.Name);

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
			_environment.temporaryUIElementNames.Add(slotGrid.Name);

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
					slot.Click += SlotOnClick;
					slotGrid.Children.Add(slot);
					Grid.SetColumn(slot, j - 1);
					Grid.SetRow(slot, i);
					_environment.temporaryUIElementNames.Add(slot.Name);

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
						_environment.temporaryUIElementNames.Add(emptySlotText.Name);
					}
					else
					{
						TextBlock slotText = new TextBlock
						{
							Name = "slotText",
							Text = save.currentEnvironment.fullText,
							FontSize = 16
						};
						_environment.temporaryUIElementNames.Add(slotText.Name);

						TextBlock slotDate = new TextBlock
						{
							Name = "slotDate",
							Text = save.currentTime.ToString(CultureInfo.InvariantCulture),
							FontSize = 16
						};
						_environment.temporaryUIElementNames.Add(slotDate.Name);

						StackPanel slotRightPanel = new StackPanel
						{
							Name = "slotRightPanel",
							Orientation = Orientation.Vertical
						};
						slotRightPanel.Children.Add(slotText);
						slotRightPanel.Children.Add(slotDate);
						_environment.temporaryUIElementNames.Add(slotRightPanel.Name);

						Grid innerSlotGrid = new Grid
						{
							Name = "innerSlotGrid",
						};
						innerSlotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
						innerSlotGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
						innerSlotGrid.Children.Add(slotRightPanel);
						Grid.SetColumn(slotRightPanel, 1);
						slot.Content = innerSlotGrid;
						_environment.temporaryUIElementNames.Add(innerSlotGrid.Name);
					}
				}
			}

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = "Cancel",
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIElementNames.Add(cancelTextBlock.Name);

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5),
				Width = 240
			};
			cancelButton.Click += CancelButtonOnClick;
			_environment.temporaryUIElementNames.Add(cancelButton.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(slotGrid);
			verticalStackPanel.Children.Add(cancelButton);
			_environment.temporaryUIElementNames.Add(verticalStackPanel.Name);

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

			_environment.temporaryUIElementNames.Add(saveMenuBorder.Name);
		}

		// Event handler for clicking save slot
		private void SlotOnClick(object sender, RoutedEventArgs e)
		{
			Button btn = (Button) sender;
			if (Int32.TryParse(btn.Name.Substring(btn.Name.Length - 1, 1), out int saveIndex))
			{
				SaveGame(saveIndex);
			}
			ClearTemporaryUiElements();
			Settings.inGame = true;
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
				Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0))
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
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
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
				Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0))
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
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
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
