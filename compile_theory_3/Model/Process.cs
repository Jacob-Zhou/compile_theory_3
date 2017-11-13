using compile_theory_3.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_3.Model
{
	class Detail
	{
		public Detail(string kind, string production)
		{
			this.kind = kind;
			this.production = production;
		}

		public string kind { get; set; }
		public string production { get; set; }
	}

	class Process : INotifyPropertyChanged
	{
		public Process(string kind, string production, string value)
		{
			this.kind = kind;
			this.value = value;
			detail = new ObservableCollection<Process>();
			detail.Add(new Process("\\", production));
		}

		public Process(string value)
		{
			this.value = value;
		}

		public Process(string kind, string value)
		{
			this.kind = kind;
			this.value = value;
		}

		public void addDetail(string kind, string production, string value)
		{
			detail.Add(new Process(kind, production, value));
			OnPropertyChanged(new PropertyChangedEventArgs("detail"));
		}

		public void addDetail(string kind)
		{
			detail.Add(new Process(kind, ""));
			OnPropertyChanged(new PropertyChangedEventArgs("detail"));
		}

		public void addDetail(Process process)
		{
			detail.Add(process);
			OnPropertyChanged(new PropertyChangedEventArgs("detail"));
		}

		private bool IsExpanded;
		public bool isExpanded
		{
			get { return IsExpanded; }
			set
			{
				IsExpanded = value;
				if(detail != null)
				{
					for (int i = 0; i < detail.Count; ++i)
					{
						detail[i].isExpanded = ProcessViewModel.IsExpanded;
					}
				}
				OnPropertyChanged(new PropertyChangedEventArgs("isExpanded"));
			}
		}

		private string Kind;
		public string kind
		{
			get { return Kind; }
			set
			{
				Kind = value;
				OnPropertyChanged(new PropertyChangedEventArgs("kind"));
			}
		}

		private string Value;
		public string value
		{
			get { return Value; }
			set
			{
				Value = value;
				OnPropertyChanged(new PropertyChangedEventArgs("value"));
			}
		}

		private ObservableCollection<Process> Detail;
		public ObservableCollection<Process> detail
		{
			get { return Detail; }
			set
			{
				Detail = value;
				OnPropertyChanged(new PropertyChangedEventArgs("detail"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}
	}
}
