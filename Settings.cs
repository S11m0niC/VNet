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
		public static readonly int TextDisplaySpeedInMiliseconds = 20;

		public static readonly List<string> GameKeywordList = new List<string> {"jump", "show", "clear", "play", "with", "choice", "execute" };
		public static readonly List<string> SetupKeywordList = new List<string> {"label", "character", "image"};

		/*
		 * Settings which change at runtime
		 */
		public static int windowWidth = 1280;
		public static int windowHeight = 720;
		public static bool textDisplayedFully;
		public static bool executeNext;
		public static bool inChoice;
	}
}
