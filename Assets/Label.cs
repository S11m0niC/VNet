using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	public class Label
	{
		public string name;
		public int lineNumber;

		public Label(string name, int lineNum)
		{
			this.name = name;
			this.lineNumber = lineNum;
		}
	}
}
