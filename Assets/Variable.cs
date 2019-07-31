using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet.Assets
{
	public abstract class Variable : Asset
	{}

	public class Boolean : Variable
	{
		public bool value;

		public Boolean(string name, bool value)
		{
			this.name = name;
			this.value = value;
		}
	}

	public class Integer : Variable
	{
		public int value;

		public Integer(string name, int value)
		{
			this.name = name;
			this.value = value;
		}
	}
}
