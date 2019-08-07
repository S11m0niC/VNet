using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

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
		public static readonly string StartScriptUri = "./game." + ScriptExtension;
		// Path and names of save files
		public static string SaveFilePath(int saveFileIndex)
		{
			return ".\\saves\\save_" + saveFileIndex.ToString("D2");
		} 
		// The minimum splash screen time
		public static readonly int SplashScreenMinimalTimeInMiliseconds = 3000;
		// List of keywords considered to be "game" words. These are executed during normal gameplay
		public static readonly List<string> GameKeywordList = new List<string> {"jump", "show", "clear", "play", "stop", "execute", "int", "bool", "if", "add", "subtract", "set" };
		// List of keywords considered to be "setup" words. These are executed when launching the game
		public static readonly List<string> SetupKeywordList = new List<string> {"label", "character", "image", "color", "sound", "music", "choice", "name"};

		/*
		 * Global variables which change at runtime
		 */
		public static double windowWidth = 1280;
		public static double windowHeight = 720;
		public static bool textDisplayedFully;
		public static bool executeNext;
		public static bool inGame;
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

		/*
		 * Settings adjusted in script
		 */
		public static string gameName = "VNet";
	}
}
