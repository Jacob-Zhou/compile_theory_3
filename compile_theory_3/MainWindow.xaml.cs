using compile_theory_3.Model;
using compile_theory_3.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace compile_theory_3
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public Brush defaultBrush;
		public MainWindow()
		{
			InitializeComponent();
			SourceViewModel.Init(textEditor);
			ProcessViewModel.Init(treeView);
			StateViewModel.Init(textBox);
			ErrorViewModel.getInstance().Init(ErrorDataGrid);

			CommandBinding binding = new CommandBinding(ApplicationCommands.Open);
			binding.Executed += Binding_Open_Executed;
			CommandBindings.Add(binding);

			binding = new CommandBinding(ApplicationCommands.Properties);
			binding.Executed += Binding_Properties_Executed;
			CommandBindings.Add(binding);
		}

		private void Binding_Properties_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			(new PropertiesWindow()).ShowDialog();
		}

		private void Binding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog().Value == true)
			{
				OpenFile(ofd.FileName);
			}
		}

		private void OpenFile(string path)
		{
			SourceViewModel.SourceData = File.ReadAllBytes(path);
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			//Lexer.Test();
			Lexer.preLex();
			Parser.parse();
		}

		private void textEditor_Drop(object sender, DragEventArgs e)
		{
			OpenFile((e.Data.GetData(DataFormats.FileDrop) as string[])[0]);
		}

		private void ErrorDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (defaultBrush == null)
			{
				defaultBrush = textEditor.TextArea.SelectionBrush;
			}
			var item = ((sender as DataGrid).SelectedItem as Error);
			if (item != null)
			{
				textEditor.Select(textEditor.Document.GetOffset(item.line, item.lineOffset), item.length);
				textEditor.TextArea.SelectionBrush = Brushes.Red;
				textEditor.ScrollTo(item.line, item.lineOffset);
			}
		}

		private void ErrorDataGrid_GotFocus(object sender, RoutedEventArgs e)
		{
			ErrorDataGrid_SelectionChanged((sender as DataGrid), null);
		}

		private void ErrorDataGrid_LostFocus(object sender, RoutedEventArgs e)
		{
			textEditor.TextArea.SelectionBrush = defaultBrush;
			textEditor.Select(0, 0);
		}

		private void treeView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			ProcessViewModel.ChangeMode();
		}

		private void SaveCommands_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.DefaultExt = "txt";
			if (sfd.ShowDialog().Value == true)
			{
				textEditor.Save(sfd.FileName);
			}
		}
	}
}
