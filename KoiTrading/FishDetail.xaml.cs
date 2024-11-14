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

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var checkOutWindow = new CheckOut(fishItem);
            checkOutWindow.Show();
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