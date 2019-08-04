using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace VNet.Assets
{
	public class Background : Asset
	{
		public WriteableBitmap image;
		public Uri imageUri;

		public Background(string name, Uri uri)
		{
			this.name = name;
			this.image = null;
			this.imageUri = uri;
		}

		public Background(string name, WriteableBitmap image)
		{
			this.name = name;
			this.image = image;
			this.imageUri = null;
		}
	}
}
