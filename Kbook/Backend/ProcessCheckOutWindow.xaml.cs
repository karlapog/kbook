using System;
using System.Windows;
using System.Windows.Controls;
using Kbook.Backend;
using Kbook.Models;

namespace Kbook
{
    public partial class ProcessCheckOutWindow : Window
    {
        private Reservation reservation;

        public ProcessCheckOutWindow(Reservation reservation)
        {
            InitializeComponent();

            this.reservation = reservation;

            // Populate fields
            txtGuestInfo.Text = $"Guest: {reservation.GuestName}";
            txtRoomInfo.Text = $"Room: {reservation.RoomNumber}";
            txtDatesInfo.Text = $"Check-In: {reservation.CheckIn:yyyy-MM-dd} | Check-Out: {reservation.CheckOut:yyyy-MM-dd}";
            txtTotalAmount.Text = reservation.TotalAmount.ToString("N2");

            btnConfirm.Click += BtnConfirm_Click;
            btnCancel.Click += BtnCancel_Click;

            cmbPaymentMethod.Focus();
        }

        /// <summary>
        /// Confirm Payment button click handler
        /// </summary>
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPaymentMethod.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cmbPaymentMethod.Focus();
                return;
            }

            string paymentMethod = ((ComboBoxItem)cmbPaymentMethod.SelectedItem).Content.ToString();

            MessageBoxResult result = MessageBox.Show(
                $"Confirm payment of ₱{reservation.TotalAmount:N2} via {paymentMethod}?",
                "Confirm Payment",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = CheckOutService.ProcessCheckOut(
                        reservation.ReservationId,
                        reservation.RoomId,
                        reservation.TotalAmount,
                        paymentMethod);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Check-out successful!\n\n" +
                            $"Guest: {reservation.GuestName}\n" +
                            $"Amount Paid: ₱{reservation.TotalAmount:N2}\n" +
                            $"Payment Method: {paymentMethod}\n\n" +
                            $"Thank you!",
                            "Check-Out Successful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to process check-out.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
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