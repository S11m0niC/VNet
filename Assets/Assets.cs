using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public List<Choice> choices;

		public Assets()
		{
			labels = new List<Label>();
			backgrounds = new List<Background>();
			characters = new List<Character>();
			sounds = new List<Sound>();
			choices = new List<Choice>();
		}

		public void CreateCharacter(string name, string moodName, string imagePath)
		{
			var character = new Character(name, moodName, ConvertToAbsolutePath(imagePath));
			characters.Add(character);
		}

		public void CreateCharacter(string name, string imagePath)
		{
			var character = new Character(name, ConvertToAbsolutePath(imagePath));
			characters.Add(character);
		}

		public void CreateCharacter(string name)
		{
			var character = new Character(name);
			characters.Add(character);
		}

		public void CreateLabel(string name, string line)
		{
			Label lab = labels.Find(i => i.name == name);
			if (lab != null)
			{
				lab.lineNumber = Int32.Parse(line);
				return;
			} 
			int lineNum = Int32.Parse(line);
			var label = new Label(name, lineNum);
			labels.Add(label);
		}

		public void CreateChoice(string name)
		{
			Choice ch = new Choice(name);
			if (choices.Find(i => i.name == name) == null)
			{
				choices.Add(ch);
			}
		}

		public void CreateBackground(string name, string imagePath)
		{
			var image = new BitmapImage(new Uri(ConvertToAbsolutePath(imagePath), UriKind.Absolute));
			var background = new Background(name, new WriteableBitmap(image));
			backgrounds.Add(background);
		}

		public void AddImageToCharacter(string charName, string moodName, string imagePath)
		{
			var selectedCharacter = characters.Find(i => i.name == charName);
			selectedCharacter?.AddMoodImage(moodName, ConvertToAbsolutePath(imagePath));
		}

		public void AddOptionToChoice(string name, string option)
		{
			var selectedChoice = choices.Find(i => i.name == name);
			selectedChoice?.AddOption(option);
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

		public string ConvertToAbsolutePath(string relativePath)
		{
			return System.IO.Path.GetFullPath(relativePath);
		}
	}
}
