using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VNet.Assets;

namespace VNet
{
	public class GameEnvironment
	{
		
		public string currentBackgroundName;

		public string fullText;
		public string displayedText;

		public int FullTextLength => fullText.Length;
		public int DisplayedTextLength => displayedText.Length;

		public string leftCharacterName;
		public string centerCharacterName;
		public string rightCharacterName;

		public string currentSongName;

		public List<string> onscreenButtonNames;
		public List<string> temporaryUIElementNames;

		public GameEnvironment()
		{
			currentBackgroundName = null;
			leftCharacterName = null;
			centerCharacterName = null;
			rightCharacterName = null;
			onscreenButtonNames = new List<string>();
			temporaryUIElementNames = new List<string>();
		}
	}
}
