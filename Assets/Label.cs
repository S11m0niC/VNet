using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	public class Label : Asset
	{
		public int scriptIndex;
		public int lineNumber;

		public Label(string name, int scriptIndex, int lineNumber)
		{
			this.name = name;
			this.scriptIndex = scriptIndex;
			this.lineNumber = lineNumber;
		}
	}
}
