using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VNet.Assets
{
	public class Character : Asset
	{
		public Color color;
		public string abbreviation;
		public List<Mood> moods;

		public double heightCoefficient;
		public int horizontalOffset;
		public int verticalOffset;

		public Character(string name)
		{
			this.name = name;
			color = Colors.White;
			moods = new List<Mood>();
			horizontalOffset = 0;
			verticalOffset = 0;
		}

		public Character(string name, string imagePath)
		{
			this.name = name;
			color = Colors.White;
			moods = new List<Mood> {new Mood("default", imagePath)};
			horizontalOffset = 0;
			verticalOffset = 0;
		}

		public Character(string name, string moodName, string imagePath)
		{
			this.name = name;
			color = Colors.White;
			moods = new List<Mood> {new Mood(moodName, imagePath)};
			horizontalOffset = 0;
			verticalOffset = 0;
		}

		public void AddMoodImage(string moodName, string imagePath)
		{
			moods.Add(new Mood(moodName, imagePath));
		}
	}

	public class Mood
	{
		public string name;
		public Uri imageUri;
		public double heightInPixels;
		public double widthInPixels;

		public Mood(string name, string imagePath)
		{
			this.name = name;
			this.imageUri = new Uri(imagePath, UriKind.Absolute);
			BitmapImage image = new BitmapImage(imageUri);
			this.heightInPixels = image.PixelHeight;
			this.widthInPixels = image.PixelWidth;
		}
	}
}
