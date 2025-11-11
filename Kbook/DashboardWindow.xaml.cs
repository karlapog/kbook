using Kbook.Backend;
using Kbook.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Kbook
{
    public partial class DashboardWindow : Window
    {
        private int staffUserId;
        private string staffUsername;

        public DashboardWindow(int userId, string username)
        {
            InitializeComponent();

            this.staffUserId = userId;
            this.staffUsername = username;

            // Set welcome message
            txtWelcomeUser.Text = username;

            // Attach event handlers
            btnLogout.Click += BtnLogout_Click;

            // Guest Management event handlers
            btnAddGuest.Click += BtnAddGuest_Click;
            btnEditGuest.Click += BtnEditGuest_Click;
            btnDeleteGuest.Click += BtnDeleteGuest_Click;

            // Room Management event handlers
            btnAddRoom.Click += BtnAddRoom_Click;
            btnEditRoom.Click += BtnEditRoom_Click;
            btnDeleteRoom.Click += BtnDeleteRoom_Click;

            // Reservation Management event handlers
            btnAddReservation.Click += BtnAddReservation_Click;
            btnEditReservation.Click += BtnEditReservation_Click;
            btnCancelReservation.Click += BtnCancelReservation_Click;

            // Check-In event handlers
            btnProcessCheckIn.Click += BtnProcessCheckIn_Click;
            btnRefreshCheckIn.Click += BtnRefreshCheckIn_Click;

            // Check-Out event handlers
            btnProcessCheckOut.Click += BtnProcessCheckOut_Click;
            btnRefreshCheckOut.Click += BtnRefreshCheckOut_Click;

            // Initialize dashboard data
            LoadDashboardData();
        }

        /// <summary>
        /// Loads initial data for the dashboard
        /// </summary>
        private void LoadDashboardData()
        {
            LoadGuests();
            LoadRooms();
            LoadReservations();
            LoadCheckIns();
            LoadCheckOuts();
        }

        #region Guest Management

        /// <summary>
        /// Loads all guests into the DataGrid
        /// </summary>
        private void LoadGuests()
        {
            try
            {
                List<Guest> guests = GuestService.GetAllGuests();
                dgGuests.ItemsSource = guests;
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
        /// Add Guest button click handler
        /// </summary>
        private void BtnAddGuest_Click(object sender, RoutedEventArgs e)
        {
            AddEditGuestWindow addWindow = new AddEditGuestWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadGuests(); // Refresh the grid
            }
        }

        /// <summary>
        /// Edit Guest button click handler
        /// </summary>
        private void BtnEditGuest_Click(object sender, RoutedEventArgs e)
        {
            if (dgGuests.SelectedItem == null)
            {
                MessageBox.Show("Please select a guest to edit.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Guest selectedGuest = (Guest)dgGuests.SelectedItem;
            AddEditGuestWindow editWindow = new AddEditGuestWindow(selectedGuest);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadGuests(); // Refresh the grid
            }
        }

        /// <summary>
        /// Delete Guest button click handler
        /// </summary>
        private void BtnDeleteGuest_Click(object sender, RoutedEventArgs e)
        {
            if (dgGuests.SelectedItem == null)
            {
                MessageBox.Show("Please select a guest to delete.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Guest selectedGuest = (Guest)dgGuests.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete guest '{selectedGuest.Name}'?\n\nThis action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = GuestService.DeleteGuest(selectedGuest.GuestId);

                    if (success)
                    {
                        MessageBox.Show("Guest deleted successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        LoadGuests(); // Refresh the grid
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete guest.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting guest: {ex.Message}\n\nThis guest may have existing reservations.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Room Management

        /// <summary>
        /// Loads all rooms into the DataGrid
        /// </summary>
        private void LoadRooms()
        {
            try
            {
                List<Room> rooms = RoomService.GetAllRooms();
                dgRooms.ItemsSource = rooms;
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
        /// Add Room button click handler
        /// </summary>
        private void BtnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            AddEditRoomWindow addWindow = new AddEditRoomWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadRooms(); // Refresh the grid
            }
        }

        /// <summary>
        /// Edit Room button click handler
        /// </summary>
        private void BtnEditRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedItem == null)
            {
                MessageBox.Show("Please select a room to edit.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Room selectedRoom = (Room)dgRooms.SelectedItem;
            AddEditRoomWindow editWindow = new AddEditRoomWindow(selectedRoom);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadRooms(); // Refresh the grid
            }
        }

        /// <summary>
        /// Delete Room button click handler
        /// </summary>
        private void BtnDeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedItem == null)
            {
                MessageBox.Show("Please select a room to delete.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Room selectedRoom = (Room)dgRooms.SelectedItem;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete Room {selectedRoom.RoomNumber}?\n\nThis action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = RoomService.DeleteRoom(selectedRoom.RoomId);

                    if (success)
                    {
                        MessageBox.Show("Room deleted successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        LoadRooms(); // Refresh the grid
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete room.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting room: {ex.Message}\n\nThis room may have existing reservations.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Reservation Management

        /// <summary>
        /// Loads all reservations into the DataGrid
        /// </summary>
        private void LoadReservations()
        {
            try
            {
                List<Reservation> reservations = ReservationService.GetAllReservations();
                dgReservations.ItemsSource = reservations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reservations: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Add Reservation button click handler
        /// </summary>
        private void BtnAddReservation_Click(object sender, RoutedEventArgs e)
        {
            AddEditReservationWindow addWindow = new AddEditReservationWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadReservations(); // Refresh the grid
                LoadRooms(); // Refresh rooms (status may have changed)
            }
        }

        /// <summary>
        /// Edit Reservation button click handler
        /// </summary>
        private void BtnEditReservation_Click(object sender, RoutedEventArgs e)
        {
            if (dgReservations.SelectedItem == null)
            {
                MessageBox.Show("Please select a reservation to edit.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Reservation selectedReservation = (Reservation)dgReservations.SelectedItem;

            // Don't allow editing of checked-in or checked-out reservations
            if (selectedReservation.Status == "CheckedIn" || selectedReservation.Status == "CheckedOut")
            {
                MessageBox.Show("Cannot edit reservations that are already checked-in or checked-out.",
                    "Cannot Edit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            AddEditReservationWindow editWindow = new AddEditReservationWindow(selectedReservation);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadReservations(); // Refresh the grid
            }
        }

        /// <summary>
        /// Cancel Reservation button click handler
        /// </summary>
        private void BtnCancelReservation_Click(object sender, RoutedEventArgs e)
        {
            if (dgReservations.SelectedItem == null)
            {
                MessageBox.Show("Please select a reservation to cancel.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Reservation selectedReservation = (Reservation)dgReservations.SelectedItem;

            // Don't allow canceling already cancelled or checked-out reservations
            if (selectedReservation.Status == "Cancelled" || selectedReservation.Status == "CheckedOut")
            {
                MessageBox.Show($"Cannot cancel a reservation that is already {selectedReservation.Status}.",
                    "Cannot Cancel",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to cancel this reservation?\n\n" +
                $"Guest: {selectedReservation.GuestName}\n" +
                $"Room: {selectedReservation.RoomNumber}\n" +
                $"Check-In: {selectedReservation.CheckIn:yyyy-MM-dd}\n\n" +
                $"This will set the room status back to Available.",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = ReservationService.CancelReservation(selectedReservation.ReservationId, selectedReservation.RoomId);

                    if (success)
                    {
                        MessageBox.Show("Reservation cancelled successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        LoadReservations(); // Refresh the grid
                        LoadRooms(); // Refresh rooms (status changed back to Available)
                    }
                    else
                    {
                        MessageBox.Show("Failed to cancel reservation.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cancelling reservation: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Check-In Management

        /// <summary>
        /// Loads all pending check-ins into the DataGrid
        /// </summary>
        private void LoadCheckIns()
        {
            try
            {
                List<Reservation> checkIns = CheckInService.GetPendingCheckIns();
                dgCheckIn.ItemsSource = checkIns;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading check-ins: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Process Check-In button click handler
        /// </summary>
        private void BtnProcessCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (dgCheckIn.SelectedItem == null)
            {
                MessageBox.Show("Please select a reservation to check-in.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Reservation selectedReservation = (Reservation)dgCheckIn.SelectedItem;

            // Confirm check-in
            MessageBoxResult result = MessageBox.Show(
                $"Process check-in for:\n\n" +
                $"Guest: {selectedReservation.GuestName}\n" +
                $"Room: {selectedReservation.RoomNumber}\n" +
                $"Check-In: {selectedReservation.CheckIn:yyyy-MM-dd}\n" +
                $"Check-Out: {selectedReservation.CheckOut:yyyy-MM-dd}\n\n" +
                $"Continue?",
                "Confirm Check-In",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = CheckInService.ProcessCheckIn(selectedReservation.ReservationId, selectedReservation.RoomId);

                    if (success)
                    {
                        MessageBox.Show($"Guest {selectedReservation.GuestName} has been checked in successfully!",
                            "Check-In Successful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        LoadCheckIns(); // Refresh check-in grid
                        LoadReservations(); // Refresh reservations grid
                        LoadRooms(); // Refresh rooms grid
                    }
                    else
                    {
                        MessageBox.Show("Failed to process check-in.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing check-in: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Refresh Check-In button click handler
        /// </summary>
        private void BtnRefreshCheckIn_Click(object sender, RoutedEventArgs e)
        {
            LoadCheckIns();
            MessageBox.Show("Check-in list refreshed.",
                "Refreshed",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        #region Check-Out Management

        /// <summary>
        /// Loads all checked-in guests (ready for check-out) into the DataGrid
        /// </summary>
        private void LoadCheckOuts()
        {
            try
            {
                List<Reservation> checkOuts = CheckOutService.GetCheckedInReservations();
                dgCheckOut.ItemsSource = checkOuts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading check-outs: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Process Check-Out button click handler
        /// </summary>
        private void BtnProcessCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (dgCheckOut.SelectedItem == null)
            {
                MessageBox.Show("Please select a reservation to check-out.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            Reservation selectedReservation = (Reservation)dgCheckOut.SelectedItem;

            // Open payment dialog
            ProcessCheckOutWindow checkOutWindow = new ProcessCheckOutWindow(selectedReservation);
            bool? result = checkOutWindow.ShowDialog();

            if (result == true)
            {
                LoadCheckOuts(); // Refresh check-out grid
                LoadReservations(); // Refresh reservations grid
                LoadRooms(); // Refresh rooms grid (status changed to Available)
            }
        }

        /// <summary>
        /// Refresh Check-Out button click handler
        /// </summary>
        private void BtnRefreshCheckOut_Click(object sender, RoutedEventArgs e)
        {
            LoadCheckOuts();
            MessageBox.Show("Check-out list refreshed.",
                "Refreshed",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        #endregion

        /// <summary>
        /// Logout button click event
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Open login window
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                // Close current dashboard
                this.Close();
            }
        }
    }
}