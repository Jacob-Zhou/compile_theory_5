using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace compile_theory_5.Model.IRSystem.IRViewModel
{
	class IRMomoryViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<IRMomoryUnit> Momory = new ObservableCollection<IRMomoryUnit>();

		public void Init(DataGrid dataGrid)
		{
			dataGrid.ItemsSource = Momory;
		}

		public int Add(string name, double value = 0)
		{
			Momory.Add(new IRMomoryUnit(name, value));

			OnPropertyChanged(new PropertyChangedEventArgs("Momory"));
			return Momory.Count() - 1;
		}

		public void Reset()
		{
			Momory.Clear();
			OnPropertyChanged(new PropertyChangedEventArgs("Momory"));
		}

		public int? FindAddress(string name)
		{
			int i;
			if(TryFindAddress(name, out i))
			{
				return i;
			}
			else
			{
				return null;
			}
		}

		public bool TryFindAddress(string name, out int address)
		{
			address = 0;
			foreach (var u in Momory)
			{
				if (u.Name == name)
				{
					return true;
				}
				address++;
			}
			return false;
		}

		public double GetValue(int address)
		{
			double value;
			if(TryGetValue(address, out value))
			{
				return value;
			}
			else
			{
				throw new Exception("地址不合法");
			}
		}

		public bool TryGetValue(int address, out double value)
		{
			if (address < Momory.Count)
			{
				value = Momory[address].Value;
				return true;
			}
			else
			{
				value = 0;
				return false;
			}
		}

		public bool SetValue(int address, double value)
		{
			if (address < Momory.Count)
			{
				Momory[address].Value = value;
				return true;
			}
			return false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}
	}
}
