using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem
{
	class IRCache
	{
		private Dictionary<int, IRProcess> cache = new Dictionary<int, IRProcess>();

		public IRProcess GetProcess(int address)
		{
			if (!cache.ContainsKey(address))
			{
				writeCache(address, IRSystem.parser.ParseLine(address));
			}
			return cache[address];
		}

		private void writeCache(int address, IRProcess process)
		{
			cache[address] = IRSystem.parser.ParseLine(address);
			//TO DO
		}

		public void Reset()
		{
			cache.Clear();
		}
	}
}
