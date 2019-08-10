using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet
{
	public class UILanguage
	{
		public string initial;
		public string UI_mainMenu_continue;
		public string UI_mainMenu_newGame;
		public string UI_mainMenu_loadGame;
		public string UI_mainMenu_options;
		public string UI_mainMenu_exit;

		public string UI_loadMenu_delete;
		public string UI_loadMenu_load;
		public string UI_loadMenu_loadInstructions;
		public string UI_loadMenu_deleteInstructions;

		public string UI_saveMenu_instructions;
		public string UI_saveMenu_save;
		public string UI_saveMenu_overwriteInstructions;
		public string UI_saveMenu_overwrite;

		public string UI_returnToMenu_instructions;
		public string UI_returnToMenu_return;

		public string UI_exitGame_instructions;
		public string UI_exitGame_exit;

		public string UI_emptySlot;
		public string UI_confirm;
		public string UI_back;
		public string UI_cancel;

		public string UI_options_textSpeed;
		public string UI_options_soundVolume;
		public string UI_options_musicVolume;
		public string UI_options_colorNames;
		public string UI_options_colorBorders;

		public UILanguage() { }

		public static UILanguage createLanguage(string languageCode)
		{
			UILanguage lang = new UILanguage();
			switch (languageCode)
			{
				case "EN":
					lang.initial = "EN";
					lang.UI_mainMenu_continue = "Continue";
					lang.UI_mainMenu_newGame = "New Game";
					lang.UI_mainMenu_loadGame = "Load Game";
					lang.UI_mainMenu_options = "Options";
					lang.UI_mainMenu_exit = "Exit";
					lang.UI_loadMenu_delete = "Delete";
					lang.UI_loadMenu_load = "Load";
					lang.UI_loadMenu_loadInstructions = "Select slot to load:";
					lang.UI_loadMenu_deleteInstructions = "Select slot to delete:";
					lang.UI_saveMenu_instructions = "Select slot to save:";
					lang.UI_saveMenu_save = "Save";
					lang.UI_saveMenu_overwriteInstructions = "Overwrite save slot?";
					lang.UI_saveMenu_overwrite = "Overwrite";
					lang.UI_returnToMenu_instructions = "Return to main menu?\n(Unsaved progress will be lost)";
					lang.UI_returnToMenu_return = "Return";
					lang.UI_exitGame_instructions = "Exit game?\n(Unsaved progress will be lost)";
					lang.UI_exitGame_exit = "Exit";
					lang.UI_emptySlot = "*Empty slot*";
					lang.UI_confirm = "Confirm";
					lang.UI_back = "Back";
					lang.UI_cancel = "Cancel";
					lang.UI_options_textSpeed = "Text speed";
					lang.UI_options_soundVolume = "Sound volume";
					lang.UI_options_musicVolume = "Music volume";
					lang.UI_options_colorNames = "Color character names";
					lang.UI_options_colorBorders = "Color border lines";
					break;

				case "SI":
					lang.initial = "SI";
					lang.UI_mainMenu_continue = "Nadaljuj";
					lang.UI_mainMenu_newGame = "Nova igra";
					lang.UI_mainMenu_loadGame = "Naloži igro";
					lang.UI_mainMenu_options = "Nastavitve";
					lang.UI_mainMenu_exit = "Izhod";
					lang.UI_loadMenu_delete = "Izbriši";
					lang.UI_loadMenu_load = "Naloži";
					lang.UI_loadMenu_loadInstructions = "Izberi prostor za nalaganje:";
					lang.UI_loadMenu_deleteInstructions = "Izberi prostor za izbris:";
					lang.UI_saveMenu_instructions = "Izberi prostor za shranjevanje:";
					lang.UI_saveMenu_save = "Shrani";
					lang.UI_saveMenu_overwriteInstructions = "Prepiši shranjeno igro?";
					lang.UI_saveMenu_overwrite = "Prepiši";
					lang.UI_returnToMenu_instructions = "Vrnitev na glavni meni?\n(Neshranjen napredek bo izgubljen)";
					lang.UI_returnToMenu_return = "Vrni";
					lang.UI_exitGame_instructions = "Izhod iz igre?\n(Neshranjen napredek bo izgubljen)";
					lang.UI_exitGame_exit = "Izhod";
					lang.UI_emptySlot = "*Prazno*";
					lang.UI_confirm = "Potrdi";
					lang.UI_back = "Nazaj";
					lang.UI_cancel = "Prekliči";
					lang.UI_options_textSpeed = "Hitrost besedila";
					lang.UI_options_soundVolume = "Glasnost zvokov";
					lang.UI_options_musicVolume = "Glasnost glasbe";
					lang.UI_options_colorNames = "Pobarvaj imena likov";
					lang.UI_options_colorBorders = "Pobarvaj okvir besedila";
					break;
			}

			return lang;
		}
	}
}
