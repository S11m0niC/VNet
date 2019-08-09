using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VNet.Assets;

namespace VNet
{
	public class Savegame
	{
		public GameEnvironment currentEnvironment;
		public List<Variable> currentVariables;

		public int currentScriptIndex;
		public int currentScriptLine;

		public DateTime currentTime;

		public Savegame() { }

		public Savegame(GameEnvironment environment, List<Variable> variables, int index, int line)
		{
			currentEnvironment = environment;
			currentVariables = variables;
			currentScriptIndex = index;
			currentScriptLine = line;
			currentTime = DateTime.Now;
		}

		public static Savegame DeserializeSaveGame(int saveFileIndex)
		{
			try
			{
				Savegame save;
				string saveGameLocation = Settings.SaveFilePath(saveFileIndex);
				XmlSerializer serializer = new XmlSerializer(typeof(Savegame));
				using (StreamReader reader = new StreamReader(saveGameLocation))
				{
					save = (Savegame)serializer.Deserialize(reader);
					reader.Close();
				}

				return save;
			}
			// On exception (no save, corrupted save...)
			catch (Exception)
			{
				return null;
			}
		}
	}
}
