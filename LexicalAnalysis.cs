using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNet
{
	class LexicalAnalysis
	{
		private readonly int[,] _automata = new int[8, 256];
		private Script _source;

		private const int _StartState = 0;
		private int _currentState;
		private string _lexem;

		public LexicalAnalysis(Script src)
		{
			_source = src;
			SetAutomata();
		}

		/*
		 * Sets up the state machine rules and states.
		 */
		private void SetAutomata()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					_automata[i, j] = -1;
				}
			}

			// for numbers [0-9]
			for (int i = 48; i < 58; i++)
			{
				_automata[0, i] = 2;
				_automata[1, i] = 1;
				_automata[2, i] = 2;
			}

			// for letters [a-z], [A-Z], [_]
			for (int i = 65; i < 91; i++)
			{
				_automata[0, i] = 1;
				_automata[1, i] = 1;
			}
			for (int i = 97; i < 123; i++)
			{
				_automata[0, i] = 1;
				_automata[1, i] = 1;
			}
			_automata[0, 95] = 1;
			_automata[1, 95] = 1;

			// for punctuation [(, ), ", ., =, \, !, ?]
			_automata[0, 40] = 3;
			_automata[0, 41] = 3;
			_automata[0, 34] = 3;
			_automata[0, 39] = 3;
			_automata[0, 46] = 3;
			_automata[0, 61] = 3;
			_automata[0, 92] = 3;
			_automata[0, 33] = 3;
			_automata[0, 63] = 3;

			// for comments [//]
			_automata[0, 47] = 4;
			_automata[4, 47] = 5;

			// for whitespace [" ", \t]
			_automata[0, 9] = 6;
			_automata[0, 32] = 6;
			_automata[6, 9] = 6;
			_automata[6, 32] = 6;
		}

		/*
		 * Returns the next state in the state machine.
		 * If there are no remaining bytes in the input, returns -1.
		 */
		private int NextState
		{
			get
			{
				if (_source.currentPositionInLine >= _source.lineLengths[_source.currentLine]) return -1;
				char currentChar = _source.lines[_source.currentLine][_source.currentPositionInLine];
				return _automata[_currentState, currentChar];
			}
		}

		/*
		 * Checks if the state machine is at an end state.
		 */
		private bool AtEndState()
		{
			if (_currentState == 1 || _currentState == 2 || _currentState == 3 || _currentState == 5 ||
				_currentState == 6 || _currentState == 7) return true;
			return false;
		}

		/*
		 * Sets the token type according to the current state of the state machine.
		 */
		private Type FindTokenType()
		{
			switch (_currentState)
			{
				case 1:
					return Type.Word;
				case 2:
					return Type.Number;
				case 3:
					return Type.Punctuation;
				case 5:
					return Type.Comment;
				case 6:
					return Type.Whitespace;
				case 7:
					return Type.NewLine;
				default:
					return Type.LexError;
			}
		}

		/*
		 * Gets the next token from the source text.
		 */
		public Token GetNextToken()
		{
			// If at the end of the script
			if (_source.currentLine >= _source.lineCount)
			{
				return new Token("", Type.Eof, new Location(_source.currentLine, _source.currentPositionInLine));
			}

			_currentState = _StartState;
			_lexem = "";

			// If at the start of line and new line token was not yet returned
			if (_source.currentPositionInLine == -1)
			{
				_source.currentPositionInLine = 0;
				return new Token("", Type.NewLine, new Location(_source.currentLine, _source.currentPositionInLine));
			}

			// If at the start of line
			if (_source.currentPositionInLine == 0)
			{
				_source.lineLengths[_source.currentLine] = _source.lines[_source.currentLine].Length;
				// Skips empty lines
				while (_source.lineLengths[_source.currentLine] == 0)
				{
					_source.currentLine++;
					if (_source.currentLine >= _source.lineCount)
					{
						return new Token("", Type.Eof, new Location(_source.currentLine, _source.currentPositionInLine));
					}
					_source.lineLengths[_source.currentLine] = _source.lines[_source.currentLine].Length;
				}
			}

			while (true)
			{
				// If next character fits current state
				if (NextState != -1)
				{
					_currentState = NextState;
					_lexem += _source.lines[_source.currentLine][_source.currentPositionInLine];

					// More characters to process
					if (_source.currentPositionInLine < _source.lineLengths[_source.currentLine] - 1)
					{
						_source.currentPositionInLine++;
					}
					// No more characters to process, at an end state - return appropriate token
					else if (AtEndState())
					{
						int tokenPosInLine = _source.currentPositionInLine - _lexem.Length;
						int tokenLine = _source.currentLine;
						_source.currentPositionInLine = -1;
						_source.currentLine++;
						return ProcessToken(new Token(_lexem, FindTokenType(), new Location(tokenLine, tokenPosInLine)));
					}
					// No more characters to process, not at an end state - return error
					else
					{
						int errorPosInLine = _source.currentPositionInLine - _lexem.Length;
						int errorLine = _source.currentLine;
						_source.currentPositionInLine = -1;
						_source.currentLine++;
						return new Token("", Type.LexError, new Location(errorLine, errorPosInLine));
					}
				}
				// If next character doesn't fit
				else
				{
					// At an end state, return appropriate token
					if (AtEndState())
					{
						return ProcessToken(new Token(_lexem, FindTokenType(), new Location(_source.currentLine, _source.currentPositionInLine)));
					}
					// Not at an end state - return error
					return new Token(_lexem, Type.LexError, new Location(_source.currentLine, _source.currentPositionInLine));
				}
			}
		}

		/*
		 * Processes the token before passing on (defines subtype, removes unnecessary lexems etc.)
		 */
		private Token ProcessToken(Token token)
		{
			switch (token.Type)
			{
				case Type.Word:
					if (Settings.KeywordList.Contains(token.Lexem))
					{
						token.Type = Type.Keyword;
					}
					break;
				case Type.Punctuation:
					if (token.Lexem == "\"")
					{
						token.Type = Type.Quote;
						token.Lexem = "";
					}
					else if (token.Lexem == "\\")
					{

					}
					break;
			}
			return token;
		}
	}

	public enum Type
	{
		Word,
		Keyword,
		Number,
		Punctuation,
		Quote,
		Comment,
		Eof,
		Whitespace,
		NewLine,
		LexError
	}

	public class Token
	{
		public string Lexem { get; set; }
		public Type Type { get; set; }
		public Location Location { get; set; }

		public Token() { }

		public Token(string srcLexem, Type srcType, Location srcLocation)
		{
			Lexem = srcLexem;
			Type = srcType;
			Location = srcLocation;
		}
	}

	public class Location
	{
		public int Line { get; }
		public int Column { get; }

		public Location() { }
		public Location(int srcLine, int srcColumn)
		{
			Line = srcLine;
			Column = srcColumn;
		}

		public override string ToString()
		{
			return "(" + Line + ", " + Column + ")";
		}
	}
}