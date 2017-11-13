using compile_theory_3.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_3.Model
{
	public class Variable : INotifyPropertyChanged
	{
		private string Name;
		private double Value;
		static private bool isSet = false;

		public Variable(string name)
		{
			Name = name;
		}

		public string name
		{
			get
			{
				return Name;
			}

			set
			{
				Name = value;
				OnPropertyChanged(new PropertyChangedEventArgs("name"));
			}
		}

		public double getValue()
		{
			return Value;
		}

		public string value
		{
			get
			{
				return Value.ToString();
			}

			set
			{
				if(!double.TryParse(value, out Value))
				{
					Value = 0;
					isSet = false;
				}
				else
				{
					isSet = true;
				}
				OnPropertyChanged(new PropertyChangedEventArgs("value"));
			}
		}

		public static bool IsSet
		{
			get
			{
				return isSet;
			}

			set
			{
				isSet = value;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}
	}
}
