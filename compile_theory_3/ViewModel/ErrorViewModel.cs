using compile_theory_3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace compile_theory_3.ViewModel
{
	class ErrorViewModel
	{
		private static ErrorViewModel _singleton = new ErrorViewModel();
		private ObservableCollection<Error> errors = new ObservableCollection<Error>();
		private DataGrid errorDataGrid;

		private ErrorViewModel() { }

		static public ErrorViewModel getInstance()
		{
			return _singleton;
		}

		public void Init(DataGrid dGrid)
		{
			errorDataGrid = dGrid;
			errorDataGrid.ItemsSource = errors;
		}

		public void addError(Error error)
		{
			errors.Add(error);
			if(errorDataGrid != null)
			{
				errorDataGrid.Visibility = System.Windows.Visibility.Visible;
			}
		}

		public void clear()
		{
			errors.Clear();
			if (errorDataGrid != null)
			{
				errorDataGrid.Visibility = System.Windows.Visibility.Hidden;
			}
		}

		public Error getError(int index)
		{
			if(errors.Count > index)
			{
				return errors[index];
			}
			else
			{
				return null;
			}
		}
	}
}
