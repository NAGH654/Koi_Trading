using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using KoiTradding.DAL.Repositories;

namespace KoiTrading
{
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService;

        public LoginWindow()
        {
            InitializeComponent();
            var context = new KoiFishTradingContext();
            var repository = new AccountRepository(context);
            _accountService = new AccountService(repository);
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = ((TextBox)LogicalTreeHelper.FindLogicalNode(this, "EmailTextBox"))?.Text ?? string.Empty;
                string password = ((PasswordBox)LogicalTreeHelper.FindLogicalNode(this, "PasswordBox"))?.Password ?? string.Empty;

                // Validation
                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Email cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Password cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Attempt to login
                var account = await _accountService.LoginAsync(email, password);

                if (account != null)
                {
                    // Login successful
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    var ShopList = new ShopList();
                     ShopList.Show();
                     this.Close();
                }
                else
                {
                    // Login failed
                    MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
           
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}
