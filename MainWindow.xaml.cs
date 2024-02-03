using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace lignumsoftAdamKurek
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            csvDataGrid.ItemsSource = csvData;
            AddColumn(null, null);
        }

        private void CsvDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            AdjustListLength(csvData.Last(), cols + removedColumns.Count);
        }

        private ObservableCollection<List<string>> csvData = new ObservableCollection<List<string>>() { new List<string>()};

        private int cols;
        Stack<int> removedColumns = new();
        string NewestTable = "";

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
            int addingIndex;
            if(!removedColumns.TryPop(out addingIndex))
            {
                addingIndex = cols;
                foreach (List<string> v in csvData)
                {
                    v.Add(String.Empty);
                }
            }

            csvDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = $"Column {addingIndex + 1}",
                Binding = new System.Windows.Data.Binding($"[{addingIndex}]"),
                IsReadOnly = false,
            });
            cols++;
        }

        private void RemoveColumn(object sender, RoutedEventArgs e)
        {
            if (cols <= 0) { return; }
            cols--;
            DataGridTextColumn column = (DataGridTextColumn)csvDataGrid.ColumnFromDisplayIndex(csvDataGrid.Columns.Count - 1);
            Binding b = (Binding)column.Binding;
            int columnIndex = int.Parse(b.Path.Path.Substring(1, b.Path.Path.Length-2));
            csvDataGrid.Columns.Remove(column);

            foreach (List<string> v in csvData)
            {
                v[columnIndex] = string.Empty;
            }
            removedColumns.Push(columnIndex);
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

        //private void csvDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    while (csvData.Count > 1 && csvData.Last().All(string.IsNullOrEmpty))
        //    {
        //        csvData.RemoveAt(csvData.Count - 1);
        //    }
        //}

        private void CreateTable_Click(object sender, RoutedEventArgs e)
        {
            CreateTableWindow window = new();
            window.ShowDialog();
            NewestTable = window.NewTableName;
        }

        private void SaveToDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<int> columnOrder = new();
                foreach (var column in csvDataGrid.Columns)
                {
                    columnOrder.Add(column.DisplayIndex);
                }
                SaveToDatabaseWindow saveWnd = new(new ObservableCollection<List<string>>(csvData.OrderBy(list => columnOrder.Select(index => list[index]))), cols, NewestTable);
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetFromDatabase_Click(object sender, RoutedEventArgs e)
        {
            GetFromDatabaseWindow window = new GetFromDatabaseWindow();
            window.ShowDialog();
            csvDataGrid.Columns.Clear();
            csvData = window.list;
            try
            {
                cols = csvData.OrderByDescending(list => list.Count).FirstOrDefault()!.Count;
                AdjustListLengths(csvData,cols);
                for (int i = 0;i<cols ; i++)
                {
                    csvDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = $"Column {i + 1}",
                        Binding = new System.Windows.Data.Binding($"[{i}]"),
                        IsReadOnly = false,
                    });
                }
                csvDataGrid.ItemsSource = csvData;
            }catch(Exception ex)
            {
                MessageBox.Show("Table was Empty or corrupted");
            }
        }

        

    }
}