using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem.IRInterpreter
{
	class IRInterpreter
	{
		public int instructionPointer = 1;
		public void Execute()
		{
			IRSystem.source.ShowIP();
			IRSystem.cache.GetProcess(instructionPointer++).Run(); //IP+1, 取指令, 执行;
		}

		public void Reset()
		{
			instructionPointer = 1;
		}
	}
}
