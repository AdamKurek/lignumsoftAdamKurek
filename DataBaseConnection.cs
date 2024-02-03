using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Text;

namespace lignumsoftAdamKurek
{
    internal class DataBaseConnection
    {
        static string databaseName = "DB3";
        static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";
        public static void PushToDatabase(string Table, IEnumerable<IEnumerable<string>> values, int rows)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (IEnumerable<string> row in values)
                {
                    string commandString = $"INSERT INTO {Table} (";
                    SqlCommand cmd = new SqlCommand("",connection);

                    for (int i = 0; i < rows; i++)
                    {
                        cmd.Parameters.AddWithValue("@Value" + (i + 1), row.ElementAt(i));
                        commandString += $"column{i + 1}, ";
                    }
                    commandString = commandString.Substring(0,commandString.Length - 2) + " VALUES (";
                    for (int i = 0; i < rows; i++)
                    {
                        commandString += $"@Value{i + 1}, ";
                    }
                    commandString = commandString.Substring(0, commandString.Length - 2) + ")";
                    cmd.CommandText = commandString;

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        

        public static ObservableCollection<List<string>> GetFromDatabase(string table)
        {
            ObservableCollection<List<string>> result = new ObservableCollection<List<string>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string commandString = $"SELECT * FROM {table}";
                SqlCommand cmd = new SqlCommand(commandString, connection);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> rowValues = new List<string>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rowValues.Add(reader[i].ToString());
                        }

                        result.Add(rowValues);
                    }
                }

                connection.Close();
            }

            return result;
        }


        public static void CreateTable(string name, int columns)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand($"CREATE TABLE {name} ({GenerateColumns(columns)})", connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating table: {ex.Message}");
                }
            }
        }

        private static string GenerateColumns(int columns)
        {
            StringBuilder columnsBuilder = new StringBuilder();

            for (int i = 1; i <= columns; i++)
            {
                columnsBuilder.Append($"Column{i} INT");
                if (i < columns)
                {
                    columnsBuilder.Append(", ");
                }
            }
            return columnsBuilder.ToString();
        }

    }
}
