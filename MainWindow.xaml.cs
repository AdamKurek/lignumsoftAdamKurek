using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
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
            csvDataGrid.InitializingNewItem += CsvDataGrid_InitializingNewItem;
            csvDataGrid.ItemsSource = csvData;
            AddColumn(null, null);
        }

        private void CsvDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            AdjustListLength(csvData.Last(), cols);
        }

        private ObservableCollection<List<string>> csvData = new ObservableCollection<List<string>>() { new List<string>() {new("") } };
        public event PropertyChangedEventHandler? PropertyChanged;

        private int cols;
        private void LoadCsvData(string filePath)
        {
            csvData = new ObservableCollection<List<string>>();
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
            csvData.Add(new List<string>());
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
            try {
                cols--;
                for (int i = 0; i < csvData.Count; i++)
                {
                    csvData[i].RemoveAt(csvData[i].Count - 1);
                }
                csvDataGrid.Columns.Remove(csvDataGrid.Columns[csvDataGrid.Columns.Count-1]);
                //csvDataGrid.Columns.RemoveAt(SelectedColumn--);
                //csvDataGrid.Columns[SelectedColumn].Header = "xd";
            }
            catch {
            }
        }

      

        static void AdjustListLengths(IEnumerable<List<string>> csvData, int columns)
        {
            foreach (var innerList in csvData)
            {
                AdjustListLength(innerList, columns);
            }
        }

        static void AdjustListLength(List<string> csvData, int columns)
        {
            if (csvData.Count != columns)
            {
                if (csvData.Count < columns)
                {
                    csvData.AddRange(Enumerable.Repeat(string.Empty, columns - csvData.Count));
                }
            }
        }

    }
}