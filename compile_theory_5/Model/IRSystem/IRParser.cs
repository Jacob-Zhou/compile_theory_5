using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem
{
	class IRParser
	{
		private List<IRToken> parserTokens;
		private IRToken token;
		private int tokenOffset;
		private int line;
		private IRToken OldToken;
		private IRProcess process;
		private byte operandType = 0; //id: 1; num: 0
		private byte operandCount = 0;
		private List<double> operandValues = new List<double>();
		private double operandValue;
		private int operandValueI;
		private int operandResult;

		public IRProcess ParseLine(int line)
		{
			parserTokens = IRSystem.lexer.LexLine(line);
			operandValues.Clear();
			tokenOffset = 0;
			this.line = line;
			NextToken();
			operandCount = 0;
			operandType = 0;
			operandResult = -1;
			if (token == null)
			{
				BuildProcess(IRTokenKind.NOP);
				return process;
			}
			if (instruction())
			{
				return process;
			}
			else
			{
				return null;
			}
		}

		private bool accapt(IRTokenKind tokenKind)
		{//token may null
			if (token == null)
			{
				return false;
			}
			if (tokenKind == token.kind)
			{
				OldToken = token;
				NextToken();
				if (token == null)
				{
					token = new IRToken(IRTokenKind.EOL);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		private void NextToken()
		{
			if (tokenOffset < parserTokens.Count)
			{
				token = parserTokens[tokenOffset];
				tokenOffset++;
			}
			else
			{
				token = null;
			}
		}

		private void BuildProcess(IRTokenKind IRTK)
		{
			switch (IRTK)
			{
				case IRTokenKind.MOV:
					process = new IRProcess(InstructionType.MOV, operandCount);
					break;
				case IRTokenKind.ADD:
					process = new IRProcess(InstructionType.ADD, operandCount);
					break;
				case IRTokenKind.SUB:
					process = new IRProcess(InstructionType.SUB, operandCount);
					break;
				case IRTokenKind.MUL:
					process = new IRProcess(InstructionType.MUL, operandCount);
					break;
				case IRTokenKind.DIV:
					process = new IRProcess(InstructionType.DIV, operandCount);
					break;
				case IRTokenKind.JMP:
					process = new IRProcess(InstructionType.JMP, operandCount);
					break;
				case IRTokenKind.JE:
					process = new IRProcess(InstructionType.JE, operandCount);
					break;
				case IRTokenKind.JL:
					process = new IRProcess(InstructionType.JL, operandCount);
					break;
				case IRTokenKind.JG:
					process = new IRProcess(InstructionType.JG, operandCount);
					break;
				case IRTokenKind.NOP:
					process = new IRProcess(InstructionType.NOP, operandCount);
					break;
				case IRTokenKind.HLT:
					process = new IRProcess(InstructionType.HLT, operandCount);
					break;
				default:
					process = new IRProcess(InstructionType.NOP, operandCount);
					break;
			}
			switch (operandCount)
			{
				case 0:
					process.operandType = OperandType.NULL;
					break;
				case 1:
					if (operandType == 0)
					{
						process.operandType = OperandType.N;
						process.literals = new List<double>();
						for(int i = 0; i < 1; i++)
						{
							process.literals.Add(operandValues[i]);
						}
					}
					else
					{
						process.operandType = OperandType.I;
						process.operands = new List<int>();
						for (int i = 0; i < 1; i++)
						{
							process.operands.Add((int)operandValues[i]);
						}
					}
					break;
				case 2:
					switch (operandType)
					{
						case 0:
							process.operandType = OperandType.NN;
							process.literals = new List<double>();
							for (int i = 0; i < 2; i++)
							{
								process.literals.Add(operandValues[i]);
							}
							break;
						case 1:
							process.operandType = OperandType.NI;
							process.literals = new List<double>();
							process.operands = new List<int>();
							process.literals = new List<double> { operandValues[0] };
							process.operands = new List<int> { (int)operandValues[1] };
							break;
						case 2:
							process.operandType = OperandType.IN;
							process.literals = new List<double> { operandValues[1] };
							process.operands = new List<int> { (int)operandValues[0] };
							break;
						case 3:
							process.operandType = OperandType.II;
							process.operands = new List<int>();
							for (int i = 0; i < 2; i++)
							{
								process.operands.Add((int)operandValues[i]);
							}
							break;
					}
					break;
				case 3:
					switch (operandType)
					{
						case 0:
							process.operandType = OperandType.NN | OperandType.N;
							process.literals = new List<double>();
							for (int i = 0; i < 3; i++)
							{
								process.literals.Add(operandValues[i]);
							}
							break;
						case 1:
							process.operandType = OperandType.NN | OperandType.I;
							process.literals = new List<double> { operandValues[0], operandValues[1] };
							process.operands = new List<int> { (int)operandValues[2] };
							break;
						case 2:
							process.operandType = OperandType.NI | OperandType.N;
							process.literals = new List<double> { operandValues[0], operandValues[2] };
							process.operands = new List<int> { (int)operandValues[1] };
							break;
						case 3:
							process.operandType = OperandType.NI | OperandType.I;
							process.literals = new List<double> { operandValues[0] };
							process.operands = new List<int> { (int)operandValues[1], (int)operandValues[2] };
							break;
						case 4:
							process.operandType = OperandType.IN | OperandType.N;
							process.literals = new List<double> { operandValues[1], operandValues[2] };
							process.operands = new List<int> { (int)operandValues[0] };
							break;
						case 5:
							process.operandType = OperandType.IN | OperandType.I;
							process.literals = new List<double> { operandValues[1] };
							process.operands = new List<int> { (int)operandValues[0], (int)operandValues[2] };
							break;
						case 6:
							process.operandType = OperandType.II | OperandType.N;
							process.literals = new List<double> {operandValues[2] };
							process.operands = new List<int> { (int)operandValues[0], (int)operandValues[1]};
							break;
						case 7:
							process.operandType = OperandType.II | OperandType.I;
							process.operands = new List<int>();
							for (int i = 0; i < 3; i++)
							{
								process.operands.Add((int)operandValues[i]);
							}
							break;
					}
					break;
				default:
					break;
			}
			if(operandResult != -1)
			{
				if(process.operands == null)
				{
					process.operands = new List<int>();
				}
				process.operands.Add(operandResult);
			}
		}

		private bool instruction()
		{
			switch (token.kind)
			{
				case IRTokenKind.MOV:
					if (!accapt(IRTokenKind.MOV))
					{
						return false;
					}

					if (!dual_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if(operandCount != 1)
					{
						return false;
					}

					BuildProcess(IRTokenKind.MOV);
					return true;

				case IRTokenKind.ADD:
					if (!accapt(IRTokenKind.ADD))
					{
						return false;
					}

					if (!triple_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.ADD);
					return true;

				case IRTokenKind.SUB:
					if (!accapt(IRTokenKind.SUB))
					{
						return false;
					}

					if (!triple_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.SUB);
					return true;

				case IRTokenKind.MUL:
					if (!accapt(IRTokenKind.MUL))
					{
						return false;
					}

					if (!triple_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.MUL);
					return true;

				case IRTokenKind.DIV:
					if (!accapt(IRTokenKind.DIV))
					{
						return false;
					}

					if (!triple_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.DIV);
					return true;

				case IRTokenKind.JMP:
					if (!accapt(IRTokenKind.JMP))
					{
						return false;
					}

					if (!operand())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 1)
					{
						return false;
					}

					BuildProcess(IRTokenKind.JMP);
					return true;

				case IRTokenKind.JE:
					if (!accapt(IRTokenKind.JE))
					{
						return false;
					}

					if (!dt_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 3 && operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.JE);
					return true;

				case IRTokenKind.JL:
					if (!accapt(IRTokenKind.JL))
					{
						return false;
					}

					if (!dt_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 3 && operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.JL);
					return true;

				case IRTokenKind.JG:
					if (!accapt(IRTokenKind.JG))
					{
						return false;
					}

					if (!dt_operands())
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 3 && operandCount != 2)
					{
						return false;
					}

					BuildProcess(IRTokenKind.JG);
					return true;

				case IRTokenKind.NOP:
					if (!accapt(IRTokenKind.NOP))
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 0)
					{
						return false;
					}

					BuildProcess(IRTokenKind.NOP);
					return true;

				case IRTokenKind.HLT:
					if (!accapt(IRTokenKind.HLT))
					{
						return false;
					}

					if (!accapt(IRTokenKind.EOL))
					{
						return false;
					}

					if (operandCount != 0)
					{
						return false;
					}

					BuildProcess(IRTokenKind.HLT);
					return true;
			}
			return false;
		}

		private bool dt_operands()
		{
			if (!operand())
			{
				return false;
			}

			if (!accapt(IRTokenKind.COMMA))
			{
				return false;
			}

			if (!operand())
			{
				return false;
			}

			if (!dt_operands_1())
			{
				return false;
			}
			return true;
		}

		private bool dt_operands_1()
		{
			switch (token.kind)
			{
				case IRTokenKind.COMMA:
					if (!accapt(IRTokenKind.COMMA))
					{
						return false;
					}

					if (!operand())
					{
						return false;
					}
					return true;
				case IRTokenKind.EOL:
					return true;
			}
			return false;
		}

		private bool triple_operands()
		{
			if (!operand())
			{
				return false;
			}

			if (!accapt(IRTokenKind.COMMA))
			{
				return false;
			}

			if (!dual_operands())
			{
				return false;
			}
			return true;
		}

		private bool dual_operands()
		{
			if (!operand())
			{
				return false;
			}

			if (!accapt(IRTokenKind.COMMA))
			{
				return false;
			}

			if (!accapt(IRTokenKind.ID))
			{
				return false;
			}

			if (!IRSystem.momory.TryFindAddress(OldToken.value, out operandResult))
			{
				operandResult = IRSystem.momory.Add(OldToken.value);
			}
			return true;
		}

		private bool operand()
		{
			switch (token.kind)
			{
				case IRTokenKind.ID:
					if (!accapt(IRTokenKind.ID))
					{
						return false;
					}
					if(!IRSystem.momory.TryFindAddress(OldToken.value, out operandValueI))
					{
						operandValueI = operandResult = IRSystem.momory.Add(OldToken.value);
					}
					operandValues.Add(operandValueI);
					operandType <<= 1;
					operandType += 1;
					operandCount++;
					return true;

				case IRTokenKind.NUM:
					if (!accapt(IRTokenKind.NUM))
					{
						return false;
					}
					if(!double.TryParse(OldToken.value, out operandValue))
					{
						return false;
					}
					operandValues.Add(operandValue);
					operandType <<= 1;
					operandCount++;
					return true;
			}
			return false;
		}
	}
}
