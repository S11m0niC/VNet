using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace VNet.Assets
{
	public class Background
	{
		public string name;
		public WriteableBitmap image;
		public bool onScreen = false;

		public Background(string name, WriteableBitmap image)
		{
			this.name = name;
			this.image = image;
		}
	}
}
