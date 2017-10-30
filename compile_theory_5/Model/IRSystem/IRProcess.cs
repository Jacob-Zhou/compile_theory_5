using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem
{
	enum InstructionType
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
		HLT
	}

	enum OperandType
	{
		I = 1,		//00000001
		N = 2,		//00000010
		NN = 16,	//00010000
		NI = 20,	//00010100
		IN = 24,	//00011000
		II = 28,	//00011100
		NULL = 0,
	}

	class IRProcess
	{
		public IRProcess(InstructionType type, int operandCount, OperandType operandType, List<int> operands, List<double> literals)
		{
			this.type = type;
			this.operandCount = operandCount;
			this.operands = operands;
			this.operandType = operandType;
			this.literals = literals;
		}

		public IRProcess(InstructionType type, int operandCount)
		{
			this.type = type;
			this.operandCount = operandCount;
		}

		public InstructionType type { get; set; }
		public int operandCount { get; set; }
		public List<int> operands { get; set; }
		public OperandType operandType { get; set; }
		public List<double> literals { get; set; }
	}
}
