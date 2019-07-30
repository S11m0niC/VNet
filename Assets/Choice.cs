using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	public class Choice
	{
		public string name;
		public string text;

		public List<Option> options;

		public Choice(string name)
		{
			this.name = name;
			options = new List<Option>();
		}
	}

	public class Option
	{
		public string text;
		public string destinationLabel;

		public Option(string text, string destLabel)
		{
			this.text = text;
			this.destinationLabel = destLabel;
		}
	}
}
