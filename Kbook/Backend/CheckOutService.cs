using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Kbook.Database;
using Kbook.Models;

namespace Kbook.Backend
{
    /// <summary>
    /// Handles check-out and payment operations
    /// </summary>
    public class CheckOutService
    {
        /// <summary>
        /// Gets all currently checked-in reservations (ready for check-out)
        /// </summary>
        public static List<Reservation> GetCheckedInReservations()
        {
            List<Reservation> reservations = new List<Reservation>();
            string query = @"SELECT r.reservation_id, r.guest_id, r.room_id, r.check_in, r.check_out, r.status,
                            g.name AS guest_name, rm.room_number, rm.price
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
                            RoomNumber = reader.GetString("room_number"),
                            TotalAmount = CalculateTotalAmount(reader.GetDateTime("check_in"), reader.GetDateTime("check_out"), reader.GetDecimal("price"))
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

        /// <summary>
        /// Calculates total amount for a reservation
        /// </summary>
        private static decimal CalculateTotalAmount(DateTime checkIn, DateTime checkOut, decimal pricePerNight)
        {
            int nights = (checkOut - checkIn).Days;
            return nights * pricePerNight;
        }

        /// <summary>
        /// Processes check-out with payment
        /// </summary>
        public static bool ProcessCheckOut(int reservationId, int roomId, decimal amount, string paymentMethod)
        {
            try
            {
                // Update reservation status to CheckedOut
                string updateReservationQuery = "UPDATE reservations SET status = 'CheckedOut' WHERE reservation_id = @reservationId";

                MySqlParameter[] reservationParams = new MySqlParameter[]
                {
                    new MySqlParameter("@reservationId", reservationId)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(updateReservationQuery, reservationParams);

                if (rowsAffected > 0)
                {
                    // Insert payment record
                    string insertPaymentQuery = "INSERT INTO payments (reservation_id, amount, method, payment_date, status) VALUES (@reservationId, @amount, @method, NOW(), 'Paid')";

                    MySqlParameter[] paymentParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@reservationId", reservationId),
                        new MySqlParameter("@amount", amount),
                        new MySqlParameter("@method", paymentMethod)
                    };

                    DatabaseHelper.ExecuteNonQuery(insertPaymentQuery, paymentParams);

                    // Update room status to Available
                    RoomService.UpdateRoomStatus(roomId, "Available");

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing check-out: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets payment history for a reservation
        /// </summary>
        public static List<Payment> GetPaymentsByReservation(int reservationId)
        {
            List<Payment> payments = new List<Payment>();
            string query = "SELECT payment_id, reservation_id, amount, method, payment_date, status FROM payments WHERE reservation_id = @reservationId";

            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@reservationId", reservationId)
            };

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.ExecuteReader(query, parameters))
                {
                    while (reader.Read())
                    {
                        payments.Add(new Payment
                        {
                            PaymentId = reader.GetInt32("payment_id"),
                            ReservationId = reader.GetInt32("reservation_id"),
                            Amount = reader.GetDecimal("amount"),
                            Method = reader.GetString("method"),
                            PaymentDate = reader.GetDateTime("payment_date")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payments: {ex.Message}");
            }

            return payments;
        }
    }
}