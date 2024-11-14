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
                string email = EmailTextBox.Text ?? string.Empty;
                string password = PasswordBox.Password ?? string.Empty;

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
        
                var account = await _accountService.LoginAsync(email, password);

                if (account != null)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
                    // Store the logged-in user in the UserSession
                    UserSession.LoggedInUser = account;

                    // Open the ShopList window
                    var shopList = new ShopList();
                    shopList.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
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
