using System;
using MySql.Data.MySqlClient;
using Kbook.Database;
using Kbook.Models;

namespace Kbook.Backend
{
    /// <summary>
    /// Handles staff authentication only
    /// </summary>
    public class AuthenticationService
    {
        /// <summary>
        /// Authenticates a staff user and returns the result
        /// </summary>
        public static AuthenticationResult Authenticate(string username, string password)
        {
            AuthenticationResult result = new AuthenticationResult();

            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                result.ErrorMessage = "Username and password cannot be empty.";
                return result;
            }

            try
            {
                // Authenticate from admins table
                string query = "SELECT admin_id, username, full_name FROM admins WHERE username = @username AND password = @password";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@password", password)
                };

                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        result.IsSuccessful = true;
                        result.UserId = reader.GetInt32("admin_id");
                        result.Username = reader.IsDBNull(reader.GetOrdinal("full_name"))
                            ? reader.GetString("username")
                            : reader.GetString("full_name");
                    }
                    else
                    {
                        result.ErrorMessage = "Invalid username or password.";
                    }
                }
            }
            catch (MySqlException ex)
            {
                result.ErrorMessage = $"Database error: {ex.Message}\n\nPlease ensure:\n" +
                    "1. XAMPP MySQL service is running\n" +
                    "2. Database 'kbook_db' exists\n" +
                    "3. Admins table is created";
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Retrieves staff user details by ID
        /// </summary>
        public static StaffUser GetStaffById(int adminId)
        {
            string query = "SELECT admin_id, username, full_name FROM admins WHERE admin_id = @adminId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@adminId", adminId)
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        return new StaffUser
                        {
                            UserId = reader.GetInt32("admin_id"),
                            Username = reader.GetString("username"),
                            FullName = reader.IsDBNull(reader.GetOrdinal("full_name"))
                                ? reader.GetString("username")
                                : reader.GetString("full_name")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving staff user: {ex.Message}");
            }

            return null;
        }
    }
}