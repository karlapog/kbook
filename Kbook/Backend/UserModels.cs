using System;

namespace Kbook.Models
{
    /// <summary>
    /// Represents a staff/admin user
    /// </summary>
    public class StaffUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
    }

    /// <summary>
    /// Result of authentication attempt
    /// </summary>
    public class AuthenticationResult
    {
        public bool IsSuccessful { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string ErrorMessage { get; set; }

        public AuthenticationResult()
        {
            IsSuccessful = false;
            UserId = 0;
            Username = string.Empty;
            ErrorMessage = string.Empty;
        }
    }

    /// <summary>
    /// Represents a guest record (managed by staff)
    /// </summary>
    public class Guest
    {
        public int GuestId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Represents a hotel room
    /// </summary>
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Represents a reservation
    /// </summary>
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; }

        // Navigation properties (for display)
        public string GuestName { get; set; }
        public string RoomNumber { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// Represents a payment record
    /// </summary>
    public class Payment
    {
        public int PaymentId { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}