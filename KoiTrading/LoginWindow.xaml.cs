using System.Windows;
using System.Windows.Navigation;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTrading;

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
        var email = EmailTextBox.Text ?? string.Empty;
        var password = PasswordBox.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            MessageBox.Show("Email cannot be empty", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Password cannot be empty", "Validation Error", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var account = await _accountService.LoginAsync(email, password);

        if (account != null)
        {
            MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Store the logged-in user in the UserSession
            UserSession.LoggedInUser = account;

            // Check the roleId and navigate accordingly
            switch (account.RoleId)
            {
                case 1: // Admin or Manager role
                    var dashboardStaff = new DashboardStaff();
                    dashboardStaff.Show();
                    break;

                case 2: // Staff role
                    var dashboardManager = new DashboardManager();
                    dashboardManager.Show();
                    break;

                case 3: // Regular User role (ShopList)
                    var shopList = new ShopList();
                    shopList.Show();
                    break;

                default:
                    MessageBox.Show("Unknown role. Cannot redirect to the appropriate dashboard.", 
                        "Role Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }

            Close(); // Close the login window after redirecting
        }
        else
        {
            MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButton.OK,
                MessageBoxImage.Error);
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
        Close();
    }
}