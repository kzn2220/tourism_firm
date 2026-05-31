using System;
using System.Collections.Generic;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class EmployeeWindow : Window
    {
        private readonly ITourAdapter _tourAdapter;
        private readonly IClientAdapter _clientAdapter;
        private readonly IOrderAdapter _orderAdapter;
        private readonly IUserAdapter _userAdapter;
        private User _currentUser;

        public EmployeeWindow(IUserAdapter userAdapter, IClientAdapter clientAdapter, ITourAdapter tourAdapter, IOrderAdapter orderAdapter)
        {
            InitializeComponent();
            _userAdapter = userAdapter;
            _clientAdapter = clientAdapter;
            _tourAdapter = tourAdapter;
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

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTour = ToursDataGrid.SelectedItem as Tour;
            if (selectedTour == null)
            {
                MessageBox.Show("Выберите тур для оформления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!_currentUser.EmployeeId.HasValue)
            {
                MessageBox.Show("У вас нет привязанного сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int employeeId = _currentUser.EmployeeId.Value;
            if (employeeId <= 0)
            {
                MessageBox.Show("Некорректный идентификатор сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CreateOrderWindow orderWindow = new CreateOrderWindow(_orderAdapter, _clientAdapter, selectedTour, employeeId);
            orderWindow.Owner = this;
            if (orderWindow.ShowDialog() == true)
                LoadTours();
        }

        private void AllOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            AllOrdersWindow ordersWindow = new AllOrdersWindow(_orderAdapter, _currentUser);
            this.Close();
            ordersWindow.Show();
        }

        private void AllClientsButton_Click(object sender, RoutedEventArgs e)
        {
            AllClientsWindow clientsWindow = new AllClientsWindow(_clientAdapter, _currentUser);
            this.Close();
            clientsWindow.Show();
        }

        private void DropFilterButton_Click(object sender, RoutedEventArgs e)
        {
            CountryTextBox.Clear();
            MaxPriceTextBox.Clear();
            MinAmountSeatsTextBox.Clear();
            LoadTours();
        }

        private void BackToAutorizationButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}