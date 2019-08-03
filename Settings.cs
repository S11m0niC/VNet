using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet
{
	public static class Settings
	{
		/*
		 * Read-Only global variables and settings
		 */
		public static readonly string ScriptExtension = "vnets";
		public static readonly string StartScriptUri = "./game." + ScriptExtension;
		
		public static readonly int SplashScreenMinimalTimeInMiliseconds = 3000;
		
		public static readonly List<string> GameKeywordList = new List<string> {"jump", "show", "clear", "play", "stop", "with", "execute", "int", "bool", "if", "add", "subtract", "set" };
		public static readonly List<string> SetupKeywordList = new List<string> {"label", "character", "image", "color", "sound", "music", "choice"};

		/*
		 * Global variables which change at runtime
		 */
		public static int windowWidth = 1280;
		public static int windowHeight = 720;
		public static bool textDisplayedFully;
		public static bool executeNext;
		public static bool inGame;
		public static bool allowProgress;

		/*
		 * Settings adjusted in the settings screen
		 */
		public static int TextDisplaySpeedInMiliseconds = 20;
		public static double SoundVolumeMultiplier = 1.0;
		public static double MusicVolumeMultiplier = 1.0;
	}
}
