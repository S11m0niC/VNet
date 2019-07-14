using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	class Choice
	{
		public string name;

		public string option1;
		public string option2;
		public string option3;
		public string option4;

		public int scriptLine1;
		public int scriptLine2;
		public int scriptLine3;
		public int scriptLine4;

		public Choice(string name)
		{
			this.name = name;
			option1 = null;
			option2 = null;
			option3 = null;
			option4 = null;
		}

		public void AddOption(string option)
		{
			if (option1 == null)
			{
				option1 = option;
			}
			else if (option2 == null)
			{
				option2 = option;
			}
			else if (option3 == null)
			{
				option3 = option;
			}
			else if (option4 == null)
			{
				option4 = option;
			}
		}
	}
}
