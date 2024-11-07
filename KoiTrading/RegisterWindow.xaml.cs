using System.Windows;
using System.Windows.Navigation;

namespace KoiTrading;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close(); // Đóng cửa sổ đăng ký hiện tại
        e.Handled = true;
    }

    private void SignUpBtn_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}