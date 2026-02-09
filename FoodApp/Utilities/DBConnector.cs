using Microsoft.Data.SqlClient;
using Dapper;

namespace FoodApp.Utilities
{
    public class DBConnector
    {
        //private static SqlConnection? _connection;
        private static string _connectionString ="";


        public static void SetConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string must not be null or whitespace.", nameof(connectionString));
            _connectionString = connectionString;
        }

        /// <summary>
        /// Synchronously opens the database connection.
        /// </summary>
        public static SqlConnection Open()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            Console.WriteLine(connection.State);
            return connection;
        }

        //send query using Dapper
        public static IEnumerable<T> QueryDatabase<T>(string sql, object? parameters = null)
        {
            SqlConnection connection = Open();
            try
            {
                Console.WriteLine($"Executing Dapper query. {sql}");
                return connection.Query<T>(sql, parameters);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL error ({ex.Number}): {ex.Message}");
                return Enumerable.Empty<T>();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
