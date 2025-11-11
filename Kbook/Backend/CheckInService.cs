using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Kbook.Database;
using Kbook.Models;

namespace Kbook.Backend
{
    /// <summary>
    /// Handles check-in operations
    /// </summary>
    public class CheckInService
    {
        /// <summary>
        /// Gets all reservations with status "Booked" (ready for check-in)
        /// </summary>
        public static List<Reservation> GetPendingCheckIns()
        {
            List<Reservation> reservations = new List<Reservation>();
            string query = @"SELECT r.reservation_id, r.guest_id, r.room_id, r.check_in, r.check_out, r.status,
                            g.name AS guest_name, rm.room_number
                            FROM reservations r
                            INNER JOIN guests g ON r.guest_id = g.guest_id
                            INNER JOIN rooms rm ON r.room_id = rm.room_id
                            WHERE r.status = 'Booked'
                            ORDER BY r.check_in ASC";

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
                throw new Exception($"Error retrieving pending check-ins: {ex.Message}");
            }

            return reservations;
        }

        /// <summary>
        /// Processes check-in for a reservation
        /// </summary>
        public static bool ProcessCheckIn(int reservationId, int roomId)
        {
            try
            {
                // Update reservation status to CheckedIn
                string query = "UPDATE reservations SET status = 'CheckedIn' WHERE reservation_id = @reservationId";

                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@reservationId", reservationId)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    // Ensure room status is Occupied
                    RoomService.UpdateRoomStatus(roomId, "Occupied");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing check-in: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all currently checked-in reservations
        /// </summary>
        public static List<Reservation> GetCheckedInReservations()
        {
            List<Reservation> reservations = new List<Reservation>();
            string query = @"SELECT r.reservation_id, r.guest_id, r.room_id, r.check_in, r.check_out, r.status,
                            g.name AS guest_name, rm.room_number
                            FROM reservations r
                            INNER JOIN guests g ON r.guest_id = g.guest_id
                            INNER JOIN rooms rm ON r.room_id = rm.room_id
                            WHERE r.status = 'CheckedIn'
                            ORDER BY r.check_out ASC";

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
                throw new Exception($"Error retrieving checked-in reservations: {ex.Message}");
            }

            return reservations;
        }
    }
}