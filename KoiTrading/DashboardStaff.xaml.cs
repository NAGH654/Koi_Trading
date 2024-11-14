using System.IO;
using System.Windows;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;

namespace KoiTrading
{
    public partial class DashboardStaff : Window
    {
        private readonly KoiFishService _koiFishService;
        private readonly CategoryService _categoryService;
        private byte[] _koiImageData;

        public DashboardStaff() 
        {
            InitializeComponent();
            var context = new KoiFishTradingContext();
            var repository = new KoiFishRepository(context);
            var repository2 = new CategoryRepository(context);
            _koiFishService = new KoiFishService(repository);
            _categoryService = new CategoryService(repository2);
            
            LoadCategoriesAsync();
        }
        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategories();
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "CategoryName"; 
            CategoryComboBox.SelectedValuePath = "CategoryId";  
        }
        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _koiImageData = File.ReadAllBytes(openFileDialog.FileName);
                MessageBox.Show("Image selected successfully!");
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(CategoryComboBox.SelectedValue?.ToString(), out int categoryId);
                string origin = OriginTextBox.Text;
                string gender = GenderTextBox.Text;
                int.TryParse(AgeTextBox.Text, out int age);
                decimal.TryParse(SizeTextBox.Text, out decimal size);
                string status = StatusTextBox.Text;
                decimal.TryParse(PriceTextBox.Text, out decimal price);
                string health = HealthTextBox.Text;
                
                var koiFish = new KoiFish
                {
                    CategoryId = categoryId,
                    Origin = origin,
                    Gender = gender,
                    Age = age,
                    Size = size,
                    Status = status,
                    Price = price,
                    Health = health,
                    KoiImage = _koiImageData 
                };
                
                bool isAdded = await _koiFishService.AddKoiFishAsync(koiFish);

                if (isAdded)
                {
                    MessageBox.Show("Koi Fish added successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to add Koi Fish.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
