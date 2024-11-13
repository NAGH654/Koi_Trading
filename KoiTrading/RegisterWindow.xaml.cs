using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using System.Windows;

using KoiTradding.DAL.Repositories;
using System.Windows.Navigation;

namespace KoiTrading
{
    public partial class RegisterWindow : Window
    {
        private readonly AccountService _accountService;

        public RegisterWindow()
        {
            InitializeComponent();
            var context = new KoiFishTradingContext();
            var repository = new AccountRepository(context);
            _accountService = new AccountService(repository);
        }

        // SignUp Button Click Event
        private async void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            var username = Username.Text;
            var password = PasswordBox.Password; // Reference named PasswordBox
            var confirmPassword = ConfirmPasswordBox.Password;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Create account object
                var account = new Account
                {
                    Email = username,
                    Password = password // In production, hash the password for security
                };

                bool isAccountCreated = await _accountService.CreateAccountAsync(account);

                if (isAccountCreated)
                {
                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // Close the Register window
                }
                else
                {
                    MessageBox.Show("An error occurred while creating the account. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
