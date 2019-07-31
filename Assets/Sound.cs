using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	public class Sound : Asset
	{
		public string location;

		public Sound(string name, string location)
		{
			this.name = name;
			this.location = location;
		}
	}
}
