using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VNet.Assets
{
	public class Character
	{
		public string name;
		public List<Mood> moods;

		public Character(string name)
		{
			this.name = name;
			moods = new List<Mood>();
		}

		public Character(string name, string imagePath)
		{
			this.name = name;
			moods = new List<Mood> {new Mood("default", imagePath)};
		}

		public Character(string name, string moodName, string imagePath)
		{
			this.name = name;
			moods = new List<Mood> {new Mood(moodName, imagePath)};
		}

		public void AddMoodImage(string moodName, string imagePath)
		{
			moods.Add(new Mood(moodName, imagePath));
		}
	}

	public class Mood
	{
		public string name;
		public WriteableBitmap image;

		public Mood(string name, string imagePath)
		{
			this.name = name;
			this.image = new WriteableBitmap(new BitmapImage(new Uri(imagePath, UriKind.Absolute)));
		}
	}
}
