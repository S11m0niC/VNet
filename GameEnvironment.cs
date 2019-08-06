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
using System.Xml.Serialization;
using VNet.Assets;

namespace VNet
{
	public class GameEnvironment
	{
		
		public string currentBackgroundName;
		public string leftCharacterName;
		public string centerCharacterName;
		public string rightCharacterName;

		public string fullText;
		[XmlIgnore]
		public string displayedText;

		[XmlIgnore]
		public int FullTextLength => fullText.Length;
		[XmlIgnore]
		public int DisplayedTextLength => displayedText.Length;

		public string currentSongName;
		public bool currentSongRepeating;
		public string currentSoundName;
		public bool currentSoundRepeating;

		[XmlIgnore]
		public List<string> onscreenButtonNames;
		[XmlIgnore]
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
