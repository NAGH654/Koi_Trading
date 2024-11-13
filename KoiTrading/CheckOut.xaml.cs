using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KoiTrading;

public partial class CheckOut : Window
{
    
    private string selectedPaymentMethod;
    public CheckOut()
    {
        InitializeComponent();
    }
    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null && (textBox.Text == "MM" || textBox.Text == "YY"))
        {
            textBox.Text = "";
            textBox.Foreground = Brushes.Black;
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = textBox.Name == "MMTextBox" ? "MM" : "YY";
            textBox.Foreground = Brushes.Gray;
        }
    }
    
    private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        if (radioButton != null)
        {
            selectedPaymentMethod = radioButton.Content.ToString();
        }
    }
    private void CartButton_Click(object sender, RoutedEventArgs e)
    {
        var cartWindow = new Cart();
        cartWindow.Show();
        this.Hide();
    }
}