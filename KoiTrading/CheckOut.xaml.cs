using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KoiTradding.DAL.Models;

namespace KoiTrading
{
    public partial class CheckOut : Window
    {
        private List<KoiFish> _cartItems;
        private decimal _totalAmount;

        public CheckOut(KoiFish fishItem)
        {
            InitializeComponent();

            // Initialize the cart with the selected KoiFish item
            _cartItems = new List<KoiFish> { fishItem };
            CartItemsList.ItemsSource = _cartItems;

            // Calculate total amount based on cart items
            _totalAmount = fishItem.Price ?? 0;
            TotalAmount.Text = _totalAmount.ToString("C"); // Display as currency
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            var shopListWindow = new ShopList();
            shopListWindow.Show();
            this.Close();
        }

        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            // Logic for payment method selection can be added here if needed
        }

        private void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            // Confirm the purchase and show a confirmation message
            MessageBox.Show("Thank you for your purchase! Your order has been confirmed.", 
                            "Payment Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Logic for clearing cart or returning to shop can go here
            var shopListWindow = new ShopList();
            shopListWindow.Show();
            this.Close();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "MM" || textBox.Text == "YY"))
            {
                textBox.Text = "";
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Name == "MMTextBox" ? "MM" : "YY";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
