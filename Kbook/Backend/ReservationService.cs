using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Kbook.Database;
using Kbook.Models;

namespace Kbook.Backend
{
    /// <summary>
    /// Handles all reservation-related operations
    /// </summary>
    public class ReservationService
    {
        /// <summary>
        /// Retrieves all reservations with guest and room details
        /// </summary>
        public static List<Reservation> GetAllReservations()
        {
            List<Reservation> reservations = new List<Reservation>();
            string query = @"SELECT r.reservation_id, r.guest_id, r.room_id, r.check_in, r.check_out, r.status,
                            g.name AS guest_name, rm.room_number
                            FROM reservations r
                            INNER JOIN guests g ON r.guest_id = g.guest_id
                            INNER JOIN rooms rm ON r.room_id = rm.room_id
                            ORDER BY r.check_in DESC";

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query))
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            ReservationId = reader.GetInt32("reservation_id"),
                            GuestId = reader.GetInt32("guest_id"),
                            RoomId = reader.GetInt32("room_id"),
                            CheckIn = reader.GetDateTime("check_in"),
                            CheckOut = reader.GetDateTime("check_out"),
                            Status = reader.GetString("status"),
                            GuestName = reader.GetString("guest_name"),
                            RoomNumber = reader.GetString("room_number")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving reservations: {ex.Message}");
            }

            return reservations;
        }

        /// <summary>
        /// Retrieves a single reservation by ID
        /// </summary>
        public static Reservation GetReservationById(int reservationId)
        {
            string query = @"SELECT r.reservation_id, r.guest_id, r.room_id, r.check_in, r.check_out, r.status,
                            g.name AS guest_name, rm.room_number
                            FROM reservations r
                            INNER JOIN guests g ON r.guest_id = g.guest_id
                            INNER JOIN rooms rm ON r.room_id = rm.room_id
                            WHERE r.reservation_id = @reservationId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@reservationId", reservationId)
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        return new Reservation
                        {
                            ReservationId = reader.GetInt32("reservation_id"),
                            GuestId = reader.GetInt32("guest_id"),
                            RoomId = reader.GetInt32("room_id"),
                            CheckIn = reader.GetDateTime("check_in"),
                            CheckOut = reader.GetDateTime("check_out"),
                            Status = reader.GetString("status"),
                            GuestName = reader.GetString("guest_name"),
                            RoomNumber = reader.GetString("room_number")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving reservation: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Creates a new reservation
        /// </summary>
        public static bool CreateReservation(int guestId, int roomId, DateTime checkIn, DateTime checkOut)
        {
            string query = "INSERT INTO reservations (guest_id, room_id, check_in, check_out, status) VALUES (@guestId, @roomId, @checkIn, @checkOut, 'Booked')";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@guestId", guestId),
                new MySqlParameter("@roomId", roomId),
                new MySqlParameter("@checkIn", checkIn.Date),
                new MySqlParameter("@checkOut", checkOut.Date)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);

                // Update room status to Occupied
                if (rowsAffected > 0)
                {
                    RoomService.UpdateRoomStatus(roomId, "Occupied");
                }

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating reservation: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing reservation
        /// </summary>
        public static bool UpdateReservation(int reservationId, int guestId, int roomId, DateTime checkIn, DateTime checkOut, string status)
        {
            string query = "UPDATE reservations SET guest_id = @guestId, room_id = @roomId, check_in = @checkIn, check_out = @checkOut, status = @status WHERE reservation_id = @reservationId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@reservationId", reservationId),
                new MySqlParameter("@guestId", guestId),
                new MySqlParameter("@roomId", roomId),
                new MySqlParameter("@checkIn", checkIn.Date),
                new MySqlParameter("@checkOut", checkOut.Date),
                new MySqlParameter("@status", status)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating reservation: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels a reservation
        /// </summary>
        public static bool CancelReservation(int reservationId, int roomId)
        {
            string query = "UPDATE reservations SET status = 'Cancelled' WHERE reservation_id = @reservationId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@reservationId", reservationId)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);

                // Set room back to Available
                if (rowsAffected > 0)
                {
                    RoomService.UpdateRoomStatus(roomId, "Available");
                }

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error cancelling reservation: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a reservation (hard delete)
        /// </summary>
        public static bool DeleteReservation(int reservationId)
        {
            string query = "DELETE FROM reservations WHERE reservation_id = @reservationId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@reservationId", reservationId)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting reservation: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets reservations by status
        /// </summary>
        public static List<Reservation> GetReservationsByStatus(string status)
        {
            List<Reservation> reservations = new List<Reservation>();
            string query = @"SELECT r.reservation_id, r.guest_id, r.room_id, r.check_in, r.check_out, r.status,
                            g.name AS guest_name, rm.room_number
                            FROM reservations r
                            INNER JOIN guests g ON r.guest_id = g.guest_id
                            INNER JOIN rooms rm ON r.room_id = rm.room_id
                            WHERE r.status = @status
                            ORDER BY r.check_in DESC";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@status", status)
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            ReservationId = reader.GetInt32("reservation_id"),
                            GuestId = reader.GetInt32("guest_id"),
                            RoomId = reader.GetInt32("room_id"),
                            CheckIn = reader.GetDateTime("check_in"),
                            CheckOut = reader.GetDateTime("check_out"),
                            Status = reader.GetString("status"),
                            GuestName = reader.GetString("guest_name"),
                            RoomNumber = reader.GetString("room_number")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving reservations by status: {ex.Message}");
            }

            return reservations;
        }

        /// <summary>
        /// Checks if a room is available for the given dates
        /// </summary>
        public static bool IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, int excludeReservationId = 0)
        {
            string query = @"SELECT COUNT(*) FROM reservations 
                           WHERE room_id = @roomId 
                           AND reservation_id != @excludeReservationId
                           AND status NOT IN ('Cancelled', 'CheckedOut')
                           AND (
                               (check_in <= @checkIn AND check_out > @checkIn) OR
                               (check_in < @checkOut AND check_out >= @checkOut) OR
                               (check_in >= @checkIn AND check_out <= @checkOut)
                           )";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@roomId", roomId),
                new MySqlParameter("@checkIn", checkIn.Date),
                new MySqlParameter("@checkOut", checkOut.Date),
                new MySqlParameter("@excludeReservationId", excludeReservationId)
            };

            try
            {
                object result = DatabaseHelper.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result) == 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking room availability: {ex.Message}");
            }
        }
    }
}