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
		public string leftCharacterMood;

		public string centerCharacterName;
		public string centerCharacterMood;

		public string rightCharacterName;
		public string rightCharacterMood;

		public string fullText;
		public string nameOfCharacterTalking;

		[XmlIgnore]
		public string displayedText;

		[XmlIgnore]
		public int FullTextLength => fullText.Length;
		[XmlIgnore]
		public int DisplayedTextLength => displayedText.Length;

		public string currentSongName;
		public bool currentSongRepeating;
		public double currentSongVolume;

		public string currentSoundName;
		public bool currentSoundRepeating;
		public double currentSoundVolume;

		[XmlIgnore]
		public UILanguage currentLanguage;

		[XmlIgnore]
		public List<string> choiceButtonNames;
		[XmlIgnore]
		public List<string> temporaryUIlayer1;
		[XmlIgnore]
		public List<string> temporaryUIlayer2;

		public GameEnvironment()
		{
			currentBackgroundName = null;

			leftCharacterName = null;
			leftCharacterMood = null;
			centerCharacterName = null;
			centerCharacterMood = null;
			rightCharacterName = null;
			rightCharacterMood = null;

			nameOfCharacterTalking = null;
			fullText = null;

			currentSongName = null;
			currentSongRepeating = false;
			currentSongVolume = 0.0;
			currentSoundName = null;
			currentSoundRepeating = false;
			currentSoundVolume = 0.0;

			choiceButtonNames = new List<string>();
			temporaryUIlayer1 = new List<string>();
			temporaryUIlayer2 = new List<string>();
		}
	}
}
