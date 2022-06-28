using AlarmTableGenerator.Models;
using AlarmTableGenerator.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace AlarmTableGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            Height = SystemParameters.PrimaryScreenHeight * 0.9;
            Width = SystemParameters.PrimaryScreenWidth * 0.9;
            DataContext = _mainViewModel = new MainViewModel();
            StateChanged += MainWindowStateChangeRaised;
            _mainViewModel.FileParsedSuccessfully += _mainViewModel_FileParsedSuccessfully;
        }

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        public void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private void _mainViewModel_FileParsedSuccessfully(object sender, EventArgs e)
        {
            var latestFile = _mainViewModel.FileEntries[^1];
            ListViewTable.Items.Clear();
            ErrorsListView.Items.Clear();
            
            //should try to make this more generic and modular in the future
            //current implementation will require adding to this for each new file type
            //could allow each file type to have a "TryConvertAlarms" and "TryConvertWarnings" per the interface
            //then AlarmFile would fail the ones it shouldn't do, or pass the file type into the method
            if (latestFile.FileType == typeof(AlarmFile))
            {
                var alarmFile = (AlarmFile)latestFile;
                ListViewTable.ItemsSource = alarmFile.TryConvertEntries();

                ErrorsListView.ItemsSource = alarmFile.CompileErrors();
            }
        }
    }
}
