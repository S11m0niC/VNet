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
		public BitmapImage Image
		{
			get
			{
				return new BitmapImage(imageUri);
			}
		}
		public Uri imageUri;

		public Background(string name, Uri uri)
		{
			this.name = name;
			this.imageUri = uri;
		}
	}
}
