using System;
using System.Windows;
using Kbook.Models;
using Kbook.Backend;

namespace Kbook
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Set initial focus
            this.Loaded += (s, e) => txtUsername.Focus();

            // Attach event handlers
            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        /// <summary>
        /// Login Button Click Event - Validates staff credentials
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Get input values
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Disable login button to prevent double-clicks
            btnLogin.IsEnabled = false;

            try
            {
                // Authenticate staff user
                AuthenticationResult result = AuthenticationService.Authenticate(username, password);

                if (result.IsSuccessful)
                {
                    // Open Front Desk Dashboard
                    DashboardWindow dashboard = new DashboardWindow(result.UserId, result.Username);
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    // Show error message
                    MessageBox.Show(result.ErrorMessage,
                        "Login Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    // Clear password field
                    txtPassword.Clear();
                    txtUsername.Focus();

                    // Re-enable login button
                    btnLogin.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Re-enable login button
                btnLogin.IsEnabled = true;
            }
        }

        /// <summary>
        /// Cancel Button Click Event - Closes the application
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}