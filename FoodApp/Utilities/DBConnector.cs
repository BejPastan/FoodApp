using Microsoft.Data.SqlClient;
using Dapper;

namespace FoodApp.Utilities
{
    /// <summary>
    /// Utility class for managing database connections and executing queries using Dapper.
    /// </summary>
    public class DBConnector
    {
        //private static SqlConnection? _connection;
        private static string _connectionString = "";


        /// <summary>
        /// method for setting the connection string for the database.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentException"></exception>
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
            SqlConnection connection = new(_connectionString);
            connection.Open();
            Console.WriteLine(connection.State);
            return connection;
        }

        /// <summary>
        /// Sendingquery to database and return result as list of T, if error occurs return empty list and log the error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<T> QueryDatabase<T>(string sql, object? parameters = null)
        {
            SqlConnection connection = Open();
            try
            {
                return connection.Query<T>(sql, parameters);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL error ({ex.Number}): {ex.Message}");
                return [];
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public static IEnumerable<T> QueryNested<T, T2>(string sql, string[] column, object? parameters = null)
        {
            SqlConnection connection = Open();
            try
            {

                return [];
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
