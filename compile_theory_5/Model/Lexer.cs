using compile_theory_5.Model;
using compile_theory_5.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model
{
	enum TokenKind
	{
		IF,
		ELSE,
		WHILE,
		DO,
		BREAK,
		GT,
		GEQU,
		ADD,
		SUB,
		MULT,
		DIV,
		EQU,
		LT,
		LEQU,
		LBRA,
		RBRA,
		SEMI,
		ID,
		NUM,
		LPAR,
		RPAR,
		ANNO,
		ERROR,
		EOF
	};

	enum ErrorKind
	{
		UNNEEDBRA,
		INVALIDCHAR,
		NUMBERERROR,
		NESTBRA,
		ANNONOTCLOSED,
		DEFAULT
	};

	class Lexer
	{
		static Dictionary<string, TokenKind> KeyWords = new Dictionary<string, TokenKind> {
			{ "if", TokenKind.IF } ,
			{ "else", TokenKind.ELSE } ,
			{ "while", TokenKind.WHILE } ,
			{ "do", TokenKind.DO } ,
			{ "break", TokenKind.BREAK } };

		static HashSet<char> symbols = new HashSet<char>
		{ '+', '-', '*', '/', '=', '<', '>', '{', '}', ';', '(', ')', '_' };

		static public Token LexNext()
		{
			int state = 0;
			string value = string.Empty;
			int startOffset = 0;
			ErrorKind errorKind = ErrorKind.DEFAULT;
			char? cQuestion;
			char c;
			while (true)
			{
				cQuestion = SourceViewModel.NextChar();
				if (cQuestion.HasValue)
				{
					c = cQuestion.Value;
					switch (state)
					{
						case 0:
							startOffset = SourceViewModel.GetOffset();
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

							if (c == '+')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.ADD); ;
							}

							if (c == '-')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.SUB);
							}

							if (c == '*')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.MULT);
							}

							if (c == '/')
							{
								state = 7;
								value += c;
								break;
							}

							if (c == '=')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.EQU);
							}

							if (c == '<')
							{
								value = c.ToString();
								var f = SourceViewModel.Forward();
								if (f.HasValue && f.Value == '=')
								{
									SourceViewModel.NextChar();
									return new Token(startOffset, value + '=', TokenKind.LEQU);
								}
								else
								{
									return new Token(startOffset, value, TokenKind.LT);
								}
							}

							if (c == '>')
							{
								value = c.ToString();
								var f = SourceViewModel.Forward();
								if (f.HasValue && f.Value == '=')
								{
									SourceViewModel.NextChar();
									return new Token(startOffset, value + '=', TokenKind.GEQU);
								}
								else
								{
									return new Token(startOffset, value, TokenKind.GT);
								}
							}

							if (c == '{')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.LBRA);
							}

							if (c == '}')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.RBRA);
							}

							if (c == '(')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.LPAR);
							}

							if (c == ')')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.RPAR);
							}

							if (c == ';')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.SEMI);
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.NUMBERERROR;
								break;
							}

							//ERROR
							state = 4;
							value += c;
							errorKind = ErrorKind.INVALIDCHAR;
							break;

						case 1:
							if (char.IsLetterOrDigit(c))
							{
								value += c;
								break;
							}

							SourceViewModel.putBack();

							if (KeyWords.ContainsKey(value))
							{
								return new Token(startOffset, value, KeyWords[value]);
							}
							else
							{
								return new Token(startOffset, value, TokenKind.ID);
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

							SourceViewModel.putBack();
							return new Token(startOffset, value, TokenKind.NUM);

						case 3:
							if (c == '*')
							{
								state = 9;
							}
							value += c;
							break;
						case 4:
							switch (errorKind)
							{
								case ErrorKind.NUMBERERROR:
									if (char.IsDigit(c) || c == '.')
									{
										value += c;
									}
									else
									{
										SourceViewModel.putBack();
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									break;
								case ErrorKind.INVALIDCHAR:
									if (char.IsLetterOrDigit(c) || symbols.Contains(c) || char.IsWhiteSpace(c))
									{
										SourceViewModel.putBack();
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									else
									{
										value += c;
									}
									break;
								default:
									if (char.IsWhiteSpace(c))
									{
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									else
									{
										value += c;
									}
									break;
							}
							break;

						case 5:
							if (char.IsDigit(c))
							{
								state = 6;
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.NUMBERERROR;
								break;
							}

							SourceViewModel.putBack();

							HandleError(startOffset, value, ErrorKind.NUMBERERROR);
							return new Token(startOffset, value, TokenKind.ERROR);

						case 6:
							if (char.IsDigit(c))
							{
								state = 6;
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.NUMBERERROR;
								break;
							}

							SourceViewModel.putBack();
							return new Token(startOffset, value, TokenKind.NUM);

						case 7:
							if (c == '*')
							{
								value += c;
								state = 3;
								break;
							}

							if (c == '/')
							{
								value += c;
								state = 8;
								break;
							}

							SourceViewModel.putBack();
							return new Token(startOffset, value, TokenKind.DIV);

						case 8:
							if (c == '\r' || c == '\n') //for the case "\r\n" and "\n";
							{
								return new Token(startOffset, value, TokenKind.ANNO);
							}

							value += c;
							break;

						case 9:
							if (c == '/')
							{
								value += c;
								return new Token(startOffset, value, TokenKind.ANNO);
							}

							value += c;
							state = 3;
							break;
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
								return new Token(startOffset, value, KeyWords[value]);
							}
							else
							{
								return new Token(startOffset, value, TokenKind.ID);
							}
						case 2:
						case 6:
							return new Token(startOffset, value, TokenKind.NUM);
						case 3:
						case 9:
							HandleError(startOffset, value, ErrorKind.ANNONOTCLOSED);
							return new Token(startOffset, value, TokenKind.ERROR);
						case 4:
							HandleError(startOffset, value, errorKind);
							return new Token(startOffset, value, TokenKind.ERROR);
						case 5:
							HandleError(startOffset, value, ErrorKind.NUMBERERROR);
							return new Token(startOffset, value, TokenKind.ERROR);
						case 7:
							return new Token(startOffset, value, TokenKind.DIV);
						case 8:
							return new Token(startOffset, value, TokenKind.ANNO);
					}
				}
			}
		}

		static private void HandleError(int eOffset, string eValue, ErrorKind ekind)
		{
			Error e = new Error();
			e.line = SourceViewModel.GetLine(eOffset);
			e.lineOffset = SourceViewModel.GetLineOffset(eOffset);
			e.length = eValue.Length;
			e.kind = ekind;
			switch (ekind)
			{
				case ErrorKind.ANNONOTCLOSED:
					e.information = "注释未闭合";
					break;
				case ErrorKind.NUMBERERROR:
					e.information = string.Format("数字 {0} 格式错误", eValue);
					break;
				case ErrorKind.INVALIDCHAR:
					e.information = string.Format("无法识别的字符: {0}", eValue);
					break;
				case ErrorKind.NESTBRA:
					e.information = "出现嵌套的注释";
					break;
				default:
					e.information = string.Format("未知类型错误: {0}", eValue);
					break;
			}
		}

		static public void Reset()
		{
			SourceViewModel.Reset();
		}

		static public void SetLexPosition(int offset)
		{
			SourceViewModel.SetOffset(offset - 1);
		}

		static public void Highlighting()
		{
			var t = LexNext();
			while (t != null)
			{
				switch (t.kind)
				{
					case TokenKind.ADD:
					case TokenKind.SUB:
					case TokenKind.MULT:
					case TokenKind.DIV:
					case TokenKind.EQU:
					case TokenKind.GT:
					case TokenKind.LT:
					case TokenKind.LEQU:
					case TokenKind.GEQU:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.OPERATOR);
						break;
					case TokenKind.SEMI:
					case TokenKind.LBRA:
					case TokenKind.RBRA:
					case TokenKind.LPAR:
					case TokenKind.RPAR:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.PUNCT);
						break;
					case TokenKind.BREAK:
					case TokenKind.DO:
					case TokenKind.ELSE:
					case TokenKind.IF:
					case TokenKind.WHILE:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.KEYWORD);
						break;
					case TokenKind.ID:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.ID);
						break;
					case TokenKind.NUM:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.NUM);
						break;
					case TokenKind.ANNO:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.ANNO);
						break;
					case TokenKind.ERROR:
						SourceViewModel.Colorize(t.offset, t.value.Length, HLKind.ERROR);
						break;
				}
				t = LexNext();
			}
		}

		static public void Test()
		{
			//ProcessViewModel.Clear();
			//var t = LexNext();
			//while (t != null)
			//{
			//	//ProcessViewModel.Add(new Process(t.kind.ToString(), t.value));
			//	t = LexNext();
			//}
		}
	}
}
