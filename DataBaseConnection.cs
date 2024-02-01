using Microsoft.Data.SqlClient;

namespace lignumsoftAdamKurek
{
    internal class DataBaseConnection
    {
        static SqlConnection sqlConnection;
        static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";
        public static void PushToDatabase(IEnumerable<IEnumerable<string>> values)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (IEnumerable<string> row in values)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO DB3 (column1, column2, column3) VALUES (@Value1, @Value2, @Value3)", connection);
                    cmd.Parameters.AddWithValue("@Value1", row.ElementAt(0));
                    cmd.Parameters.AddWithValue("@Value2", row.ElementAt(1));
                    cmd.Parameters.AddWithValue("@Value3", row.ElementAt(2));
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
