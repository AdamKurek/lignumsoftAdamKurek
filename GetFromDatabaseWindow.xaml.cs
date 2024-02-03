using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace lignumsoftAdamKurek
{
    public partial class GetFromDatabaseWindow : Window
    {
        public ObservableCollection<List<string>> list; 
        public GetFromDatabaseWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            list = DataBaseConnection.GetFromDatabase(stringTextBox.Text);
        }

    }
}