using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KoiTrading;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        var registerWindow = new RegisterWindow();
        registerWindow.Show();
        this.Close();
        e.Handled = true;
    }

    private void LoginBtn_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}