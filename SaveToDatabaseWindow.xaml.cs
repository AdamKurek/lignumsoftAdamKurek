using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace lignumsoftAdamKurek
{
    public partial class SaveToDatabaseWindow : Window
    {
        private ObservableCollection<List<string>> fields;
        private int cols;


        public SaveToDatabaseWindow(ObservableCollection<List<string>> fields, int cols, string Table)
        {
            InitializeComponent();
            stringTextBox.Text = Table;
            this.fields = fields;
            this.cols = cols;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = stringTextBox.Text;
            DataBaseConnection.PushToDatabase(stringTextBox.Text, fields, cols);
        }

    }
}