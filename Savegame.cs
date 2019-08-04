using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using VNet.Assets;

namespace VNet
{
	public class Savegame
	{
		public GameEnvironment currentEnvironment;
		public List<Variable> currentVariables;
		public int currentScriptLine;

		public Savegame() { }

		public Savegame(GameEnvironment environment, List<Variable> variables, int line)
		{
			currentEnvironment = environment;
			currentVariables = variables;
			currentScriptLine = line;
		}
	}
}
