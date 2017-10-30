using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace compile_theory_5.Commands
{
	class SaveCommands
	{
		private static RoutedUICommand requery;

		static SaveCommands()
		{
			InputGestureCollection inputs = new InputGestureCollection();
			inputs.Add(new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl+S"));
			requery = new RoutedUICommand("Requary", "Requary", typeof(SaveCommands), inputs);
		}

		public static RoutedUICommand Requery
		{
			get
			{
				return requery;
			}
		}
	}
}
