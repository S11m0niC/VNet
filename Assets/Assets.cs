using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VNet.Assets
{
	public class Assets
	{
		public List<Label> labels;
		public List<Background> backgrounds;
		public List<Character> characters;
		public List<Sound> sounds;

		public Assets()
		{
			labels = new List<Label>();
			backgrounds = new List<Background>();
			characters = new List<Character>();
			sounds = new List<Sound>();
		}

		public void CreateCharacter(string name, string moodName, string imagePath)
		{
			var character = new Character(name, moodName, imagePath);
			characters.Add(character);
		}

		public void CreateCharacter(string name, string imagePath)
		{
			var character = new Character(name, imagePath);
			characters.Add(character);
		}

		public void CreateCharacter(string name)
		{
			var character = new Character(name);
			characters.Add(character);
		}

		public void CreateLabel(string name, string line)
		{
			int lineNum = Int32.Parse(line);
			var label = new Label(name, lineNum);
			labels.Add(label);
		}

		public void CreateBackground(string name, string imagePath)
		{
			var image = new BitmapImage(new Uri(imagePath, UriKind.Relative));
			var background = new Background(name, image);
			backgrounds.Add(background);
		}

		public void AddImageToCharacter(string charName, string moodName, string imagePath)
		{
			var selectedCharacter = characters.Find(i => i.name == charName);
			selectedCharacter?.AddMoodImage(moodName, imagePath);
		}

		public void SetBackgroundToShowing(string name)
		{
			foreach (var bg in this.backgrounds)
			{
				if (bg.name == name)
					bg.onScreen = true;
				else
					bg.onScreen = false;
			}
		}
	}
}
