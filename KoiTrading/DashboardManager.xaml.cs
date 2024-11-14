using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTrading
{
    public partial class DashboardManager : Window
    {
        private readonly AccountService _service;
        private readonly OrderService _orderService;
        public DashboardManager()
        {
            InitializeComponent();
            var context = new KoiFishTradingContext();
            var repository = new AccountRepository(context);
            var repository2 = new OrderRepository(context); 
            _service = new AccountService(repository);
            _orderService = new OrderService(repository2);
            _ = LoadAccounts();
            _ = LoadOrders();
        }

        private async Task LoadAccounts()
        {
            var accounts = await _service.GetAllAccountsAsync();
            if (accounts != null && accounts.Any())
            {
                EmployeeDataGrid.ItemsSource = accounts;
            }
        }

        private async Task LoadOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders != null && orders.Any())
            {
                OrderDataGrid.ItemsSource = orders;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text;
                string password = PasswordBox.Password;
                string fullname = FullNameTextBox.Text;
                string phone = PhoneTextBox.Text;
                string status = StatusTextBox.Text;
                string address = AddressTextBox.Text;

                var account = new Account();
                
                account.Email = email;
                account.Password = password;
                account.FullName = fullname;
                account.Phone = phone;
                account.Status = status;
                account.Address = address;
                account.RoleId = 1;

                bool isAdded = await _service.AddAccountAsync(account);
                if (isAdded)
                {
                    MessageBox.Show(" added successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to add.");
                }

                LoadAccounts();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Account account)
            {
                try
                {
                    account.Email = EmailTextBox.Text;
                    account.Password = PasswordBox.Password;
                    account.FullName = FullNameTextBox.Text;
                    account.Phone = PhoneTextBox.Text;
                    account.Status = StatusTextBox.Text;
                    account.Address = AddressTextBox.Text;
                    bool isUpdated = await _service.UpdateAccountAsync(account);
                    if (isUpdated)
                    {
                        MessageBox.Show("updated successfully.", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadAccounts();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        private void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Account selectedItem)
            {
                EmailTextBox.Text = selectedItem.Email;
                PasswordBox.Password = selectedItem.Password;
                FullNameTextBox.Text = selectedItem.FullName;
                PhoneTextBox.Text = selectedItem.Phone;
                StatusTextBox.Text = selectedItem.Status;
                AddressTextBox.Text = selectedItem.Address;
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is Account account)
            {
                var result = MessageBox.Show("Are you sure you want to delete this koi fish?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool isDeleted = await _service.DeleteAccountAsync(account.AccountId);

                        if (isDeleted)
                        {
                            MessageBox.Show("deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadAccounts();
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a employee to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearForm()
        {
            EmailTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            FullNameTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            StatusTextBox.Text = string.Empty;
            AddressTextBox.Text = string.Empty;
        }
    }
}
