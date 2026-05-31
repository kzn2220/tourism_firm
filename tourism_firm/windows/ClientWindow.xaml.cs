using System;
using System.Collections.Generic;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class ClientWindow : Window
    {
        private readonly ITourAdapter _tourAdapter;
        private readonly IUserAdapter _userAdapter;
        private readonly IClientAdapter _clientAdapter;
        private readonly IOrderAdapter _orderAdapter;
        private User _currentUser;

        public ClientWindow(ITourAdapter tourAdapter, IUserAdapter userAdapter, IClientAdapter clientAdapter, IOrderAdapter orderAdapter)
        {
            InitializeComponent();
            _tourAdapter = tourAdapter;
            _userAdapter = userAdapter;
            _clientAdapter = clientAdapter;
            _orderAdapter = orderAdapter;
        }

        public void SetUser(User user)
        {
            _currentUser = user;
            LoadTours();
        }

        private void LoadTours()
        {
            try
            {
                List<Tour> tours = _tourAdapter.GetAllTours();
                ToursDataGrid.ItemsSource = tours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string country = CountryTextBox.Text.Trim();
                decimal? maxPrice = null;
                if (!string.IsNullOrEmpty(MaxPriceTextBox.Text) && decimal.TryParse(MaxPriceTextBox.Text, out decimal price))
                    maxPrice = price;
                int? minSeats = null;
                if (!string.IsNullOrEmpty(MinAmountSeatsTextBox.Text) && int.TryParse(MinAmountSeatsTextBox.Text, out int seats))
                    minSeats = seats;
                List<Tour> tours = _tourAdapter.SearchTours(country, maxPrice, minSeats);
                ToursDataGrid.ItemsSource = tours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.ClientId.HasValue)
            {
                ClientProfileWindow profileWindow = new ClientProfileWindow(_clientAdapter, _orderAdapter, _userAdapter, _currentUser);
                profileWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Нет данных о клиенте", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DropFilterButton_Click(object sender, RoutedEventArgs e)
        {
            CountryTextBox.Clear();
            MaxPriceTextBox.Clear();
            MinAmountSeatsTextBox.Clear();
            LoadTours();
        }
    }
}