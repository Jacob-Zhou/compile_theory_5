using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem.IRInterpreter
{
	static class IRExecuteUnit
	{
		public static void Run(this IRProcess p)
		{
			OperandType targetType;
			OperandType sourceType;
			int Icount = 0;
			int Ncount = 0;
			bool isJump = false;
			switch (p.type)
			{
				case InstructionType.MOV:
					if(p.operandType == OperandType.I)
					{
						IRSystem.momory.SetValue(p.operands[1], IRSystem.momory.GetValue(p.operands[0]));
					}
					else if(p.operandType == OperandType.N)
					{
						IRSystem.momory.SetValue(p.operands[0], p.literals[0]);
					}
					else
					{
						//Error
					}
					return;
				case InstructionType.ADD:
					switch (p.operandType)
					{
						case OperandType.II:
							IRSystem.momory.SetValue(p.operands[2], IRSystem.momory.GetValue(p.operands[0]) + IRSystem.momory.GetValue(p.operands[1]));
							break;
						case OperandType.IN:
						case OperandType.NI:
							IRSystem.momory.SetValue(p.operands[1], IRSystem.momory.GetValue(p.operands[0]) + p.literals[0]);
							break;
						case OperandType.NN:
							IRSystem.momory.SetValue(p.operands[0], p.literals[0] + p.literals[1]);
							break;
					}
					return;
				case InstructionType.SUB:
					switch (p.operandType)
					{
						case OperandType.II:
							IRSystem.momory.SetValue(p.operands[2], IRSystem.momory.GetValue(p.operands[0]) - IRSystem.momory.GetValue(p.operands[1]));
							break;
						case OperandType.IN:
							IRSystem.momory.SetValue(p.operands[1], IRSystem.momory.GetValue(p.operands[0]) - p.literals[0]);
							break;
						case OperandType.NI:
							IRSystem.momory.SetValue(p.operands[1], p.literals[0] - IRSystem.momory.GetValue(p.operands[0]));
							break;
						case OperandType.NN:
							IRSystem.momory.SetValue(p.operands[0], p.literals[0] - p.literals[1]);
							break;
					}
					return;
				case InstructionType.MUL:
					switch (p.operandType)
					{
						case OperandType.II:
							IRSystem.momory.SetValue(p.operands[2], IRSystem.momory.GetValue(p.operands[0]) * IRSystem.momory.GetValue(p.operands[1]));
							break;
						case OperandType.IN:
						case OperandType.NI:
							IRSystem.momory.SetValue(p.operands[1], IRSystem.momory.GetValue(p.operands[0]) * p.literals[0]);
							break;
						case OperandType.NN:
							IRSystem.momory.SetValue(p.operands[0], p.literals[0] * p.literals[1]);
							break;
					}
					return;
				case InstructionType.DIV:
					switch (p.operandType)
					{
						case OperandType.II:
							IRSystem.momory.SetValue(p.operands[2], IRSystem.momory.GetValue(p.operands[0]) / IRSystem.momory.GetValue(p.operands[1]));
							break;
						case OperandType.IN:
							IRSystem.momory.SetValue(p.operands[1], IRSystem.momory.GetValue(p.operands[0]) / p.literals[0]);
							break;
						case OperandType.NI:
							IRSystem.momory.SetValue(p.operands[1], p.literals[0] / IRSystem.momory.GetValue(p.operands[0]));
							break;
						case OperandType.NN:
							IRSystem.momory.SetValue(p.operands[0], p.literals[0] / p.literals[1]);
							break;
					}
					return;

				case InstructionType.JMP:
					if (p.operandType == OperandType.I)
					{
						IRSystem.interpreter.instructionPointer = (int)IRSystem.momory.GetValue(p.operands[0]);
					}
					else if (p.operandType == OperandType.N)
					{
						IRSystem.interpreter.instructionPointer = (int)p.literals[0];
					}
					else
					{
						//Error
						return;
					}
					//IRSystem.interpreter.instructionPointer -= 2;
					return;

				case InstructionType.JE:
					targetType = p.operandType & (OperandType)3;
					sourceType = OperandType.NULL;
					Icount = 0;
					Ncount = 0;
					isJump = false;

					if (targetType == OperandType.NULL)
					{
						switch (p.operandType)
						{
							case OperandType.II:
								targetType = OperandType.I;
								sourceType = OperandType.I;
								break;
							case OperandType.NI:
								targetType = OperandType.I;
								sourceType = OperandType.N;
								break;
							case OperandType.IN:
								targetType = OperandType.N;
								sourceType = OperandType.I;
								break;
							case OperandType.NN:
								targetType = OperandType.N;
								sourceType = OperandType.N;
								break;
							default:
								//ERROR
								break;
						}
					}
					else
					{
						sourceType = p.operandType & (OperandType)28;
					}

					switch (sourceType)
					{
						case OperandType.I:
							if (IRSystem.momory.GetValue(p.operands[0]) == 0)
							{
								isJump = true;
								Icount = 1;
							}
							break;
						case OperandType.N:
							if (p.literals[0] == 0)
							{
								isJump = true;
								Ncount = 1;
							}
							break;
						case OperandType.II:
							if (IRSystem.momory.GetValue(p.operands[0]) == IRSystem.momory.GetValue(p.operands[1]))
							{
								isJump = true;
								Icount = 2;
							}
							break;
						case OperandType.IN:
							if (IRSystem.momory.GetValue(p.operands[0]) == p.literals[0])
							{
								isJump = true;
								Icount = 1;
								Ncount = 1;
							}
							break;
						case OperandType.NI:
							if (p.literals[0] == IRSystem.momory.GetValue(p.operands[0]))
							{
								isJump = true;
								Icount = 1;
								Ncount = 1;
							}
							break;
						case OperandType.NN:
							if (p.literals[0] == p.literals[1])
							{
								isJump = true;
								Ncount = 2;
							}
							break;
					}

					if (isJump)
					{
						if (targetType == OperandType.I)
						{
							IRSystem.interpreter.instructionPointer = (int)IRSystem.momory.GetValue(p.operands[Icount]);
						}
						else if (targetType == OperandType.N)
						{
							IRSystem.interpreter.instructionPointer = (int)p.literals[Ncount];
						}
						else
						{
							return;
							//ERROR
						}
						//IRSystem.interpreter.instructionPointer -= 2;
					}
					return;

				case InstructionType.JL:
					targetType = p.operandType & (OperandType)3;
					sourceType = OperandType.NULL;
					Icount = 0;
					Ncount = 0;
					isJump = false;
					if (targetType == OperandType.NULL)
					{
						switch (p.operandType)
						{
							case OperandType.II:
								targetType = OperandType.I;
								sourceType = OperandType.I;
								break;
							case OperandType.NI:
								targetType = OperandType.I;
								sourceType = OperandType.N;
								break;
							case OperandType.IN:
								targetType = OperandType.N;
								sourceType = OperandType.I;
								break;
							case OperandType.NN:
								targetType = OperandType.N;
								sourceType = OperandType.N;
								break;
							default:
								//ERROR
								break;
						}
					}
					else
					{
						sourceType = p.operandType & (OperandType)28;
					}

					switch (sourceType)
					{
						case OperandType.I:
							if (IRSystem.momory.GetValue(p.operands[0]) < 0)
							{
								isJump = true;
								Icount = 1;
							}
							break;
						case OperandType.N:
							if (p.literals[0] < 0)
							{
								isJump = true;
								Ncount = 1;
							}
							break;
						case OperandType.II:
							if (IRSystem.momory.GetValue(p.operands[0]) < IRSystem.momory.GetValue(p.operands[1]))
							{
								isJump = true;
								Icount = 2;
							}
							break;
						case OperandType.IN:
							if (IRSystem.momory.GetValue(p.operands[0]) < p.literals[0])
							{
								isJump = true;
								Icount = 1;
								Ncount = 1;
							}
							break;
						case OperandType.NI:
							if (p.literals[0] == IRSystem.momory.GetValue(p.operands[0]))
							{
								isJump = true;
								Icount = 1;
								Ncount = 1;
							}
							break;
						case OperandType.NN:
							if (p.literals[0] < p.literals[1])
							{
								isJump = true;
								Ncount = 2;
							}
							break;
					}

					if (isJump)
					{
						if (targetType == OperandType.I)
						{
							IRSystem.interpreter.instructionPointer = (int)IRSystem.momory.GetValue(p.operands[Icount]);
						}
						else if (targetType == OperandType.N)
						{
							IRSystem.interpreter.instructionPointer = (int)p.literals[Ncount];
						}
						else
						{
							return;
							//ERROR
						}
						//IRSystem.interpreter.instructionPointer -= 2;
					}
					return;

				case InstructionType.JG:
					targetType = p.operandType & (OperandType)3;
					sourceType = OperandType.NULL;
					Icount = 0;
					Ncount = 0;
					isJump = false;
					if (targetType == OperandType.NULL)
					{
						switch (p.operandType)
						{
							case OperandType.II:
								targetType = OperandType.I;
								sourceType = OperandType.I;
								break;
							case OperandType.NI:
								targetType = OperandType.I;
								sourceType = OperandType.N;
								break;
							case OperandType.IN:
								targetType = OperandType.N;
								sourceType = OperandType.I;
								break;
							case OperandType.NN:
								targetType = OperandType.N;
								sourceType = OperandType.N;
								break;
							default:
								//ERROR
								break;
						}
					}
					else
					{
						sourceType = p.operandType & (OperandType)28;
					}

					switch (sourceType)
					{
						case OperandType.I:
							if (IRSystem.momory.GetValue(p.operands[0]) > 0)
							{
								isJump = true;
								Icount = 1;
							}
							break;
						case OperandType.N:
							if (p.literals[0] > 0)
							{
								isJump = true;
								Ncount = 1;
							}
							break;
						case OperandType.II:
							if (IRSystem.momory.GetValue(p.operands[0]) > IRSystem.momory.GetValue(p.operands[1]))
							{
								isJump = true;
								Icount = 2;
							}
							break;
						case OperandType.IN:
							if (IRSystem.momory.GetValue(p.operands[0]) > p.literals[0])
							{
								isJump = true;
								Icount = 1;
								Ncount = 1;
							}
							break;
						case OperandType.NI:
							if (p.literals[0] > IRSystem.momory.GetValue(p.operands[0]))
							{
								isJump = true;
								Icount = 1;
								Ncount = 1;
							}
							break;
						case OperandType.NN:
							if (p.literals[0] > p.literals[1])
							{
								isJump = true;
								Ncount = 2;
							}
							break;
					}

					if (isJump)
					{
						if (targetType == OperandType.I)
						{
							IRSystem.interpreter.instructionPointer = (int)IRSystem.momory.GetValue(p.operands[Icount]);
						}
						else if (targetType == OperandType.N)
						{
							IRSystem.interpreter.instructionPointer = (int)p.literals[Ncount];
						}
						else
						{
							return;
							//ERROR
						}
						//IRSystem.interpreter.instructionPointer -= 2;
					}
					return;
				case InstructionType.NOP:
					return;
				case InstructionType.HLT:
					IRSystem.isRunning = false;
					IRSystem.Pause();
					return;
			}
		}
	}
}
