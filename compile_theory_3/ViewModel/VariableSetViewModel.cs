using compile_theory_3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace compile_theory_3.ViewModel
{
	class VariableSetViewModel
	{
		static private ObservableCollection<Variable> variables = new ObservableCollection<Variable>();
		static private bool isSetVariable = false;

		public static bool IsSetVariable
		{
			get
			{
				return isSetVariable;
			}

			set
			{
				isSetVariable = value;
			}
		}

		static public void AddVariable(Variable var)
		{
			variables.Add(var);
		}

		static public void AddVariable(string name)
		{
			variables.Add(new Variable(name));
		}

		static public void Reset()
		{
			variables.Clear();
			isSetVariable = false;
		}

		static public double? getValue(string name)
		{
			double dv;
			if(double.TryParse(name, out dv))
			{
				return dv;
			}
			else
			{
				foreach (var v in variables)
				{
					if (name == v.name)
					{
						return v.getValue();
					}
				}
				return null;
			}
		}

		static public void SetValue()
		{
			if (variables.Count > 0)
			{
				isSetVariable = SetIdValueView.SetValue(variables);
			}
			else
			{
				isSetVariable = true;
			}
		}
	}
}
