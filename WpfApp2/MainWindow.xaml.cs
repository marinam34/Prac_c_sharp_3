using ClassLibrary2;
using Microsoft.Win32;
using System;
using System.Windows;
using ViewModel;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUIServices
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);
            comboBox_Enum.ItemsSource = Enum.GetValues(typeof(ClassLibrary2.FRawEnum));
        }
        public void ReportError(string message)
        {
            errorBar.Text = message;
            errorBar.Visibility = Visibility.Visible;
        }

        public string? ReportSaveFile()
        {
            SaveFileDialog files = new SaveFileDialog();
            if (files.ShowDialog() == true)
            {
                return files.FileName;
            }
            return null;
        }
        public string? ReportOpenFile()
        {
            OpenFileDialog files = new OpenFileDialog();
            if (files.ShowDialog() == true)
            {
                return files.FileName;
            }
            return null;
        }
    }
}
