using System;
using MySql.Data.MySqlClient;

namespace Kbook.Database
{
    /// <summary>
    /// Handles all MySQL database connections and operations
    /// </summary>
    public class DatabaseHelper
    {
        private static readonly string connectionString = "server=localhost;uid=root;pwd=;database=kbook_db;";

        /// <summary>
        /// Gets a new MySQL connection
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create database connection: {ex.Message}");
            }
        }

        /// <summary>
        /// Tests if the database connection is working
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return conn.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes a scalar query and returns a single value
        /// </summary>
        public static object ExecuteScalar(string query, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string query, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a query and returns a data reader
        /// </summary>
        public static MySqlDataReader ExecuteReader(string query, MySqlParameter[] parameters = null)
        {
            MySqlConnection conn = GetConnection();
            conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
        }
    }
}