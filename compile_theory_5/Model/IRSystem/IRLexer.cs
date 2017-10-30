using compile_theory_5.Model.IRSystem.IRViewModel;
using compile_theory_5.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem
{
	enum IRTokenKind
	{
		MOV,
		ADD,
		SUB,
		MUL,
		DIV,
		JMP,
		JE,
		JL,
		JG,
		NOP,
		ANNO,
		ERROR,
		COMMA,
		ID,
		NUM,
		EOL,
		HLT,
		COLON
	};

	enum IRErrorKind
	{
		UNNEEDBRA,
		INVALIDCHAR,
		NUMBERERROR,
		NESTBRA,
		ANNONOTCLOSED,
		DEFAULT
	};

	class IRLexer
	{
		private int offset;
		private int startOffset;
		private int length;
		private bool LexMode = false;

		Dictionary<string, IRTokenKind> KeyWords = new Dictionary<string, IRTokenKind> {
			{ "MOV", IRTokenKind.MOV },
			{ "ADD", IRTokenKind.ADD },
			{ "SUB", IRTokenKind.SUB },
			{ "MUL", IRTokenKind.MUL },
			{ "DIV", IRTokenKind.DIV },
			{ "JMP", IRTokenKind.JMP },
			{ "JE", IRTokenKind.JE },
			{ "JL", IRTokenKind.JL },
			{ "JG", IRTokenKind.JG },
			{ "NOP", IRTokenKind.NOP },
			{ "HLT", IRTokenKind.HLT },
			{ "mov", IRTokenKind.MOV },
			{ "add", IRTokenKind.ADD },
			{ "sub", IRTokenKind.SUB },
			{ "mul", IRTokenKind.MUL },
			{ "div", IRTokenKind.DIV },
			{ "jmp", IRTokenKind.JMP },
			{ "je", IRTokenKind.JE },
			{ "jl", IRTokenKind.JL },
			{ "jg", IRTokenKind.JG },
			{ "nop", IRTokenKind.NOP },
			{ "hlt", IRTokenKind.HLT }};

		public IRToken LexNext()
		{
			int state = 0;
			string value = string.Empty;
			int startOffset = GetOffset();
			IRErrorKind errorKind = IRErrorKind.DEFAULT;
			char? cQuestion;
			char c;
			while (true)
			{
				cQuestion = NextChar();
				if (cQuestion.HasValue)
				{
					c = cQuestion.Value;
					switch (state)
					{
						case 0:
							startOffset = GetOffset();
							if (char.IsLetter(c) || c == '_')
							{
								state = 1;
								value += c;
								break;
							}

							if (char.IsDigit(c))
							{
								state = 2;
								value += c;
								break;
							}

							if (char.IsWhiteSpace(c))
							{
								break;
							}

							if (c == ',')
							{
								value += c;
								return new IRToken(IRTokenKind.COMMA, value, startOffset);
							}

							if (c == ':')
							{
								value += c;
								return new IRToken(IRTokenKind.COLON, value, startOffset);
							}

							if (c == ';')
							{
								state = 8;
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = IRErrorKind.NUMBERERROR;
								break;
							}

							//ERROR
							state = 4;
							value += c;
							errorKind = IRErrorKind.INVALIDCHAR;
							break;

						case 1:
							if (char.IsLetterOrDigit(c))
							{
								value += c;
								break;
							}

							putBack();

							if (KeyWords.ContainsKey(value))
							{
								return new IRToken(KeyWords[value], value, startOffset);
							}
							else
							{
								return new IRToken(IRTokenKind.ID, value, startOffset);
							}

						case 2:
							if (char.IsDigit(c))
							{
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 5;
								value += c;
								break;
							}

							putBack();
							return new IRToken(IRTokenKind.NUM, value, startOffset);
						case 4:
							switch (errorKind)
							{
								case IRErrorKind.NUMBERERROR:
									if (char.IsDigit(c) || c == '.')
									{
										value += c;
									}
									else
									{
										putBack();
										return new IRToken(IRTokenKind.ERROR, value, startOffset);
									}
									break;
								case IRErrorKind.INVALIDCHAR:
									if (char.IsLetterOrDigit(c) || c == ',' || char.IsWhiteSpace(c))
									{
										putBack();
										return new IRToken(IRTokenKind.ERROR, value, startOffset);
									}
									else
									{
										value += c;
									}
									break;
								default:
									if (char.IsWhiteSpace(c))
									{
										return new IRToken(IRTokenKind.ERROR, value, startOffset);
									}
									else
									{
										value += c;
									}
									break;
							}
							break;
						case 5:
							//case: "1234.5"
							if (char.IsDigit(c))
							{
								state = 6;
								value += c;
								break;
							}


							//case "1234.."
							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = IRErrorKind.NUMBERERROR;
								break;
							}

							//case: "1234.a"
							putBack();
							return new IRToken(IRTokenKind.ERROR, value, startOffset);

						case 6:
							//case: "1234.56"
							if (char.IsDigit(c))
							{
								state = 6;
								value += c;
								break;
							}

							//case: "1234.5."
							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = IRErrorKind.NUMBERERROR;
								break;
							}

							putBack();
							return new IRToken(IRTokenKind.NUM, value, startOffset);
						case 8:
							if (c == '\r' || c == '\n') //for the case "\r\n" and "\n";
							{
								return new IRToken(IRTokenKind.ANNO, value, startOffset);
							}

							value += c;
							break;

						default:
							return new IRToken(IRTokenKind.ERROR, value, startOffset);
					}
				}
				else
				{
					switch (state)
					{
						case 0:
							return null;
						case 1:
							if (KeyWords.ContainsKey(value))
							{
								return new IRToken(KeyWords[value], value, startOffset);
							}
							else
							{
								return new IRToken(IRTokenKind.ID, value, startOffset);
							}
						case 2:
						case 5:
						case 6:
							return new IRToken(IRTokenKind.NUM, value, startOffset);
						case 8:
							return new IRToken(IRTokenKind.ANNO, value, startOffset);
						default:
							return new IRToken(IRTokenKind.ERROR, value, startOffset);
					}
				}
			}
		}

		private int GetOffset()
		{
			if (LexMode)
			{
				return IRSystem.source.GetOffset();
			}
			else
			{
				return offset;
			}
		}

		public List<IRToken> LexLine(int line)
		{
			if(line <= IRSystem.source.GetLineCount())
			{
				var l = IRSystem.source.GetIRLine(line);
				startOffset = l.Offset;
				offset = l.Offset;
				length = l.Length;
				IRToken t = LexNext();
				List<IRToken> r = new List<IRToken>();
				while (t != null)
				{
					if (t.kind != IRTokenKind.ANNO)
					{
						r.Add(t);
					}
					t = LexNext();
				}
				if (r.Count > 0)
				{
					return r;
				}
				else
				{
					r.Add(new IRToken(IRTokenKind.NOP));
					return r;
				}
			}
			else
			{
				return new List<IRToken> { new IRToken(IRTokenKind.HLT) };
			}
		}

		public void putBack()
		{
			if (LexMode)
			{
				IRSystem.source.putBack();
			}
			else
			{
				offset--;
			}
		}

		public char? NextChar()
		{
			if (LexMode)
			{
				return IRSystem.source.NextChar();
			}
			else
			{
				if (offset < startOffset + length)
				{
					return IRSystem.source.GetChar(offset++);
				}
				else
				{
					return null;
				}
			}
		}

		public void Highlighting()
		{
			LexMode = true;
			var t = LexNext();
			while (t != null)
			{
				switch (t.kind)
				{
					case IRTokenKind.MOV:
					case IRTokenKind.ADD:
					case IRTokenKind.SUB:
					case IRTokenKind.MUL:
					case IRTokenKind.DIV:
					case IRTokenKind.JMP:
					case IRTokenKind.JE:
					case IRTokenKind.JL:
					case IRTokenKind.JG:
					case IRTokenKind.HLT:
						IRSystem.source.Colorize(t.offset, t.value.Length, HLKind.KEYWORD);
						break;
					case IRTokenKind.COMMA:
						IRSystem.source.Colorize(t.offset, t.value.Length, HLKind.PUNCT);
						break;
					case IRTokenKind.ID:
						IRSystem.source.Colorize(t.offset, t.value.Length, HLKind.ID);
						break;
					case IRTokenKind.NUM:
						IRSystem.source.Colorize(t.offset, t.value.Length, HLKind.NUM);
						break;
					case IRTokenKind.ANNO:
						IRSystem.source.Colorize(t.offset, t.value.Length, HLKind.ANNO);
						break;
					case IRTokenKind.ERROR:
						IRSystem.source.Colorize(t.offset, t.value.Length, HLKind.ERROR);
						break;
				}
				t = LexNext();
			}
			LexMode = false;
		}

		public void Test() //测试词法输出流正确
		{
			//ProcessViewModel.Clear();
			var t = LexNext();
			while (t != null)
			{
				//ProcessViewModel.Add(new Process(t.kind.ToString(), t.value));
				t = LexNext();
			}
		}
	}
}
