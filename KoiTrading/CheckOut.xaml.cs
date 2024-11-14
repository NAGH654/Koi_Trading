using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KoiTrading;

public partial class CheckOut
{
    private readonly OrderDetailService _orderDetailService;
    private readonly OrderService _orderService;
    private readonly PaymentService _paymentService;
    private readonly List<KoiFish> _cartItems;
    private readonly decimal _totalAmount;

    public CheckOut(KoiFish fishItem)
    {
        InitializeComponent();
        LoggedInUserEmail.Text = $"Hello, {UserSession.LoggedInUser.Email}";
        _cartItems = new List<KoiFish> { fishItem };
        CartItemsList.ItemsSource = _cartItems; 
        _totalAmount = fishItem.Price ?? 0;
        TotalAmount.Text = _totalAmount.ToString("C");
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
        var cardNumber = CardNumberTextBox.Text.Trim();
        var expMonthText = MMTextBox.Text.Trim();
        var expYearText = YYTextBox.Text.Trim();
        var cvv = CVVTextBox.Text.Trim();

        // Validate Card Number
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
        {
            MessageBox.Show("Please enter a valid 16-digit card number.", "Invalid Card Number", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        // Validate Expiration Date
        if (!int.TryParse(expMonthText, out var expMonth) || expMonth < 1 || expMonth > 12)
        {
            MessageBox.Show("Please enter a valid expiration month (1-12).", "Invalid Expiration Month",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(expYearText, out var expYear))
        {
            MessageBox.Show("Please enter a valid expiration year.", "Invalid Expiration Year", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }


        if (expYear < 100) expYear += 2000;

        var currentDate = DateTime.Now;
        var expirationDate = new DateTime(expYear, expMonth, 1).AddMonths(1).AddDays(-1);

        if (expirationDate < currentDate)
        {
            MessageBox.Show("Your credit card has expired.", "Expired Card", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        // Validate CVV
        if (string.IsNullOrEmpty(cvv) || cvv.Length != 3 || !cvv.All(char.IsDigit))
        {
            MessageBox.Show("Please enter a valid 3-digit CVV.", "Invalid CVV", MessageBoxButton.OK,
                MessageBoxImage.Warning);
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


                var orderCreated = await _orderService.CreateOrderAsync(order);
                if (!orderCreated)
                {
                    MessageBox.Show("Failed to create order. Please try again.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }


                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    KoiId = _cartItems[0].KoiId,
                    Quantity = 1,
                    Price = _cartItems[0].Price
                };

                var orderDetailCreated = await _orderDetailService.CreateOrderDetailAsync(orderDetail);
                if (!orderDetailCreated)
                {
                    MessageBox.Show("Failed to create order detail. Please try again.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
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

                var paymentCreated = await _paymentService.CreatePaymentAsync(payment);
                if (!paymentCreated)
                {
                    MessageBox.Show("Failed to process payment. Please try again.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Complete the transaction
                transaction.Complete();

                // Show confirmation message and navigate back
                MessageBox.Show("Thank you for your purchase! Your order has been confirmed.", "Payment Confirmed",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                var shopListWindow = new ShopList();
                shopListWindow.Show();
                Close();
            }

        }
        catch (DbUpdateException dbEx)
        {
            // Handle specific database update exceptions
            MessageBox.Show($"Database error occurred: {dbEx.InnerException?.Message ?? dbEx.Message}",
                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            // Handle unexpected exceptions
            MessageBox.Show($"An error occurred during checkout: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private string GenerateTransactionCode()
    {
        // Example: Generate an 8-character alphanumeric code
        return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    }

    private void CartButton_Click(object sender, RoutedEventArgs e)
    {
        var shopListWindow = new ShopList();
        shopListWindow.Show();
        Close();
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
            textBox.Foreground = Brushes.Black;
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = textBox.Name == "MMTextBox" ? "MM" : "YY";
            textBox.Foreground = Brushes.Gray;
        }
    }
}