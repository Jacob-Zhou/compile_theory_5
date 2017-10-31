using compile_theory_5.Model;
using compile_theory_5.Model.IRSystem;
using compile_theory_5.ViewModel;
using ICSharpCode.AvalonEdit.Editing;
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

namespace compile_theory_5
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
			SourceViewModel.Init(sourceEditor);
			ErrorViewModel.getInstance().Init(errorDataGrid, closeErrorButton);

			IRSystem.Init(textEditor, VariableDataGrid, textBox);

			CommandBinding binding = new CommandBinding(ApplicationCommands.Open);
			binding.Executed += Binding_Open_Executed;
			CommandBindings.Add(binding);
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
			///TO DO
			//SourceViewModel.SourceData = File.ReadAllBytes(path);
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
				if (item.isVisable)
				{
					textEditor.Select(textEditor.Document.GetOffset(item.line, item.lineOffset), item.length);
					textEditor.TextArea.SelectionBrush = Brushes.Red;
					textEditor.ScrollTo(item.line, item.lineOffset);
				}
				else
				{
					textEditor.Select(0, 0);
				}
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

		private void SaveCommands_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.DefaultExt = "txt";
			if (sfd.ShowDialog().Value == true)
			{
				textEditor.Save(sfd.FileName);
			}
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			Start();
			IRSystem.Start();
		}

		private void Start()
		{
			StartButton.IsEnabled = false;
			OneStepButton.IsEnabled = false;
			PauseButton.IsEnabled = true;
			StopButton.IsEnabled = true;
			StartOverButton.IsEnabled = true;
		}

		private void OneStepButton_Click(object sender, RoutedEventArgs e)
		{
			PauseButton.IsEnabled = false;
			StopButton.IsEnabled = true;
			StartOverButton.IsEnabled = true;
			IRSystem.OneStep();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			StartButton.IsEnabled = true;
			OneStepButton.IsEnabled = true;
			IRSystem.Pause();
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			Stop();
			IRSystem.Stop();
		}

		private void Stop()
		{
			StartButton.IsEnabled = true;
			OneStepButton.IsEnabled = true;
			PauseButton.IsEnabled = false;
			StopButton.IsEnabled = false;
			StartOverButton.IsEnabled = true;
			textEditor.Select(0, 0);
		}

		private void StartOverButton_Click(object sender, RoutedEventArgs e)
		{
			IRSystem.Stop();
			Start();
			IRSystem.Start();
		}

		private void SpanTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int i;
			if (int.TryParse(((TextBox)sender).Text, out i))
			{
				if(i != IRSystem.clock.Interval)
				{
					IRSystem.changeInterval(i);
				}
			}
			else
			{
				((TextBox)sender).Text = IRSystem.clock.Interval.ToString();
			}
		}

		private void OneStepButton_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (OneStepButton.IsEnabled && e.Delta < 0)
			{
				OneStepButton_Click(sender, null);
			}
		}

		private void parseButton_Click(object sender, RoutedEventArgs e)
		{
			Parser.parse(ToIRMode);
		}

		private void closeErrorButton_Click(object sender, RoutedEventArgs e)
		{
			SourceViewModel.ChangeMode();
		}

		private void sourceMode_Click(object sender, RoutedEventArgs e)
		{
			sourceModeMenuItem.IsChecked = true;
			IRModeMenuItem.IsChecked = false;
			sourceGrid.Visibility = Visibility.Visible;
			IRGrid.Visibility = Visibility.Hidden;
		}

		private void IRMode_Click(object sender, RoutedEventArgs e)
		{
			ToIRMode();
		}

		private void ToIRMode()
		{
			Stop();
			sourceModeMenuItem.IsChecked = false;
			IRModeMenuItem.IsChecked = true;
			sourceGrid.Visibility = Visibility.Hidden;
			IRGrid.Visibility = Visibility.Visible;
		}
	}
}
