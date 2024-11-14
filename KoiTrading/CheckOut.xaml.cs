
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KoiTrading
{
    public partial class CheckOut : Window
    {
        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;
        private readonly PaymentService _paymentService;
        
        private List<KoiFish> _cartItems;
        private decimal _totalAmount;

        public CheckOut(KoiFish fishItem)
        {
            InitializeComponent();

            if (UserSession.LoggedInUser != null)
            {
                // Display the user's email
                LoggedInUserEmail.Text = $"Hello, {UserSession.LoggedInUser.Email}";
            }

            // Initialize cart items
            _cartItems = new List<KoiFish> { fishItem };
            CartItemsList.ItemsSource = _cartItems;
            _totalAmount = fishItem.Price ?? 0;
            TotalAmount.Text = _totalAmount.ToString("C");

            // Initialize services
            var context = new KoiFishTradingContext();
            var orderRepository = new OrderRepository(context);
            var orderDetailRepository = new OrderDetailRepository(context);
            var paymentRepository = new PaymentRepository(context);
            _orderService = new OrderService(orderRepository);
            _orderDetailService = new OrderDetailService(orderDetailRepository);
            _paymentService = new PaymentService(paymentRepository);
        }

        private async void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate payment fields
            string cardNumber = CardNumberTextBox.Text.Trim();
            string expMonthText = MMTextBox.Text.Trim();
            string expYearText = YYTextBox.Text.Trim();
            string cvv = CVVTextBox.Text.Trim();

            // Validate Card Number
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
            {
                MessageBox.Show("Please enter a valid 16-digit card number.", "Invalid Card Number", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate Expiration Date
            if (!int.TryParse(expMonthText, out int expMonth) || expMonth < 1 || expMonth > 12)
            {
                MessageBox.Show("Please enter a valid expiration month (1-12).", "Invalid Expiration Month", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(expYearText, out int expYear))
            {
                MessageBox.Show("Please enter a valid expiration year.", "Invalid Expiration Year", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            if (expYear < 100)
            {
                expYear += 2000;
            }

            var currentDate = DateTime.Now;
            var expirationDate = new DateTime(expYear, expMonth, 1).AddMonths(1).AddDays(-1);

            if (expirationDate < currentDate)
            {
                MessageBox.Show("Your credit card has expired.", "Expired Card", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate CVV
            if (string.IsNullOrEmpty(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
            {
                MessageBox.Show("Please enter a valid 3-digit CVV.", "Invalid CVV", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Step 1: Create the Order
                    var order = new Order
                    {
                        AccountId = UserSession.LoggedInUser.AccountId,
                        OrderDate = DateTime.UtcNow,
                        Status = "Completed",
                        TotalPrice = _totalAmount
                    };

                    // Save the Order to generate OrderId
                    bool orderCreated = await _orderService.CreateOrderAsync(order);
                    if (!orderCreated)
                    {
                        MessageBox.Show("Failed to create order. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Step 2: Create OrderDetail associated with the Order
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        KoiId = _cartItems[0].KoiId,
                        Quantity = 1,
                        Price = _cartItems[0].Price
                    };

                    bool orderDetailCreated = await _orderDetailService.CreateOrderDetailAsync(orderDetail);
                    if (!orderDetailCreated)
                    {
                        MessageBox.Show("Failed to create order detail. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Step 3: Create Payment associated with the Order
                    var payment = new Payment
                    {
                        OrderId = order.OrderId,
                        Amount = _totalAmount,
                        PaymentDate = DateTime.UtcNow,
                        Status = "Completed",
                        TransactionCode = GenerateTransactionCode()
                    };

                    bool paymentCreated = await _paymentService.CreatePaymentAsync(payment);
                    if (!paymentCreated)
                    {
                        MessageBox.Show("Failed to process payment. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Complete the transaction
                    transaction.Complete();

                    // Show confirmation message and navigate back
                    MessageBox.Show("Thank you for your purchase! Your order has been confirmed.", "Payment Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);

                    var shopListWindow = new ShopList();
                    shopListWindow.Show();
                    this.Close();
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Handle specific database update exceptions
                MessageBox.Show($"Database error occurred: {dbEx.InnerException?.Message ?? dbEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                MessageBox.Show($"An error occurred during checkout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Generates a unique transaction code for the payment.
        /// </summary>
        /// <returns>A unique transaction code string.</returns>
        private string GenerateTransactionCode()
        {
            // Example: Generate an 8-character alphanumeric code
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
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
