using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VNet
{
	public class Script
	{
		public string sourcePath;

		public int lineCount;
		public string[] lines;
		public int[] lineLengths;

		public int currentLine;
		public int currentPositionInLine;

		public Script(string srcPath)
		{
			sourcePath = srcPath;
			lines = File.ReadAllLines(sourcePath);

			lineCount = lines.Length;
			lineLengths = new int[lineCount];
			for (int i = 0; i < lineCount; i++)
			{
				lineLengths[i] = lines[i].Length;
			}

			currentLine = 0;
			currentPositionInLine = 0;
		}
	}
}
