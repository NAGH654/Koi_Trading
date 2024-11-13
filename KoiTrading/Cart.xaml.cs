using System.Windows;
using System.Windows.Input;

namespace KoiTrading;

public partial class Cart : Window
{
    public Cart()
    {
        InitializeComponent();
    }

    private void StackPanel_Mousedown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
    
    private void QuitButton_Click(object sender, RoutedEventArgs e)
    {
        var shopListWindow = new ShopList();
        shopListWindow.Show();
        
        this.Close();
    }
    
    private void CheckoutButton_Click(object sender, RoutedEventArgs e)
    {
        
    }
}