using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VNet
{
	public static class Settings
	{
		/*
		 * Read-Only global variables and settings
		 */
		// Extension used for game scripts
		public static readonly string ScriptExtension = "vnets";
		// Location and name of starting script
		public static readonly string StartScriptUri = "./scripts/game." + ScriptExtension;

		// Path and names of save files
		public static string SaveFilePath(int saveFileIndex)
		{
			return ".\\saves\\save_" + saveFileIndex.ToString("D2");
		}
		public static string SettingsFilePath()
		{
			return ".\\saves\\settings";
		}

		// The minimum splash screen time
		public static readonly int SplashScreenMinimalTimeInMiliseconds = 3000;
		// List of keywords considered to be "setup" words. These are executed when launching the game
		public static readonly List<string> SetupKeywordList = new List<string> {"include", "label", "character", "image", "color", "sound", "music", "video", "choice", "font"};
		// List of keywords considered to be "game" words. These are executed during normal gameplay
		public static readonly List<string> GameKeywordList = new List<string> {"jump", "show", "clear", "play", "stop", "ui", "end", "int", "bool", "if", "add", "subtract", "set", "save" };
		// List of keywords considered to be both "setup" and "game". These are executed both at launch and during gameplay
		public static readonly List<string> SetupAndGameKeywordList = new List<string>{"name"};
		// List of supported language initials
		public static readonly List<string> LanguageInitialList = new List<string> {"EN", "SI"};
		/*
		 * Global variables which change at runtime
		 */
		public static double windowWidth = 1280;
		public static double windowHeight = 720;
		public static bool textDisplayedFully;
		public static bool executeNext;
		public static bool inGame;
		public static bool UIvisible;
		public static bool allowProgress;
		public static bool deleteGamesOnLoadScreen;
		public static int deletedSaveSlot;
		public static bool afterLoad;

		/*
		 * Settings adjusted in the settings screen
		 */
		public static int textDisplaySpeedInMiliseconds = 20;
		public static double soundVolumeMultiplier = 1.0;
		public static double musicVolumeMultiplier = 1.0;
		public static bool colorCharacterNames = false;
		public static bool colorTextBorders = true;
		public static string language = "SI";

		/*
		 * Settings adjusted in script
		 */
		public static string gameName = "VNet";
		public static string protagonistName = "";
		public static string fontText = "Arial";
		public static string fontNames = "Arial";

		/*
		 * Loads settings from xml file
		 */
		public static void LoadSettings()
		{
			SettingsSave settingsSave = SettingsSave.DeserializeSettings();
			settingsSave?.SaveToSettings();
		}
	}

	/*
	 * Non-static class 
	 */
	public class SettingsSave
	{
		public int textDisplaySpeedInMiliseconds;
		public double soundVolumeMultiplier;
		public double musicVolumeMultiplier;
		public bool colorCharacterNames;
		public bool colorTextBorders;
		public string language;

		public SettingsSave() { }

		public SettingsSave(bool loadFromSettings)
		{
			if (loadFromSettings)
			{
				textDisplaySpeedInMiliseconds = Settings.textDisplaySpeedInMiliseconds;
				soundVolumeMultiplier = Settings.soundVolumeMultiplier;
				musicVolumeMultiplier = Settings.musicVolumeMultiplier;
				colorCharacterNames = Settings.colorCharacterNames;
				colorTextBorders = Settings.colorTextBorders;
				language = Settings.language;
			}
		}

		public void SaveToSettings()
		{
			Settings.textDisplaySpeedInMiliseconds = textDisplaySpeedInMiliseconds;
			 Settings.soundVolumeMultiplier = soundVolumeMultiplier;
			 Settings.musicVolumeMultiplier = musicVolumeMultiplier;
			 Settings.colorCharacterNames = colorCharacterNames;
			 Settings.colorTextBorders = colorTextBorders;
			 Settings.language = language;
		}

		public static SettingsSave DeserializeSettings()
		{
			try
			{
				SettingsSave save;
				string saveGameLocation = Settings.SettingsFilePath();
				XmlSerializer serializer = new XmlSerializer(typeof(SettingsSave));
				using (StreamReader reader = new StreamReader(saveGameLocation))
				{
					save = (SettingsSave)serializer.Deserialize(reader);
					reader.Close();
				}

				return save;
			}
			// On exception (no save, corrupted save...)
			catch (Exception)
			{
				return null;
			}
		}
	}
}
