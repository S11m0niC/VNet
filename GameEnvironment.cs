using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VNet.Assets;

namespace VNet
{
	public class GameEnvironment
	{
		private readonly Background _defaultBackground;

		private Background _currentBackground;

		private string _displayedText;

		public Background CurrentBackground
		{
			get => _currentBackground ?? _defaultBackground;
			set => _currentBackground = value;
		}
		public Character LeftCharacter { get; set; }

		public Character CenterCharacter { get; set; }

		public Character RightCharacter { get; set; }

		public string DisplayedText
		{
			get => _displayedText ?? "";
			set => _displayedText = value;
		}

		public GameEnvironment()
		{
			_currentBackground = null;
			LeftCharacter = null;
			CenterCharacter = null;
			RightCharacter = null;
			_displayedText = null;

			WriteableBitmap black = BitmapFactory.New(1280, 720);
			black.Clear(Colors.Black);
			
			_defaultBackground = new Background("default_black", black);
		}
	}
}
