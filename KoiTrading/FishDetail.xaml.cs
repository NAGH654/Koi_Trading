﻿using System.Windows;
using KoiTradding.DAL.Models;

namespace KoiTrading
{
    public partial class FishDetail : Window
    {
        private KoiFish fishItem;

        public FishDetail(KoiFish selectedFish)
        {
            InitializeComponent();
            fishItem = selectedFish;
            DataContext = selectedFish;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{fishItem.Origin} - {fishItem.Gender} has been added to your cart.", "Added to Cart", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var shopListWindow = new ShopList();
            shopListWindow.Show();
            this.Close();
        }
        
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var shopListWindow = new ShopList();
            shopListWindow.Show();
            this.Close();
        }
    }
}