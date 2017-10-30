using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace compile_theory_5.ViewModel
{
	class StateViewModel
	{
		private static TextBox textbox;

		public static void Init(TextBox stateTextbox)
		{
			textbox = stateTextbox;
		}

		public static void Display(string state)
		{
			if(textbox != null)
			{
				textbox.Text = state;
			}
		}
	}
}
