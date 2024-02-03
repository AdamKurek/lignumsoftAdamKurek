using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace lignumsoftAdamKurek
{
    public partial class CreateTableWindow : Window
    {
        public CreateTableWindow()
        {
            InitializeComponent();
        }

        public string NewTableName { get; private set; } = "";

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(numberTextBox.Text, out int columns))
            {
                try { 
                    DataBaseConnection.CreateTable(stringTextBox.Text, columns);
                    this.Close();
                    NewTableName = stringTextBox.Text;
                    MessageBox.Show($"Database {stringTextBox.Text} Created Sucessfully");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating table: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Invalid column number input. Please enter a valid number.");
            }
        }
        
    }
}