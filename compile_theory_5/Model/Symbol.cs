using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model
{
	enum SymbolType
	{
		terminal,
		nonterminal
	}

	partial class Symbol
	{
		public List<Symbol> childrens = new List<Symbol>();
		public SymbolType type;
		public SymbolKind kind;
		public List<string> values = new List<string>();

		public Symbol(SymbolType type, SymbolKind kind)
		{
			this.type = type;
			this.kind = kind;
		}

		public Symbol(SymbolType type, SymbolKind kind, string value)
		{
			this.type = type;
			this.kind = kind;
			this.values.Add(value);
		}
	}

}
