using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VNet.Assets
{
	public class Assets
	{
		public List<Label> labels;
		public List<Background> backgrounds;
		public List<Character> characters;
		public List<Sound> sounds;
		public List<Sound> music;
		public List<Choice> choices;
		public List<Variable> variables;

		public Assets()
		{
			labels = new List<Label>();
			backgrounds = new List<Background>();
			characters = new List<Character>();
			sounds = new List<Sound>();
			music = new List<Sound>();
			choices = new List<Choice>();
			variables = new List<Variable>();
		}

		public void CreateCharacter(string name, string moodName, string imagePath)
		{
			try
			{
				var character = new Character(name, moodName, ConvertToAbsolutePath(imagePath));
				characters.Add(character);
			}
			catch (Exception e)
			{
				MessageBox.Show("Error when creating character " + name + "!\n\n" + e.Message);
			}
		}

		public void CreateCharacter(string name, string imagePath)
		{
			try
			{
				var character = new Character(name, ConvertToAbsolutePath(imagePath));
				characters.Add(character);
			}
			catch (Exception e)
			{
				MessageBox.Show("Error when creating character " + name + "!\n\n" + e.Message);
			}
		}

		public void CreateCharacter(string name)
		{
			var character = new Character(name);
			characters.Add(character);
		}

		public void AddImageToCharacter(string charName, string moodName, string imagePath)
		{
			try
			{
				var selectedCharacter = characters.Find(i => i.name == charName);
				selectedCharacter?.AddMoodImage(moodName, ConvertToAbsolutePath(imagePath));

			}
			catch (Exception e)
			{
				MessageBox.Show("Error when adding image to character " + charName + "!\n\n" + e.Message);
			}
		}

		public void SetCharacterColor(string charName, string r, string g, string b)
		{
			var selectedCharacter = characters.Find(i => i.name == charName);
			if (selectedCharacter != null)
			{
				int.TryParse(r, out int red);
				int.TryParse(g, out int green);
				int.TryParse(b, out int blue);
				byte redByte = (byte)red;
				byte greenByte = (byte)green;
				byte blueByte = (byte)blue;
				selectedCharacter.color = Color.FromRgb(redByte, greenByte, blueByte);
			}
		}

		public void CreateLabel(string name, string scriptIndex, string line)
		{
			if (int.TryParse(line, out int ln))
			{
				if (int.TryParse(scriptIndex, out int ind))
				{
					Label lab = labels.Find(i => i.name == name);
					if (lab != null)
					{
						lab.lineNumber = ln;
						return;
					}
					int lineNum = ln;
					var label = new Label(name, ind, lineNum);
					labels.Add(label);
				}
			}
		}

		public void CreateChoice(string name)
		{
			Choice ch = new Choice(name);
			if (choices.Find(i => i.name == name) == null)
			{
				choices.Add(ch);
			}
		}

		public void EditChoiceText(string name, string text)
		{
			Choice ch = choices.Find(i => i.name == name);
			ch.text = text;
		}

		public void AddOptionToChoice(string name, string optionText, string optionLabel)
		{
			Choice ch = choices.Find(i => i.name == name);
			ch.options.Add(new Option(optionText, optionLabel));
		}

		public bool CreateBackground(string name, string imagePath, bool triggeredByScript)
		{
			try
			{
				var background = new Background(name, new Uri(ConvertToAbsolutePath(imagePath), UriKind.Absolute));
				backgrounds.Add(background);
				return true;
			}
			catch (Exception e)
			{
				if (triggeredByScript)
				{
					MessageBox.Show("Error when creating background!\n\n" + e.Message);
				}
				return false;
			}
		}

		public void CreateSound(string name, string path, bool song)
		{
			try
			{
				Sound snd = new Sound(name, ConvertToAbsolutePath(path));
				if (song)
				{
					music.Add(snd);
				}
				else
				{
					sounds.Add(snd);
				}
				
			}
			catch(Exception e)
			{
				MessageBox.Show("Error when creating sound " + name + "!\n\n" + e.Message);
			}
			
		}

		public void CreateVariable(string name, int value)
		{
			variables.Add(new Integer(name, value));
		}

		public void CreateVariable(string name, bool value)
		{
			variables.Add(new Boolean(name, value));
		}

		public string ConvertToAbsolutePath(string relativePath)
		{
			return System.IO.Path.GetFullPath(relativePath);
		}

		public void IntegerAdd(string name, int value)
		{
			Variable var = variables.Find(i => i.name == name);
			if (var is Integer integer)
			{
				integer.value += value;
			}
		}

		public void IntegerSubtract(string name, int value)
		{
			Variable var = variables.Find(i => i.name == name);
			if (var is Integer integer)
			{
				integer.value -= value;
			}
		}

		public void IntegerSet(string name, int value)
		{
			Variable var = variables.Find(i => i.name == name);
			if (var is Integer integer)
			{
				integer.value = value;
			}
		}

		public void BooleanSet(string name, bool value)
		{
			Variable var = variables.Find(i => i.name == name);
			if (var is Boolean boolean)
			{
				boolean.value = value;
			}
		}
	}

	public abstract class Asset
	{
		public string name;
	}
}
