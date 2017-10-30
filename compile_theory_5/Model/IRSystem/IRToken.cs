using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem
{
	class IRToken
	{
		public IRToken(IRTokenKind kind, string value = "", int offset = 0)
		{
			this.offset = offset;
			this.value = value;
			this.kind = kind;
		}

		public int offset { get; set; }
		public string value { get; set; }
		public IRTokenKind kind { get; set; }
	}
}
