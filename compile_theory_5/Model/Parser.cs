using compile_theory_5.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model
{
	enum SymbolKind
	{
		program,
		block,
		stmts,
		stmt,
		stmt1,
		_bool,
		_bool1,
		_bool2,
		expr,
		expr1,
		term,
		term1,
		factor,
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
		NULL,
		ALL
	}

	class SimpleProcess
	{
		public SimpleProcess(SymbolKind from, SymbolKind to)
		{
			this.from = from;
			this.to = to;
		}

		public SymbolKind from { get; set; }
		public SymbolKind to { get; set; }
		public int offset { get; set; }
		public int length { get; set; }
	}

	class SimpleError
	{
		public SimpleError(SimpleProcess errorProcess)
		{
			this.errorProcess = errorProcess;
		}

		public SimpleError(int line, SimpleProcess errorProcess)
		{
			this.line = line;
			this.errorProcess = errorProcess;
		}

		public int line { get; set; }
		public SimpleProcess errorProcess { get; set; }
	}

	class Parser
	{
		static private Stack<int> recoverPoint = new Stack<int>();
		static private Token token;
		static private Token OldToken;
		static private LinkedList<SymbolKind> result = new LinkedList<SymbolKind>();
		static private Stack<Symbol> symbolStack = new Stack<Symbol>();
		static private Symbol tempSymbol;
		static private List<SimpleError> errores = new List<SimpleError>();

		//用来匹配输入词法单元
		static private bool accapt(TokenKind tokenKind)
		{//token may null
			if (token == null)
			{
				return false;
			}
			if (tokenKind == token.kind)
			{
				OldToken = token;
				do
				{
					token = Lexer.LexNext();
					if (token == null)
					{
						token = new Token(SourceViewModel.GetEndOffset(), "", TokenKind.EOF);
						break;
					}
				} while (token.kind == TokenKind.ANNO);
				return true;
			}
			else
			{
				return false;
			}
		}

		static public void parse(Action parseCallback)	//语法分析入口
		{
			SourceViewModel.NormalMode();
			SourceViewModel.KeepOnlyRead();
			ErrorViewModel.getInstance().clear();
			errores.Clear();
			result.Clear();

			Lexer.Reset();
			token = Lexer.LexNext();
			OldToken = token;
			if (token == null)
			{
				SourceViewModel.UnkeepOnlyRead();
				return;
			}
			if (program())
			{
				if(symbolStack.Count > 0)
				{
					IRSystem.IRSystem.source.SetText(Symbol.CompileToIR(symbolStack.Peek()));
					IRSystem.IRSystem.Stop();
					parseCallback();
				}
			}
			else
			{
				SourceViewModel.ErrorMode();
			}
			SourceViewModel.UnkeepOnlyRead();
		}
		
		//采用返回 bool 来提示错误
		static private bool program()
		{
			symbolStack.Push(new Symbol(SymbolType.nonterminal, SymbolKind.program));
			symbolStack.Peek().values.Add("0");
			if (!block())
			{
				return false;
			}

			if (!accapt(TokenKind.EOF))
			{
				AddError(SymbolKind.program, SymbolKind.block, "多余的符号串");
				return false;
			}
			
			return true;
		}

		static private bool block()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.block);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			symbolStack.Peek().values.Add("1");
			if (!accapt(TokenKind.LBRA))
			{
				AddError(SymbolKind.block, SymbolKind.LBRA, "缺少预期符号 {");
				return false;
			}

			if (!stmts())
			{
				return false;
			}
			
			if (!accapt(TokenKind.RBRA))
			{
				AddError(SymbolKind.block, SymbolKind.LBRA, "缺少预期符号 }");
				return false;
			}

			symbolStack.Pop();
			return true;
		}

		static private bool stmts()
		{
			if (token == null)
			{
				AddError(SymbolKind.NULL, SymbolKind.NULL, "异常的结尾");
				return false;
			}

			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.stmts);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			switch (token.kind)
			{
				case TokenKind.ID:
				case TokenKind.IF:
				case TokenKind.WHILE:
				case TokenKind.DO:
				case TokenKind.BREAK:
				case TokenKind.LBRA:
					symbolStack.Peek().values.Add("2");
					if (!stmt())
					{
						return false;
					}

					if (!stmts())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				default:
					symbolStack.Peek().values.Add("3");
					symbolStack.Pop();
					return true;
			}
		}

		static private bool stmt()
		{
			if (token == null)
			{
				AddError(SymbolKind.NULL, SymbolKind.NULL, "异常的结尾");
				return false;
			}
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.stmt);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			switch (token.kind)
			{
				case TokenKind.ID:
					symbolStack.Peek().values.Add("4");
					symbolStack.Peek().childrens.Add(new Symbol(SymbolType.terminal, SymbolKind.ID, token.value));

					if (!accapt(TokenKind.ID))
					{
						AddError(SymbolKind.stmt, SymbolKind.ID, "内部错误");
						return false;
					}
					
					if (!accapt(TokenKind.EQU))
					{
						AddError(SymbolKind.stmt, SymbolKind.ID, "缺少预期符号 =");
						return false;
					}

					if (!expr())
					{
						return false;
					}
					
					if (!accapt(TokenKind.SEMI))
					{
						AddError(SymbolKind.stmt, SymbolKind.ID, "缺少预期符号 ;");
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.IF:
					symbolStack.Peek().values.Add("5");
					if (!accapt(TokenKind.IF))
					{
						AddError(SymbolKind.stmt, SymbolKind.IF, "内部错误");
						return false;
					}

					if (!accapt(TokenKind.LPAR))
					{
						AddError(SymbolKind.stmt, SymbolKind.IF, "缺少预期符号 (");
						return false;
					}

					if (!_bool())
					{
						return false;
					}

					if (!accapt(TokenKind.RPAR))
					{
						AddError(SymbolKind.stmt, SymbolKind.IF, "缺少预期符号 )");
						return false;
					}

					if (!stmt())
					{
						return false;
					}

					if (!stmt1())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.WHILE:
					symbolStack.Peek().values.Add("6");
					if (!accapt(TokenKind.WHILE))
					{
						AddError(SymbolKind.stmt, SymbolKind.WHILE, "内部错误");
						return false;
					}

					if (!accapt(TokenKind.LPAR))
					{
						AddError(SymbolKind.stmt, SymbolKind.WHILE, "缺少预期符号 (");
						return false;
					}

					if (!_bool())
					{
						return false;
					}

					if (!accapt(TokenKind.RPAR))
					{
						AddError(SymbolKind.stmt, SymbolKind.WHILE, "缺少预期符号 )");
						return false;
					}

					if (!stmt())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.DO:
					symbolStack.Peek().values.Add("7");
					if (!accapt(TokenKind.DO))
					{
						AddError(SymbolKind.stmt, SymbolKind.DO, "内部错误");
						return false;
					}

					if (!stmt())
					{
						return false;
					}

					if (!accapt(TokenKind.WHILE))
					{
						AddError(SymbolKind.stmt, SymbolKind.DO, "缺少关键字 while");
						return false;
					}

					if (!accapt(TokenKind.LPAR))
					{
						AddError(SymbolKind.stmt, SymbolKind.DO, "缺少预期符号 (");
						return false;
					}

					if (!_bool())
					{
						return false;
					}

					if (!accapt(TokenKind.RPAR))
					{
						AddError(SymbolKind.stmt, SymbolKind.DO, "缺少预期符号 )");
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.BREAK:
					symbolStack.Peek().values.Add("8");
					if (!accapt(TokenKind.BREAK))
					{
						AddError(SymbolKind.stmt, SymbolKind.BREAK, "内部错误");
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.LBRA:
					symbolStack.Peek().values.Add("9");
					if (!block())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				default:
					AddError(SymbolKind.stmt, SymbolKind.ALL, "找不到合法的后续符号");
					return false;
			}
		}

		static private bool stmt1()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.stmt1);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			if (token.kind == TokenKind.ELSE)
			{
				symbolStack.Peek().values.Add("10");
				if (!accapt(TokenKind.ELSE))
				{
					AddError(SymbolKind.stmt1, SymbolKind.ELSE, "内部错误");
					return false;
				}

				if (!stmt())
				{
					return false;
				}

				symbolStack.Pop();
				return true;
			}

			symbolStack.Peek().values.Add("11");
			symbolStack.Pop();
			return true;
		}

		static private bool _bool()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind._bool);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);
			symbolStack.Peek().values.Add("12");
			if (!expr())
			{
				return false;
			}

			if (!_bool1())
			{
				return false;
			}

			symbolStack.Pop();
			return true;
		}

		static private bool _bool1()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind._bool1);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			switch (token.kind)
			{
				case TokenKind.LT:
					symbolStack.Peek().values.Add("13");
					if (!accapt(TokenKind.LT))
					{
						AddError(SymbolKind._bool1, SymbolKind.LT, "内部错误");
						return false;
					}

					if (!expr())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.GT:
					symbolStack.Peek().values.Add("14");
					if (!accapt(TokenKind.GT))
					{
						AddError(SymbolKind._bool1, SymbolKind.GT, "内部错误");
						return false;
					}

					if (!expr())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.LEQU:
					symbolStack.Peek().values.Add("15");
					if (!accapt(TokenKind.LEQU))
					{
						AddError(SymbolKind._bool1, SymbolKind.LEQU, "内部错误");
						return false;
					}

					if (!expr())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.GEQU:
					symbolStack.Peek().values.Add("16");
					if (!accapt(TokenKind.GEQU))
					{
						AddError(SymbolKind._bool1, SymbolKind.GEQU, "内部错误");
						return false;
					}

					if (!expr())
					{
						return false;
					}

					symbolStack.Pop();
					return true;

				default:
					symbolStack.Peek().values.Add("17");
					symbolStack.Pop();
					return true;
			}
		}
		
		static private bool expr()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.expr);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);
			symbolStack.Peek().values.Add("18");
			if (!term())
			{
				return false;
			}

			if (!expr1())
			{
				return false;
			}

			symbolStack.Pop();
			return true;
		}

		static private bool expr1()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.expr1);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			switch (token.kind)
			{
				case TokenKind.ADD:
					symbolStack.Peek().values.Add("19");
					if (!accapt(TokenKind.ADD))
					{
						AddError(SymbolKind.expr1, SymbolKind.ADD, "内部错误");
						return false;
					}

					if (!term())
					{
						return false;
					}

					if (!expr1())
					{
						return false;
					}

					symbolStack.Pop();
					return true;
				case TokenKind.SUB:
					symbolStack.Peek().values.Add("20");
					if (!accapt(TokenKind.SUB))
					{
						AddError(SymbolKind.expr1, SymbolKind.SUB, "内部错误");
						return false;
					}

					if (!term())
					{
						return false;
					}

					if (!expr1())
					{
						return false;
					}

					symbolStack.Pop();
					return true;
				default:
					symbolStack.Peek().values.Add("21");
					symbolStack.Pop();
					return true;
			}
		}

		static private bool term()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.expr1);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);
			symbolStack.Peek().values.Add("22");
			if (!factor())
			{
				return false;
			}

			if (!term1())
			{
				return false;
			}

			symbolStack.Pop();
			return true;
		}

		static private bool term1()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.term1);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			switch (token.kind)
			{
				case TokenKind.MULT:
					symbolStack.Peek().values.Add("23");
					if (!accapt(TokenKind.MULT))
					{
						AddError(SymbolKind.term1, SymbolKind.MULT, "内部错误");
						return false;
					}

					if (!factor())
					{
						return false;
					}

					if (!term1())
					{
						return false;
					}

					symbolStack.Pop();
					return true;
				case TokenKind.DIV:
					symbolStack.Peek().values.Add("24");
					if (!accapt(TokenKind.DIV))
					{
						AddError(SymbolKind.term1, SymbolKind.DIV, "内部错误");
						return false;
					}

					if (!factor())
					{
						return false;
					}

					if (!term1())
					{
						return false;
					}

					symbolStack.Pop();
					return true;
				default:
					symbolStack.Peek().values.Add("25");
					symbolStack.Pop();
					return true;
			}
		}

		static private bool factor()
		{
			tempSymbol = new Symbol(SymbolType.nonterminal, SymbolKind.term1);
			symbolStack.Peek().childrens.Add(tempSymbol);
			symbolStack.Push(tempSymbol);

			switch (token.kind)
			{
				case TokenKind.LPAR:
					symbolStack.Peek().values.Add("26");

					if (!accapt(TokenKind.LPAR))
					{
						AddError(SymbolKind.factor, SymbolKind.LPAR, "内部错误");
						return false;
					}

					if (!expr())
					{
						return false;
					}

					if (!accapt(TokenKind.RPAR))
					{
						AddError(SymbolKind.factor, SymbolKind.LPAR, "缺少预期符号 )");
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.ID:
					symbolStack.Peek().values.Add("27");
					symbolStack.Peek().childrens.Add(new Symbol(SymbolType.terminal, SymbolKind.ID, token.value));

					if (!accapt(TokenKind.ID))
					{
						AddError(SymbolKind.factor, SymbolKind.ID, "内部错误");
						return false;
					}

					symbolStack.Pop();
					return true;

				case TokenKind.NUM:
					symbolStack.Peek().values.Add("28");
					symbolStack.Peek().childrens.Add(new Symbol(SymbolType.terminal, SymbolKind.NUM, token.value));

					if (!accapt(TokenKind.NUM))
					{
						AddError(SymbolKind.factor, SymbolKind.NUM, "内部错误");
						return false;
					}

					symbolStack.Pop();
					return true;
				default:
					AddError(SymbolKind.factor, SymbolKind.ALL, "找不到合法的后续符号");
					return false;
			}
		}
		
		static private string GetProduction(SimpleProcess errorProcess)
		{
			if (errorProcess != null)
			{
				switch (errorProcess.from)
				{
					case SymbolKind.program:
						return "program -> block";
					case SymbolKind.block:
						return "block -> { stmts }";
					case SymbolKind.stmts:
						switch (errorProcess.to)
						{
							case SymbolKind.stmt:
								return "stmts -> stmt stmts";
							case SymbolKind.NULL:
								return "stmts -> NULL";
							default:
								return null;
						}

					case SymbolKind.stmt:
						switch (errorProcess.to)
						{
							case SymbolKind.ID:
								return "stmt -> ID = expr;";
							case SymbolKind.IF:
								return "stmt -> IF ( bool ) stmt stmt1";

							case SymbolKind.WHILE:
								return "stmt -> WHILE ( bool ) stmt";

							case SymbolKind.DO:
								return "stmt -> DO stmt WHILE ( bool )";

							case SymbolKind.BREAK:
								return "stmt -> BREAK";

							case SymbolKind.block:
								return "stmt -> block";
							case SymbolKind.ALL:
								return "stmt -> ID = expr ;\n\t | IF ( bool ) stmt stmt1\n\t | WHILE( bool ) stmt\n\t | DO stmt WHILE ( bool )\n\t | BREAK";
							default:
								return null;
						}

					case SymbolKind.stmt1:
						switch (errorProcess.to)
						{
							case SymbolKind.ELSE:
								return "stmt1 -> ELSE stmt";

							case SymbolKind.NULL:
								return "stmt1 -> NULL";
							default:
								return null;
						}

					case SymbolKind._bool:
						return "bool -> expr bool1";

					case SymbolKind._bool1:
						switch (errorProcess.to)
						{
							case SymbolKind.LT:
								return "bool1 -> < bool2";

							case SymbolKind.GT:
								return "bool1 -> > bool2";

							case SymbolKind.NULL:
								return "bool1 -> NULL";
							default:
								return null;
						}

					case SymbolKind._bool2:
						switch (errorProcess.to)
						{
							case SymbolKind.expr:
								return "bool2 -> expr";

							case SymbolKind.EQU:
								return "bool2 -> = expr";
							default:
								return null;
						}

					case SymbolKind.expr:
						return "expr -> term expr1";

					case SymbolKind.expr1:
						switch (errorProcess.to)
						{
							case SymbolKind.ADD:
								return "expr1 -> + term expr1";

							case SymbolKind.SUB:
								return "expr1 -> - term expr1";

							case SymbolKind.NULL:
								return "expr1 -> NULL";
							default:
								return null;
						}

					case SymbolKind.term:
						return "term -> factor term1";

					case SymbolKind.term1:
						switch (errorProcess.to)
						{
							case SymbolKind.MULT:
								return "term1 -> * factor term1";

							case SymbolKind.DIV:
								return "term1 -> / factor term1";

							case SymbolKind.NULL:
								return "term1 -> NULL";
							default:
								return null;
						}

					case SymbolKind.factor:
						switch (errorProcess.to)
						{
							case SymbolKind.LPAR:
								return "factor -> ( expr )";

							case SymbolKind.ID:
								return "factor -> ID";

							case SymbolKind.NUM:
								return "factor -> NUM";
							case SymbolKind.ALL:
								return "factor -> ( expr )\n\t | ID  NUM";
							default:
								return null;
						}
					default:
						return null;
				}
			}
			else
			{
				return null;
			}
		}

		static private void AddError(SymbolKind from, SymbolKind to, string Information)
		{
			Error err;
			err = new Error();
			if (token != null)
			{
				err.line = SourceViewModel.GetLine(token.offset);
				err.lineOffset = SourceViewModel.GetLineOffset(token.offset);
				err.length = token.value.Length;
				err.isVisable = true;
			}
			err.value = GetProduction(new SimpleProcess(from, to));
			err.information = Information;
			ErrorViewModel.getInstance().addError(err);
		}

	}
}
