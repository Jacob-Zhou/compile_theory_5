using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_5.Model.IRSystem
{
	class IRMomoryUnit : INotifyPropertyChanged
	{
		public IRMomoryUnit(string name, double value)
		{
			this.name = name;
			this.value = value;
		}

		private string name;
		private double value;

		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				name = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Name"));
			}
		}

		public double Value
		{
			get
			{
				return value;
			}

			set
			{
				this.value = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Value"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}
	}
}
