using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class CreateOrderWindow : Window
    {
        private readonly IOrderAdapter _orderAdapter;
        private readonly IClientAdapter _clientAdapter;
        private readonly Tour _selectedTour;
        private int _selectedClientId;
        private readonly int _employeeId;

        public CreateOrderWindow(IOrderAdapter orderAdapter, IClientAdapter clientAdapter, Tour tour, int employeeId)
        {
            InitializeComponent();
            _orderAdapter = orderAdapter;
            _clientAdapter = clientAdapter;
            _employeeId = employeeId;
            _selectedTour = tour;
            TourNameTextBox.Text = tour.TourName;
            SeatsAmountTextBox.Text = "1";
            LoadClients();
            UpdateTotalPrice();
        }

        private void LoadClients()
        {
            try
            {
                List<Client> clients = _clientAdapter.GetAllClients();
                ClientComboBox.ItemsSource = clients;
                ClientComboBox.DisplayMemberPath = "FullName";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedItem is Client selectedClient)
            {
                _selectedClientId = selectedClient.ClientId;
            }
            UpdateTotalPrice();
        }

        private void DiscountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void SeatsAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            if (_selectedTour == null) return;
            if (!int.TryParse(SeatsAmountTextBox.Text, out int seatsAmount) || seatsAmount <= 0)
                seatsAmount = 1;

            decimal pricePerSeat = _selectedTour.Price;
            var selectedItem = DiscountComboBox.SelectedItem as ComboBoxItem;
            string discountText = selectedItem?.Content.ToString() ?? "Нет скидки";

            if (discountText.Contains("10%"))
                pricePerSeat *= 0.9m;
            else if (discountText.Contains("15%"))
                pricePerSeat *= 0.85m;
            else if (discountText.Contains("5%"))
                pricePerSeat *= 0.95m;
            else if (discountText.Contains("8%"))
                pricePerSeat *= 0.92m;

            decimal total = pricePerSeat * seatsAmount;
            TotalPriceText.Text = $"{total:N2} руб.";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedClientId == 0)
            {
                MessageBox.Show("Выберите клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(SeatsAmountTextBox.Text, out int seatsAmount) || seatsAmount <= 0)
            {
                MessageBox.Show("Введите корректное количество мест", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (seatsAmount > _selectedTour.AvailableSeats)
            {
                MessageBox.Show($"Доступно только {_selectedTour.AvailableSeats} мест", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_employeeId <= 0)
            {
                MessageBox.Show("Не указан сотрудник", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal pricePerSeat = _selectedTour.Price;
            var selectedItem = DiscountComboBox.SelectedItem as ComboBoxItem;
            string discountText = selectedItem?.Content.ToString() ?? "Нет скидки";

            if (discountText.Contains("10%"))
                pricePerSeat *= 0.9m;
            else if (discountText.Contains("15%"))
                pricePerSeat *= 0.85m;
            else if (discountText.Contains("5%"))
                pricePerSeat *= 0.95m;
            else if (discountText.Contains("8%"))
                pricePerSeat *= 0.92m;

            decimal totalCost = pricePerSeat * seatsAmount;

            bool result = _orderAdapter.CreateOrder(_selectedClientId, _selectedTour.TourId, _employeeId, seatsAmount, totalCost);
            if (result)
            {
                MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка оформления заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}