using System;
using System.Windows;
using System.Windows.Controls;
using Kbook.Backend;
using Kbook.Models;

namespace Kbook
{
    public partial class AddEditRoomWindow : Window
    {
        private Room currentRoom;
        private bool isEditMode;

        /// <summary>
        /// Constructor for Add mode
        /// </summary>
        public AddEditRoomWindow()
        {
            InitializeComponent();
            isEditMode = false;
            txtTitle.Text = "Add New Room";

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // Generate next available room number
            txtRoomNumber.Text = GetNextRoomNumber();

            cmbRoomType.Focus();
        }

        /// <summary>
        /// Constructor for Edit mode
        /// </summary>
        public AddEditRoomWindow(Room room)
        {
            InitializeComponent();
            isEditMode = true;
            currentRoom = room;
            txtTitle.Text = "Edit Room Information";

            // Populate fields with existing data
            txtRoomNumber.Text = room.RoomNumber;

            // Set Room Type and auto-set price
            foreach (ComboBoxItem item in cmbRoomType.Items)
            {
                if (item.Content.ToString() == room.Type)
                {
                    cmbRoomType.SelectedItem = item;
                    break;
                }
            }

            // Set Status
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content.ToString() == room.Status)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            cmbRoomType.Focus();
        }

        /// <summary>
        /// Generates the next available room number (1-200)
        /// </summary>
        private string GetNextRoomNumber()
        {
            try
            {
                var allRooms = RoomService.GetAllRooms();

                // Find the highest room number
                int maxRoomNumber = 0;
                foreach (var room in allRooms)
                {
                    if (int.TryParse(room.RoomNumber, out int roomNum))
                    {
                        if (roomNum > maxRoomNumber)
                        {
                            maxRoomNumber = roomNum;
                        }
                    }
                }

                // Generate next number (max + 1, but cap at 200)
                int nextNumber = maxRoomNumber + 1;
                if (nextNumber > 200)
                {
                    // Find first available gap
                    for (int i = 1; i <= 200; i++)
                    {
                        bool exists = false;
                        foreach (var room in allRooms)
                        {
                            if (room.RoomNumber == i.ToString())
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            return i.ToString();
                        }
                    }
                    return "200"; // Fallback if all taken
                }

                return nextNumber.ToString();
            }
            catch
            {
                return "1"; // Default to 1 if error
            }
        }

        /// <summary>
        /// Room Type selection changed - auto-set price
        /// </summary>
        private void CmbRoomType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRoomType.SelectedItem == null) return;

            string selectedType = ((ComboBoxItem)cmbRoomType.SelectedItem).Content.ToString();

            switch (selectedType)
            {
                case "Single Room":
                    txtPrice.Text = "1500.00";
                    break;
                case "Double Room":
                    txtPrice.Text = "2500.00";
                    break;
                case "Suite":
                    txtPrice.Text = "4000.00";
                    break;
                default:
                    txtPrice.Text = "0.00";
                    break;
            }
        }

        /// <summary>
        /// Save button click handler
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Room number is required.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (cmbRoomType.SelectedItem == null)
            {
                MessageBox.Show("Please select room type.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cmbRoomType.Focus();
                return;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select room status.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cmbStatus.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Price is required.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validate price is a valid decimal
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Invalid price value.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                string roomNumber = txtRoomNumber.Text.Trim();

                // Validate room number is between 1-200
                if (!int.TryParse(roomNumber, out int roomNum) || roomNum < 1 || roomNum > 200)
                {
                    MessageBox.Show("Room number must be between 1 and 200.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Check if room number already exists (except when editing current room)
                int excludeId = isEditMode ? currentRoom.RoomId : 0;
                if (RoomService.RoomNumberExists(roomNumber, excludeId))
                {
                    MessageBox.Show($"Room number '{roomNumber}' already exists. Please use a different room number.",
                        "Duplicate Room Number",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                Room room = new Room
                {
                    RoomNumber = roomNumber,
                    Type = ((ComboBoxItem)cmbRoomType.SelectedItem).Content.ToString(),
                    Status = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString(),
                    Price = price
                };

                bool success;

                if (isEditMode)
                {
                    // Update existing room
                    room.RoomId = currentRoom.RoomId;
                    success = RoomService.UpdateRoom(room);

                    if (success)
                    {
                        MessageBox.Show("Room information updated successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update room information.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Add new room
                    success = RoomService.AddRoom(room);

                    if (success)
                    {
                        MessageBox.Show("Room added successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add room.",
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