using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model
{

	partial class Symbol
	{
		static private int line;
		static private int varIndex;
		static private Stack<int> brotherIndexs = new Stack<int>();
		static private List<string> IRCodes = new List<string>();
		static private List<int> breaks = new List<int>();
		static private Dictionary<int, string> varName = new Dictionary<int, string>();

		public static string CompileToIR(Symbol ast) //生成IR代码的入口
		{
			line = 1;
			varIndex = 0;
			IRCodes.Clear();
			breaks.Clear();
			compile(ast);
			return string.Join("\n", IRCodes);
		}

		private static void addCode(string code)//添加新的IR行
		{
			IRCodes.Add(code);
			line++;
		}

		private static void addBreak()			//处理break语句
		{
			breaks.Add(line);
			addCode("br");
		}

		private static int getNewIndex()		//获取新的变量编号
		{
			return varIndex++;
		}

		private static string getName(int key)	//根据变量编号获取变量名
		{
			if (varName.ContainsKey(key))
			{
				return varName[key];
			}
			else
			{
				return "t" + key.ToString();
			}
		}

		private static int compile(Symbol subAst) //递归过程
		{
			int t0, t1, l0, l1, l2;
			string t;
			if (subAst.type == SymbolType.terminal)
			{
				t = subAst.values[0];
				foreach (var n in varName)
				{
					if (n.Value == t)
					{
						return n.Key;
					}
				}
				t0 = getNewIndex();
				varName[t0] = subAst.values[0];
				return t0;
			}
			else
			{
				switch (subAst.values[0])
				{
					case "0":   //program -> block
						compile(subAst.childrens[0]);
						addCode("HLT");
						return line;

					case "1":   //block -> { stmts }
						return compile(subAst.childrens[0]);

					case "2":   //stmts -> stmt stmts
						compile(subAst.childrens[0]);
						compile(subAst.childrens[1]);
						return line;

					case "3":   //stmts -> ε
						return line;

					case "4":   //stmt -> ID = expr ;
						t0 = compile(subAst.childrens[0]);
						t1 = compile(subAst.childrens[1]);
						addCode(string.Format("MOV {0}, {1}", getName(t1), getName(t0)));
						return line;

					case "5":   //stmt -> IF ( bool ) stmt stmt1
						t0 = compile(subAst.childrens[0]); //bool
						l0 = line; //JE LINE
						addCode("je"); //JE
						compile(subAst.childrens[1]); //stmt
						l1 = line;
						addCode("jmp"); //JMP
						l2 = compile(subAst.childrens[2]); //stmt1
														   //line

						//IR BOOL
						IRCodes[l0 - 1] = string.Format("JE  {0}, {1}", getName(t0), l1 + 1);//+
						//IR STMT															 <-+
						IRCodes[l1 - 1] = string.Format("JMP {0}", line); //+			 
						//IR STMT1										    |
						//IR OTHERS										  <-+
						return line;

					case "6":   //stmt -> WHILE ( bool ) stmt
						l0 = line;
						t0 = compile(subAst.childrens[0]); //bool
						l1 = line; //JE LINE
						addCode("je"); //JE
						compile(subAst.childrens[1]); //stmt
						IRCodes[l1 - 1] = string.Format("JE  {0}, {1}", getName(t0), line + 1);
						addCode(string.Format("JMP {0}", l0)); //line

						//handle breeak
						foreach (var b in breaks)
						{
							IRCodes[b - 1] = string.Format("JMP {0}", line);
						}
						breaks.Clear();

						return line;

					case "7":   //stmt -> DO stmt WHILE ( bool )
						l0 = line;
						compile(subAst.childrens[0]);
						t0 = compile(subAst.childrens[1]);
						addCode(string.Format("JE  {0}, {1}", getName(t0), line + 2)); //line
						addCode(string.Format("JMP {0}", l0)); //line + 1

						//handle breeak
						foreach (var b in breaks)
						{
							IRCodes[b - 1] = string.Format("JMP {0}", line);
						}
						breaks.Clear();

						return line;

					case "8":	//stmt -> BREAK
						addBreak();
						return line - 1;

					case "9":	//stmt -> block
						compile(subAst.childrens[0]);
						return line;

					case "10":	//stmt1 -> ELSE stmt
						compile(subAst.childrens[0]);
						return line;

					case "11":  //stmt1 -> ε
						return line;

					case "12":  //bool -> expr bool1
						brotherIndexs.Push(compile(subAst.childrens[0]));
						return compile(subAst.childrens[1]);

					case "13":  //bool1 -> < expr
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("JL  {0}, {1}, {2}", getName(brotherIndexs.Pop()), getName(t0), line + 3));
						addCode(string.Format("MOV 0, t{0}", t1)); //false
						addCode(string.Format("JMP {0}", line + 2));
						addCode(string.Format("MOV 1, t{0}", t1)); //true

						return t1;

					case "14":  //bool1 -> > expr
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("JG  {0}, {1}, {2}", getName(brotherIndexs.Pop()), getName(t0), line + 3));
						addCode(string.Format("MOV 0, t{0}", t1)); //false
						addCode(string.Format("JMP {0}", line + 2));
						addCode(string.Format("MOV 1, t{0}", t1)); //true

						return t1;

					case "15":  //bool1 -> <= expr
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("JG  {0}, {1}, {2}", getName(brotherIndexs.Pop()), getName(t0), line + 3)); // a <= b < == > !(a > b)
						addCode(string.Format("MOV 1, t{0}", t1)); //true
						addCode(string.Format("JMP {0}", line + 2));
						addCode(string.Format("MOV 0, t{0}", t1)); //false

						return t1;

					case "16":  //bool1 -> >= expr
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("JL  {0}, {1}, {2}", getName(brotherIndexs.Pop()), getName(t0), line + 3)); // a >= b < == > !(a < b)
						addCode(string.Format("MOV 1, t{0}", t1)); //true
						addCode(string.Format("JMP {0}", line + 2));
						addCode(string.Format("MOV 0, t{0}", t1)); //false

						return t1;

					case "17":  //bool1 -> ε
						return brotherIndexs.Pop();

					case "18":  //expr -> term expr1
						brotherIndexs.Push(compile(subAst.childrens[0]));
						return compile(subAst.childrens[1]);

					case "19":  //expr1 -> + term expr1
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("ADD {0}, {1}, t{2}", getName(brotherIndexs.Pop()), getName(t0), t1)); // a + b
						brotherIndexs.Push(t1);
						return compile(subAst.childrens[1]);

					case "20":  //expr1 -> - term expr1
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("SUB {0}, {1}, t{2}", getName(brotherIndexs.Pop()), getName(t0), t1)); // a - b
						brotherIndexs.Push(t1);
						return compile(subAst.childrens[1]);

					case "21":  //expr1 -> ε
						return brotherIndexs.Pop();

					case "22":  //term -> factor term1
						brotherIndexs.Push(compile(subAst.childrens[0]));
						return compile(subAst.childrens[1]);

					case "23":  //term1 -> * factor term1
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("MUL {0}, {1}, t{2}", getName(brotherIndexs.Pop()), getName(t0), t1)); // a * b
						brotherIndexs.Push(t1);
						return compile(subAst.childrens[1]);

					case "24":  //term1 -> / factor term1
						t0 = compile(subAst.childrens[0]);
						t1 = getNewIndex();
						addCode(string.Format("DIV {0}, {1}, t{2}", getName(brotherIndexs.Pop()), getName(t0), t1)); // a / b
						brotherIndexs.Push(t1);
						return compile(subAst.childrens[1]);

					case "25":  //term1 -> ε
						return brotherIndexs.Pop();

					case "26":  //factor -> ( expr )
					case "27":  //factor -> ID
					case "28":  //factor -> NUM
						return compile(subAst.childrens[0]);
				}
			}
			return line;
		}
	}
}
