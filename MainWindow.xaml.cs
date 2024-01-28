using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lignumsoftAdamKurek
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private List<List<string>> csvData;
        public event PropertyChangedEventHandler? PropertyChanged;

        private int cols;
        private void LoadCsvData(string filePath)
        {
            csvData = new List<List<string>>();
            int maxCols = 0;
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    List<string> row = parser.ReadFields().ToList();
                    csvData.Add(row);
                    if (row.Count > maxCols)
                    {
                        maxCols = row.Count;
                    }
                }
            }
            AdjustListLengths(csvData, maxCols);

            csvDataGrid.Columns.Clear();
            for (int colIndex = 0; colIndex < maxCols; colIndex++)
            {
                csvDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = $"Column {colIndex + 1}",
                    Binding = new System.Windows.Data.Binding($"[{colIndex}]"),
                    IsReadOnly = false,
                });
            }
            cols = maxCols;
            csvDataGrid.ItemsSource = csvData;
        }
       
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Wybierz plik";
            openFileDialog.Filter = "Pliki CSV (*.csv)|*.csv|Wszystkie pliki (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                LoadCsvData(selectedFilePath);
            }
        }

        private void AddColumn(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                csvData[i].Add(String.Empty);
            }
            csvDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = $"Column {cols + 1}",
                Binding = new System.Windows.Data.Binding($"[{cols++}]"),
                IsReadOnly = false,
            });
        }

        private void RemoveColumn(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                csvData[i].RemoveAt(csvData[i].Count - 1);
            }
            csvDataGrid.Columns.Remove(csvDataGrid.Columns[csvDataGrid.Columns.Count-1]);
            
            //csvDataGrid.Columns.RemoveAt(SelectedColumn--);
            //csvDataGrid.Columns[SelectedColumn].Header = "xd";
            cols--;
        }

        static void AdjustListLengths(List<List<string>> csvData, int columns)
        {
            foreach (var innerList in csvData)
            {
                if (innerList.Count != columns)
                {
                    if (innerList.Count < columns)
                    {
                        innerList.AddRange(Enumerable.Repeat(string.Empty, columns - innerList.Count));
                    }
                }
            }
        }
    }
}