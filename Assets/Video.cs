using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	public class Video : Asset
	{
		public string location;

		public Video(string name, string location)
		{
			this.name = name;
			this.location = location;
		}
	}
}
