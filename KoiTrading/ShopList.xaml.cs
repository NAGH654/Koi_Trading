using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTrading;

public partial class ShopList : Window
{
    private readonly int _itemsPerPage = 9;
    private readonly KoiFishService _koiFishService;
    private int _currentPage = 1;
    private ObservableCollection<KoiFish> _filteredKoiFishList;

    private ObservableCollection<KoiFish> _koiFishList;

    public ShopList()
    {
        InitializeComponent();
        if (UserSession.LoggedInUser != null) UserGreetingTextBlock.Text = $"Hello, {UserSession.LoggedInUser.Email}";
        var context = new KoiFishTradingContext();
        var repository = new KoiFishRepository(context);
        _koiFishService = new KoiFishService(repository);
        LoadKoiFishDataAsync();
    }


    private async Task LoadKoiFishDataAsync()
    {
        try
        {
            var koiFishData = await _koiFishService.GetAllKoiFishAsync();
            _koiFishList = new ObservableCollection<KoiFish>(koiFishData);
            _filteredKoiFishList = new ObservableCollection<KoiFish>(_koiFishList);

            _currentPage = 1;
            LoadPage(_currentPage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading KoiFish data: {ex.Message}");
        }
    }

    private void LoadPage(int pageNumber)
    {
        var startIndex = (pageNumber - 1) * _itemsPerPage;
        var paginatedList = _filteredKoiFishList.Skip(startIndex).Take(_itemsPerPage).ToList();
        FishList.ItemsSource = paginatedList;
    }

    private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
    {
        FilterKoiFishList();
        _currentPage = 1;
        LoadPage(_currentPage);
    }

    private void FilterKoiFishList()
    {
        // Get filter values
        var selectedOrigin = ((ComboBoxItem)OriginComboBox.SelectedItem)?.Content.ToString() ?? "All";
        var selectedGender = ((ComboBoxItem)GenderComboBox.SelectedItem)?.Content.ToString() ?? "All";
        var minPriceValid = decimal.TryParse(MinPriceTextBox.Text, out var minPrice);
        var maxPriceValid = decimal.TryParse(MaxPriceTextBox.Text, out var maxPrice);

        // Validate price inputs
        if (!minPriceValid) minPrice = 0;
        if (!maxPriceValid) maxPrice = decimal.MaxValue;

        // Call the filtering method in the service
        var filteredList =
            _koiFishService.FilterKoiFish(_koiFishList.ToList(), selectedOrigin, selectedGender, minPrice, maxPrice);

        // Update the filtered collection
        _filteredKoiFishList = new ObservableCollection<KoiFish>(filteredList);
        LoadPage(_currentPage);
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && (textBox.Text == "Min Price" || textBox.Text == "Max Price"))
        {
            textBox.Text = "";
            textBox.Foreground = Brushes.Black;
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = textBox.Name == "MinPriceTextBox" ? "Min Price" : "Max Price";
            textBox.Foreground = Brushes.Gray;
        }
    }

    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < (_filteredKoiFishList.Count + _itemsPerPage - 1) / _itemsPerPage)
        {
            _currentPage++;
            LoadPage(_currentPage);
        }
    }

    private void PreviousPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            LoadPage(_currentPage);
        }
    }

    private void FishList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (FishList.SelectedItem is KoiFish selectedFish)
        {
            var fishDetailWindow = new FishDetail(selectedFish);
            fishDetailWindow.Show();
            Hide();
        }
    }


    private void BuyNowButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is KoiFish selectedFish)
        {
            var checkoutWindow = new CheckOut(selectedFish); // Pass the selected fish directly
            checkoutWindow.Owner = this; // Optional: Set the owner to the current window
            checkoutWindow.Show(); // Show the checkout window
          
        }
    }

}