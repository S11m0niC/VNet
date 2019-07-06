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
		public static readonly List<string> GameKeywordList = new List<string> {"jump", "show", "play", "with", "choice", "execute" };
		public static readonly List<string> SetupKeywordList = new List<string> {"label", "character", "image"};

		/*
		 * Settings which change at runtime
		 */
		public static bool textDisplayedFully;
		
	}
}
