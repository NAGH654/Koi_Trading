using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KoiTradding.DAL.Models;

namespace KoiTrading
{
    public partial class ShopList : Window
    {
        private ObservableCollection<KoiFish> _koiFishList; // Dữ liệu mẫu
        private readonly int _itemsPerPage = 9; 
        private int _currentPage = 1;

        public ShopList()
        {
            InitializeComponent();
            _koiFishList = GetSampleKoiFishList(); 
            LoadPage(_currentPage); 
        }

        private void LoadPage(int pageNumber)
        {
            int startIndex = (pageNumber - 1) * _itemsPerPage;
            var paginatedList = _koiFishList.Skip(startIndex).Take(_itemsPerPage).ToList();
            FishList.ItemsSource = paginatedList;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Min Price" || textBox.Text == "Max Price"))
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
                textBox.Text = textBox.Name == "MinPriceTextBox" ? "Min Price" : "Max Price";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            var cartWindow = new Cart();
            cartWindow.Show();
            this.Close();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < (_koiFishList.Count + _itemsPerPage - 1) / _itemsPerPage)
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
            if (FishList.SelectedItem != null)
            {
                var selectedFish = (KoiFish)FishList.SelectedItem;
                var fishDetailWindow = new FishDetail(selectedFish);
                fishDetailWindow.Show();
            }
        }

        private ObservableCollection<KoiFish> GetSampleKoiFishList()
        {
            return new ObservableCollection<KoiFish>
            {
                new KoiFish { KoiId = 1, Origin = "Japan", Gender = "Male", Age = 2, Size = 60.5m, Status = "Available", Price = 10000m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p1.png" },
                new KoiFish { KoiId = 2, Origin = "Japan", Gender = "Female", Age = 3, Size = 55.0m, Status = "Available", Price = 8500m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p2.jpeg" },
                new KoiFish { KoiId = 3, Origin = "China", Gender = "Male", Age = 1, Size = 65.2m, Status = "Available", Price = 12000m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p3.png" },
                new KoiFish { KoiId = 4, Origin = "Vietnam", Gender = "Female", Age = 2, Size = 58.3m, Status = "Available", Price = 9000m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p1.png" },
                new KoiFish { KoiId = 5, Origin = "Thailand", Gender = "Male", Age = 4, Size = 62.7m, Status = "Available", Price = 9500m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p2.jpeg" },
                new KoiFish { KoiId = 6, Origin = "Japan", Gender = "Male", Age = 5, Size = 75.0m, Status = "Available", Price = 15000m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p3.png" },
                new KoiFish { KoiId = 7, Origin = "Japan", Gender = "Female", Age = 1, Size = 50.0m, Status = "Available", Price = 7000m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p1.png" },
                new KoiFish { KoiId = 8, Origin = "China", Gender = "Male", Age = 3, Size = 67.8m, Status = "Available", Price = 11500m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p2.jpeg" },
                new KoiFish { KoiId = 9, Origin = "Vietnam", Gender = "Female", Age = 2, Size = 54.0m, Status = "Available", Price = 8000m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p3.png" },
                new KoiFish { KoiId = 10, Origin = "Thailand", Gender = "Male", Age = 4, Size = 64.5m, Status = "Available", Price = 9700m, Health = "Healthy", KoiImage = "pack://application:,,,/KoiTrading;component/images/p1.png" }
            };
        }
    }
}
