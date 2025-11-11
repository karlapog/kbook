using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Kbook.Backend;
using Kbook.Models;

namespace Kbook
{
    public partial class AddEditReservationWindow : Window
    {
        private Reservation currentReservation;
        private bool isEditMode;
        private List<Room> availableRooms;

        /// <summary>
        /// Constructor for Add mode
        /// </summary>
        public AddEditReservationWindow()
        {
            InitializeComponent();
            isEditMode = false;
            txtTitle.Text = "New Reservation";

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            LoadGuests();
            LoadAvailableRooms();

            // Set minimum check-in date to today
            dpCheckIn.DisplayDateStart = DateTime.Today;
            dpCheckOut.DisplayDateStart = DateTime.Today.AddDays(1);

            cmbGuest.Focus();
        }

        /// <summary>
        /// Constructor for Edit mode
        /// </summary>
        public AddEditReservationWindow(Reservation reservation)
        {
            InitializeComponent();
            isEditMode = true;
            currentReservation = reservation;
            txtTitle.Text = "Edit Reservation";
            btnSave.Content = "Update";

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            LoadGuests();
            LoadAvailableRooms();

            // Populate fields with existing data
            cmbGuest.SelectedValue = reservation.GuestId;
            dpCheckIn.SelectedDate = reservation.CheckIn;
            dpCheckOut.SelectedDate = reservation.CheckOut;

            // Select the room
            foreach (var item in cmbRoom.Items)
            {
                if (item is Room room && room.RoomId == reservation.RoomId)
                {
                    cmbRoom.SelectedItem = item;
                    break;
                }
            }

            cmbGuest.Focus();
        }

        /// <summary>
        /// Loads all guests into the combo box
        /// </summary>
        private void LoadGuests()
        {
            try
            {
                List<Guest> guests = GuestService.GetAllGuests();
                cmbGuest.ItemsSource = guests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading guests: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads available rooms into the combo box
        /// </summary>
        private void LoadAvailableRooms()
        {
            try
            {
                availableRooms = RoomService.GetAvailableRooms();

                // Create a custom display format for rooms
                var roomDisplay = availableRooms.Select(r => new
                {
                    Room = r,
                    Display = $"Room {r.RoomNumber} - {r.Type} (₱{r.Price:N2})"
                }).ToList();

                cmbRoom.ItemsSource = roomDisplay;
                cmbRoom.DisplayMemberPath = "Display";
                cmbRoom.SelectedValuePath = "Room";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Room selection changed - update price
        /// </summary>
        private void CmbRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRoom.SelectedItem != null)
            {
                dynamic selectedItem = cmbRoom.SelectedItem;
                Room room = selectedItem.Room;
                txtPricePerNight.Text = room.Price.ToString("N2");
                CalculateTotal();
            }
        }

        /// <summary>
        /// Date picker changed - recalculate total
        /// </summary>
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateTotal();
        }

        /// <summary>
        /// Calculates total nights and total amount
        /// </summary>
        private void CalculateTotal()
        {
            if (dpCheckIn.SelectedDate.HasValue && dpCheckOut.SelectedDate.HasValue && cmbRoom.SelectedItem != null)
            {
                DateTime checkIn = dpCheckIn.SelectedDate.Value;
                DateTime checkOut = dpCheckOut.SelectedDate.Value;

                if (checkOut > checkIn)
                {
                    int nights = (checkOut - checkIn).Days;
                    txtTotalNights.Text = nights.ToString();

                    dynamic selectedItem = cmbRoom.SelectedItem;
                    Room room = selectedItem.Room;
                    decimal total = nights * room.Price;
                    txtTotalAmount.Text = total.ToString("N2");
                }
                else
                {
                    txtTotalNights.Text = "0";
                    txtTotalAmount.Text = "0.00";
                }
            }
        }

        /// <summary>
        /// Save button click handler
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate guest selection
            if (cmbGuest.SelectedItem == null)
            {
                MessageBox.Show("Please select a guest.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cmbGuest.Focus();
                return;
            }

            // Validate room selection
            if (cmbRoom.SelectedItem == null)
            {
                MessageBox.Show("Please select a room.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cmbRoom.Focus();
                return;
            }

            // Validate check-in date
            if (!dpCheckIn.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select check-in date.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                dpCheckIn.Focus();
                return;
            }

            // Validate check-out date
            if (!dpCheckOut.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select check-out date.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                dpCheckOut.Focus();
                return;
            }

            DateTime checkIn = dpCheckIn.SelectedDate.Value;
            DateTime checkOut = dpCheckOut.SelectedDate.Value;

            // Validate date range
            if (checkOut <= checkIn)
            {
                MessageBox.Show("Check-out date must be after check-in date.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                dpCheckOut.Focus();
                return;
            }

            try
            {
                int guestId = (int)cmbGuest.SelectedValue;
                dynamic selectedItem = cmbRoom.SelectedItem;
                Room room = selectedItem.Room;
                int roomId = room.RoomId;

                // Check if room is available for the selected dates
                int excludeReservationId = isEditMode ? currentReservation.ReservationId : 0;
                if (!ReservationService.IsRoomAvailable(roomId, checkIn, checkOut, excludeReservationId))
                {
                    MessageBox.Show("This room is not available for the selected dates. Please choose different dates or another room.",
                        "Room Not Available",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                bool success;

                if (isEditMode)
                {
                    // Update existing reservation
                    success = ReservationService.UpdateReservation(
                        currentReservation.ReservationId,
                        guestId,
                        roomId,
                        checkIn,
                        checkOut,
                        currentReservation.Status);

                    if (success)
                    {
                        MessageBox.Show("Reservation updated successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update reservation.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Create new reservation
                    success = ReservationService.CreateReservation(guestId, roomId, checkIn, checkOut);

                    if (success)
                    {
                        MessageBox.Show("Reservation created successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create reservation.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cancel button click handler
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}