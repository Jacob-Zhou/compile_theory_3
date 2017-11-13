using compile_theory_3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace compile_theory_3
{
	/// <summary>
	/// SetIdValueView.xaml 的交互逻辑
	/// </summary>
	public partial class SetIdValueView : Window
	{
		private static bool isSet = false;

		public SetIdValueView()
		{
			InitializeComponent();
		}

		static public bool SetValue(ObservableCollection<Variable> vars)
		{
			SetIdValueView svv = new SetIdValueView();
			svv.dataGrid.ItemsSource = vars;
			svv.ShowDialog();
			return isSet;
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			if (!Variable.IsSet)
			{
				if (MessageBox.Show("没有进行赋值， 是否继续赋值？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					return;
				}
				else
				{
					isSet = true;
					Close();
				}
			}
			else
			{
				isSet = true;
				Close();
			}
		}
	}
}
