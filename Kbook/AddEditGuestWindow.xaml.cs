using Kbook.Backend;
using Kbook.Models;
using System;
using System.Windows;

namespace Kbook
{
    public partial class AddEditGuestWindow : Window
    {
        private Guest currentGuest;
        private bool isEditMode;

        /// <summary>
        /// Constructor for Add mode
        /// </summary>
        public AddEditGuestWindow()
        {
            InitializeComponent();
            isEditMode = false;
            txtTitle.Text = "Add New Guest";

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            txtName.Focus();
        }

        /// <summary>
        /// Constructor for Edit mode
        /// </summary>
        public AddEditGuestWindow(Guest guest)
        {
            InitializeComponent();
            isEditMode = true;
            currentGuest = guest;
            txtTitle.Text = "Edit Guest Information";

            // Populate fields with existing data
            txtName.Text = guest.Name;
            txtContactNumber.Text = guest.ContactNumber;
            txtEmail.Text = guest.Email;

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            txtName.Focus();
            txtName.SelectAll();
        }

        /// <summary>
        /// Save button click handler
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter guest's full name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactNumber.Text))
            {
                MessageBox.Show("Please enter contact number.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtContactNumber.Focus();
                return;
            }

            try
            {
                Guest guest = new Guest
                {
                    Name = txtName.Text.Trim(),
                    ContactNumber = txtContactNumber.Text.Trim(),
                    Email = txtEmail.Text.Trim()
                };

                bool success;

                if (isEditMode)
                {
                    // Update existing guest
                    guest.GuestId = currentGuest.GuestId;
                    success = GuestService.UpdateGuest(guest);

                    if (success)
                    {
                        MessageBox.Show("Guest information updated successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update guest information.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Add new guest
                    success = GuestService.AddGuest(guest);

                    if (success)
                    {
                        MessageBox.Show("Guest added successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add guest.",
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