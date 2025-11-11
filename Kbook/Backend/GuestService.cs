using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Kbook.Database;
using Kbook.Models;

namespace Kbook.Backend
{
    /// <summary>
    /// Handles all guest-related operations
    /// </summary>
    public class GuestService
    {
        /// <summary>
        /// Retrieves all guests from the database
        /// </summary>
        public static List<Guest> GetAllGuests()
        {
            List<Guest> guests = new List<Guest>();
            string query = "SELECT guest_id, name, contact_number, email FROM guests ORDER BY name";

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query))
                {
                    while (reader.Read())
                    {
                        guests.Add(new Guest
                        {
                            GuestId = reader.GetInt32("guest_id"),
                            Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString("name"),
                            ContactNumber = reader.IsDBNull(reader.GetOrdinal("contact_number")) ? "" : reader.GetString("contact_number"),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving guests: {ex.Message}");
            }

            return guests;
        }

        /// <summary>
        /// Retrieves a single guest by ID
        /// </summary>
        public static Guest GetGuestById(int guestId)
        {
            string query = "SELECT guest_id, name, contact_number, email FROM guests WHERE guest_id = @guestId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@guestId", guestId)
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        return new Guest
                        {
                            GuestId = reader.GetInt32("guest_id"),
                            Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString("name"),
                            ContactNumber = reader.IsDBNull(reader.GetOrdinal("contact_number")) ? "" : reader.GetString("contact_number"),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving guest: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Adds a new guest to the database
        /// </summary>
        public static bool AddGuest(Guest guest)
        {
            string query = "INSERT INTO guests (name, contact_number, email) VALUES (@name, @contactNumber, @email)";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@name", guest.Name ?? ""),
                new MySqlParameter("@contactNumber", guest.ContactNumber ?? ""),
                new MySqlParameter("@email", guest.Email ?? "")
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding guest: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing guest's information
        /// </summary>
        public static bool UpdateGuest(Guest guest)
        {
            string query = "UPDATE guests SET name = @name, contact_number = @contactNumber, email = @email WHERE guest_id = @guestId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@guestId", guest.GuestId),
                new MySqlParameter("@name", guest.Name ?? ""),
                new MySqlParameter("@contactNumber", guest.ContactNumber ?? ""),
                new MySqlParameter("@email", guest.Email ?? "")
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating guest: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a guest from the database
        /// </summary>
        public static bool DeleteGuest(int guestId)
        {
            string query = "DELETE FROM guests WHERE guest_id = @guestId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@guestId", guestId)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting guest: {ex.Message}");
            }
        }

        /// <summary>
        /// Searches guests by name, contact, or email
        /// </summary>
        public static List<Guest> SearchGuests(string searchTerm)
        {
            List<Guest> guests = new List<Guest>();
            string query = @"SELECT guest_id, name, contact_number, email FROM guests 
                           WHERE name LIKE @search OR contact_number LIKE @search OR email LIKE @search
                           ORDER BY name";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@search", $"%{searchTerm}%")
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    while (reader.Read())
                    {
                        guests.Add(new Guest
                        {
                            GuestId = reader.GetInt32("guest_id"),
                            Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString("name"),
                            ContactNumber = reader.IsDBNull(reader.GetOrdinal("contact_number")) ? "" : reader.GetString("contact_number"),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching guests: {ex.Message}");
            }

            return guests;
        }
    }
}