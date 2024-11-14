using System.Windows;
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

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            var checkoutWindow = new CheckOut();
            checkoutWindow.Show();
            this.Close();
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