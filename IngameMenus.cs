using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using VNet.Assets;

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
			_environment.temporaryUIlayer1.Add(darkOverlay.Name);

			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = _environment.currentLanguage.UI_saveMenu_instructions,
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 20)
			};
			_environment.temporaryUIlayer1.Add(questionTextBlock.Name);

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
			_environment.temporaryUIlayer1.Add(slotGrid.Name);

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
						Background = new SolidColorBrush(Colors.White),
						Width = 300,
						Height = 100
					};
					slotGrid.Children.Add(slot);
					Grid.SetColumn(slot, j - 1);
					Grid.SetRow(slot, i);
					_environment.temporaryUIlayer1.Add(slot.Name);

					if (save == null)
					{
						TextBlock emptySlotText = new TextBlock
						{
							Name = "emptySlotText",
							Text = _environment.currentLanguage.UI_emptySlot,
							FontSize = 21,
							FontWeight = FontWeights.Bold
						};
						slot.Content = emptySlotText;
						slot.Click += SlotOnClick;
						_environment.temporaryUIlayer1.Add(emptySlotText.Name);
					}
					else
					{
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
							FontSize = 14,
							FontWeight = FontWeights.DemiBold,
							MaxHeight = 70,
							TextWrapping = TextWrapping.Wrap,
							TextTrimming = TextTrimming.CharacterEllipsis
						};
						_environment.temporaryUIlayer1.Add(slotText.Name);

						TextBlock slotDate = new TextBlock
						{
							Name = "slotDate",
							Text = save.currentTime.ToString(CultureInfo.CurrentCulture),
							FontSize = 14,
							TextTrimming = TextTrimming.CharacterEllipsis,
							Margin = new Thickness(0, 10, 0, 0)
						};
						_environment.temporaryUIlayer1.Add(slotDate.Name);

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
						innerSlotGrid.Children.Add(slotImage);
						innerSlotGrid.Children.Add(innerRightSlotGrid);
						Grid.SetColumn(slotImage, 0);
						Grid.SetColumn(innerRightSlotGrid, 1);
						slot.Content = innerSlotGrid;
						slot.Click += SlotOverwriteOnClick;
						_environment.temporaryUIlayer1.Add(innerSlotGrid.Name);
					}
				}
			}

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = _environment.currentLanguage.UI_cancel,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer1.Add(cancelTextBlock.Name);

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
			_environment.temporaryUIlayer1.Add(cancelButton.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(slotGrid);
			verticalStackPanel.Children.Add(cancelButton);
			_environment.temporaryUIlayer1.Add(verticalStackPanel.Name);

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

			_environment.temporaryUIlayer1.Add(saveMenuBorder.Name);
		}

		// Event handler for clicking filled save slot, displays confirmation box
		private void SlotOverwriteOnClick(object sender, RoutedEventArgs e)
		{
			// Gets selected save file info
			Button btn = (Button)sender;
			if (!Int32.TryParse(btn.Name.Substring(btn.Name.Length - 1, 1), out int saveIndex))
			{
				return;
			}

			// Generates UI 
			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = _environment.currentLanguage.UI_saveMenu_overwriteInstructions,
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 10)
			};
			_environment.temporaryUIlayer2.Add(questionTextBlock.Name);

			TextBlock overwriteTextBlock = new TextBlock
			{
				Name = "overwriteTextBlock",
				Text = _environment.currentLanguage.UI_saveMenu_overwrite,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer2.Add(overwriteTextBlock.Name);

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = _environment.currentLanguage.UI_cancel,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer2.Add(cancelTextBlock.Name);

			Button overwriteButton = new Button
			{
				Name = "overwrite_save_0" + saveIndex,
				IsDefault = true,
				Content = overwriteTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			overwriteButton.Click += SlotOnClick;
			_environment.temporaryUIlayer2.Add(overwriteButton.Name);

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			cancelButton.Click += SecondLayerCancelButtonOnClick;
			_environment.temporaryUIlayer2.Add(cancelButton.Name);

			Grid buttonGrid = new Grid
			{
				Name = "buttonGrid",
				Margin = new Thickness(10, 10, 10, 10),
				MinWidth = 400
			};
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			buttonGrid.Children.Add(overwriteButton);
			buttonGrid.Children.Add(cancelButton);
			Grid.SetColumn(overwriteButton, 0);
			Grid.SetColumn(cancelButton, 1);
			_environment.temporaryUIlayer2.Add(buttonGrid.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
				MinHeight = 150
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(buttonGrid);
			_environment.temporaryUIlayer2.Add(verticalStackPanel.Name);

			Border exitMenuBorder = new Border
			{
				Name = "exitMenuBorder",
				BorderThickness = new Thickness(5, 5, 5, 5),
				BorderBrush = new SolidColorBrush(Colors.White),
				Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
				Child = verticalStackPanel
			};
			ViewportContainer.Children.Add(exitMenuBorder);
			Canvas.SetLeft(exitMenuBorder, (Settings.windowWidth / 2) - 200);
			Canvas.SetTop(exitMenuBorder, (Settings.windowHeight / 2) - 75);
			Panel.SetZIndex(exitMenuBorder, 7);

			_environment.temporaryUIlayer2.Add(exitMenuBorder.Name);
		}

		private void SecondLayerCancelButtonOnClick(object sender, RoutedEventArgs e)
		{
			ClearTemporaryUiElements(2);
		}

		// Event handler for clicking empty save slot, saves game
		private void SlotOnClick(object sender, RoutedEventArgs e)
		{
			Button btn = (Button) sender;
			if (Int32.TryParse(btn.Name.Substring(btn.Name.Length - 1, 1), out int saveIndex))
			{
				SaveGame(saveIndex);
			}
			ClearTemporaryUiElements(1);
			ClearTemporaryUiElements(2);
			Settings.inGame = true;
		}

		/*
		 * Function triggers writing of current game status into XML file
		 */
		public void SaveGame(int saveFileIndex)
		{
			if (saveFileIndex < 0 || saveFileIndex > 9)
			{
				return;
			}
			Savegame savegame = new Savegame(_environment, _assets.variables, currentScriptIndex, _scripts[currentScriptIndex].currentLine - 1);
			string saveGameLocation = ".\\saves\\save_" + saveFileIndex.ToString("D2");
			XmlSerializer serializer = new XmlSerializer(savegame.GetType());
			using (StreamWriter writer = new StreamWriter(saveGameLocation))
			{
				serializer.Serialize(writer, savegame);
				writer.Close();
			}
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
			_environment.temporaryUIlayer1.Add(darkOverlay.Name);

			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = _environment.currentLanguage.UI_exitGame_instructions,
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 10)
			};
			_environment.temporaryUIlayer1.Add(questionTextBlock.Name);

			TextBlock exitTextBlock = new TextBlock
			{
				Name = "exitTextBlock",
				Text = _environment.currentLanguage.UI_exitGame_exit,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer1.Add(exitTextBlock.Name);

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = _environment.currentLanguage.UI_cancel,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer1.Add(cancelTextBlock.Name);

			Button exitButton = new Button
			{
				Name = "exitButton",
				IsDefault = true,
				Content = exitTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			exitButton.Click += SecondExitButtonOnClick;
			_environment.temporaryUIlayer1.Add(exitButton.Name);

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			cancelButton.Click += CancelButtonOnClick;
			_environment.temporaryUIlayer1.Add(cancelButton.Name);

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
			_environment.temporaryUIlayer1.Add(buttonGrid.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
				MinHeight = 150
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(buttonGrid);
			_environment.temporaryUIlayer1.Add(verticalStackPanel.Name);

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

			_environment.temporaryUIlayer1.Add(exitMenuBorder.Name);
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
			_environment.temporaryUIlayer1.Add(darkOverlay.Name);

			TextBlock questionTextBlock = new TextBlock
			{
				Name = "questionTextBlock",
				Text = _environment.currentLanguage.UI_returnToMenu_instructions,
				TextAlignment = TextAlignment.Center,
				FontSize = 21,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				Margin = new Thickness(10, 10, 10, 10)
			};
			_environment.temporaryUIlayer1.Add(questionTextBlock.Name);

			TextBlock menuTextBlock = new TextBlock
			{
				Name = "menuTextBlock",
				Text = _environment.currentLanguage.UI_returnToMenu_return,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer1.Add(menuTextBlock.Name);

			TextBlock cancelTextBlock = new TextBlock
			{
				Name = "cancelTextBlock",
				Text = _environment.currentLanguage.UI_cancel,
				FontSize = 21,
				FontWeight = FontWeights.Bold
			};
			_environment.temporaryUIlayer1.Add(cancelTextBlock.Name);

			Button menuButton = new Button
			{
				Name = "menuButton",
				IsDefault = true,
				Content = menuTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			menuButton.Click += SecondMenuButtonOnClick;
			_environment.temporaryUIlayer1.Add(menuButton.Name);

			Button cancelButton = new Button
			{
				Name = "cancelButton",
				IsCancel = true,
				Content = cancelTextBlock,
				Margin = new Thickness(5, 5, 5, 5),
				Padding = new Thickness(5, 5, 5, 5)
			};
			cancelButton.Click += CancelButtonOnClick;
			_environment.temporaryUIlayer1.Add(cancelButton.Name);

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
			_environment.temporaryUIlayer1.Add(buttonGrid.Name);

			StackPanel verticalStackPanel = new StackPanel
			{
				Name = "verticalStackPanel",
				Orientation = Orientation.Vertical,
				MinHeight = 150
			};
			verticalStackPanel.Children.Add(questionTextBlock);
			verticalStackPanel.Children.Add(buttonGrid);
			_environment.temporaryUIlayer1.Add(verticalStackPanel.Name);

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

			_environment.temporaryUIlayer1.Add(exitMenuBorder.Name);
		}
		private void SecondMenuButtonOnClick(object sender, RoutedEventArgs e)
		{
			EndGame();
		}

		private void CancelButtonOnClick(object sender, RoutedEventArgs e)
		{
			ClearTemporaryUiElements(1);
			Settings.inGame = true;
		}
	}
}
