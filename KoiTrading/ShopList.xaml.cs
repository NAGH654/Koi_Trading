using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTrading
{
    public partial class ShopList : Window
    {
        private readonly KoiFishService _koiFishService;
        private readonly int _itemsPerPage = 9;
        private int _currentPage = 1;

        private ObservableCollection<KoiFish> _koiFishList; 
        private ObservableCollection<KoiFish> _filteredKoiFishList; 

        public ShopList()
        {
            InitializeComponent();
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
            int startIndex = (pageNumber - 1) * _itemsPerPage;
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
            
            string selectedOrigin = ((ComboBoxItem)((ComboBox)FindName("OriginComboBox")).SelectedItem).Content.ToString();
            string selectedGender = ((ComboBoxItem)((ComboBox)FindName("GenderComboBox")).SelectedItem).Content.ToString();
            decimal.TryParse(MinPriceTextBox.Text, out decimal minPrice);
            decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice);
            
            var filteredList = _koiFishService.FilterKoiFish(_koiFishList.ToList(), selectedOrigin, selectedGender, minPrice, maxPrice);
            
            _filteredKoiFishList = new ObservableCollection<KoiFish>(filteredList);
            FishList.ItemsSource = _filteredKoiFishList;
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

        private void FishList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FishList.SelectedItem is KoiFish selectedFish)
            {
                var fishDetailWindow = new FishDetail(selectedFish);
                fishDetailWindow.Show();
                this.Hide();
            }
        }
    }
}