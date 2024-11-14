using System.IO;
using System.Windows;
using System.Windows.Controls;
using KoiTradding.BLL.Services;
using KoiTradding.DAL.Models;
using KoiTradding.DAL.Repositories;
using Microsoft.Win32;

namespace KoiTrading;

public partial class DashboardStaff : Window
{
    private readonly CategoryService _categoryService;
    private readonly KoiFishService _koiFishService;
    private byte[] _koiImageData;

    public DashboardStaff()
    {
        InitializeComponent();
        var context = new KoiFishTradingContext();
        var repository = new KoiFishRepository(context);
        var repository2 = new CategoryRepository(context);
        _koiFishService = new KoiFishService(repository);
        _categoryService = new CategoryService(repository2);


        LoadKoiDataAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _categoryService.GetAllCategories();
        CategoryComboBox.ItemsSource = categories;
        CategoryComboBox.DisplayMemberPath = "CategoryName";
        CategoryComboBox.SelectedValuePath = "CategoryId";
    }

    private async Task LoadKoiDataAsync()
    {
        var koiFishList = await _koiFishService.GetAllKoiFishAsync();
        KoiDataGrid.ItemsSource = koiFishList;
        await LoadCategoriesAsync();
    }

    private void KoiDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (KoiDataGrid.SelectedItem is KoiFish selectedKoiFish)
        {
            OriginTextBox.Text = selectedKoiFish.Origin;
            GenderTextBox.Text = selectedKoiFish.Gender;
            AgeTextBox.Text = selectedKoiFish.Age.ToString();
            SizeTextBox.Text = selectedKoiFish.Size.ToString();
            StatusTextBox.Text = selectedKoiFish.Status;
            PriceTextBox.Text = selectedKoiFish.Price.ToString();
            HealthTextBox.Text = selectedKoiFish.Health;
            CategoryComboBox.SelectedValue = selectedKoiFish.CategoryId;
            _koiImageData = selectedKoiFish.KoiImage;
        }
    }

    private void SelectImageButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
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
            int.TryParse(CategoryComboBox.SelectedValue?.ToString(), out var categoryId);
            var origin = OriginTextBox.Text;
            var gender = GenderTextBox.Text;
            int.TryParse(AgeTextBox.Text, out var age);
            decimal.TryParse(SizeTextBox.Text, out var size);
            var status = StatusTextBox.Text;
            decimal.TryParse(PriceTextBox.Text, out var price);
            var health = HealthTextBox.Text;

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

            var isAdded = await _koiFishService.AddKoiFishAsync(koiFish);

            if (isAdded)
                MessageBox.Show("Koi Fish added successfully!");
            else
                MessageBox.Show("Failed to add Koi Fish.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (KoiDataGrid.SelectedItem is KoiFish selectedKoiFish)
            try
            {
                selectedKoiFish.Origin = OriginTextBox.Text;
                selectedKoiFish.Gender = GenderTextBox.Text;
                selectedKoiFish.Age = int.TryParse(AgeTextBox.Text, out var age) ? age : selectedKoiFish.Age;
                selectedKoiFish.Size = decimal.TryParse(SizeTextBox.Text, out var size) ? size : selectedKoiFish.Size;
                selectedKoiFish.Status = StatusTextBox.Text;
                selectedKoiFish.Price =
                    decimal.TryParse(PriceTextBox.Text, out var price) ? price : selectedKoiFish.Price;
                selectedKoiFish.Health = HealthTextBox.Text;
                selectedKoiFish.CategoryId = (int?)CategoryComboBox.SelectedValue ?? selectedKoiFish.CategoryId;

                if (_koiImageData != null) selectedKoiFish.KoiImage = _koiImageData;

                var isUpdated = await _koiFishService.UpdateKoiFishAsync(selectedKoiFish);

                if (isUpdated)
                {
                    MessageBox.Show("Koi Fish updated successfully.", "Update Successful", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    await LoadKoiDataAsync();
                }
                else
                {
                    MessageBox.Show("Failed to update Koi Fish.", "Update Failed", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        else
            MessageBox.Show("Please select a koi fish to edit.", "No Selection", MessageBoxButton.OK,
                MessageBoxImage.Warning);
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (KoiDataGrid.SelectedItem is KoiFish selectedKoiFish)
        {
            var result = MessageBox.Show("Are you sure you want to delete this koi fish?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                try
                {
                    var isDeleted = await _koiFishService.DeleteKoiFishAsync(selectedKoiFish.KoiId);

                    if (isDeleted)
                    {
                        MessageBox.Show("Koi Fish deleted successfully.", "Deleted", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        await LoadKoiDataAsync();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete koi fish.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        else
        {
            MessageBox.Show("Please select a koi fish to delete.", "No Selection", MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private async void QuitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ClearForm()
    {
        OriginTextBox.Text = string.Empty;
        GenderTextBox.Text = string.Empty;
        AgeTextBox.Text = string.Empty;
        SizeTextBox.Text = string.Empty;
        StatusTextBox.Text = string.Empty;
        PriceTextBox.Text = string.Empty;
        HealthTextBox.Text = string.Empty;
        CategoryComboBox.SelectedIndex = -1;
        _koiImageData = null;
    }
}