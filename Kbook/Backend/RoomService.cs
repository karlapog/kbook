using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Kbook.Database;
using Kbook.Models;

namespace Kbook.Backend
{
    /// <summary>
    /// Handles all room-related operations
    /// </summary>
    public class RoomService
    {
        /// <summary>
        /// Retrieves all rooms from the database
        /// </summary>
        public static List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();
            string query = "SELECT room_id, room_number, type, status, price FROM rooms ORDER BY room_number";

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query))
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomId = reader.GetInt32("room_id"),
                            RoomNumber = reader.IsDBNull(reader.GetOrdinal("room_number")) ? "" : reader.GetString("room_number"),
                            Type = reader.IsDBNull(reader.GetOrdinal("type")) ? "" : reader.GetString("type"),
                            Status = reader.IsDBNull(reader.GetOrdinal("status")) ? "Available" : reader.GetString("status"),
                            Price = reader.GetDecimal("price")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving rooms: {ex.Message}");
            }

            return rooms;
        }

        /// <summary>
        /// Retrieves a single room by ID
        /// </summary>
        public static Room GetRoomById(int roomId)
        {
            string query = "SELECT room_id, room_number, type, status, price FROM rooms WHERE room_id = @roomId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomId", roomId)
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        return new Room
                        {
                            RoomId = reader.GetInt32("room_id"),
                            RoomNumber = reader.IsDBNull(reader.GetOrdinal("room_number")) ? "" : reader.GetString("room_number"),
                            Type = reader.IsDBNull(reader.GetOrdinal("type")) ? "" : reader.GetString("type"),
                            Status = reader.IsDBNull(reader.GetOrdinal("status")) ? "Available" : reader.GetString("status"),
                            Price = reader.GetDecimal("price")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving room: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Adds a new room to the database
        /// </summary>
        public static bool AddRoom(Room room)
        {
            string query = "INSERT INTO rooms (room_number, type, status, price) VALUES (@roomNumber, @type, @status, @price)";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomNumber", room.RoomNumber ?? ""),
                new MySqlParameter("@type", room.Type ?? ""),
                new MySqlParameter("@status", room.Status ?? "Available"),
                new MySqlParameter("@price", room.Price)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding room: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing room's information
        /// </summary>
        public static bool UpdateRoom(Room room)
        {
            string query = "UPDATE rooms SET room_number = @roomNumber, type = @type, status = @status, price = @price WHERE room_id = @roomId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomId", room.RoomId),
                new MySqlParameter("@roomNumber", room.RoomNumber ?? ""),
                new MySqlParameter("@type", room.Type ?? ""),
                new MySqlParameter("@status", room.Status ?? "Available"),
                new MySqlParameter("@price", room.Price)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating room: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates only the room status
        /// </summary>
        public static bool UpdateRoomStatus(int roomId, string status)
        {
            string query = "UPDATE rooms SET status = @status WHERE room_id = @roomId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomId", roomId),
                new MySqlParameter("@status", status)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating room status: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a room from the database
        /// </summary>
        public static bool DeleteRoom(int roomId)
        {
            string query = "DELETE FROM rooms WHERE room_id = @roomId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomId", roomId)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting room: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all available rooms
        /// </summary>
        public static List<Room> GetAvailableRooms()
        {
            List<Room> rooms = new List<Room>();
            string query = "SELECT room_id, room_number, type, status, price FROM rooms WHERE status = 'Available' ORDER BY room_number";

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query))
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomId = reader.GetInt32("room_id"),
                            RoomNumber = reader.IsDBNull(reader.GetOrdinal("room_number")) ? "" : reader.GetString("room_number"),
                            Type = reader.IsDBNull(reader.GetOrdinal("type")) ? "" : reader.GetString("type"),
                            Status = reader.GetString("status"),
                            Price = reader.GetDecimal("price")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving available rooms: {ex.Message}");
            }

            return rooms;
        }

        /// <summary>
        /// Checks if a room number already exists
        /// </summary>
        public static bool RoomNumberExists(string roomNumber, int excludeRoomId = 0)
        {
            string query = "SELECT COUNT(*) FROM rooms WHERE room_number = @roomNumber AND room_id != @excludeRoomId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomNumber", roomNumber),
                new MySqlParameter("@excludeRoomId", excludeRoomId)
            };

            try
            {
                object result = DatabaseHelper.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking room number: {ex.Message}");
            }
        }
    }
}