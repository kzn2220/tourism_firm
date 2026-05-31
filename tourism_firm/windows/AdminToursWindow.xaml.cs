using System;
using System.Collections.Generic;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class AdminToursWindow : Window
    {
        private readonly ITourAdapter _tourAdapter;
        private readonly User _currentUser;

        public AdminToursWindow(ITourAdapter tourAdapter, User user)
        {
            InitializeComponent();
            _tourAdapter = tourAdapter;
            _currentUser = user;
            DepartureDateDatePicker.SelectedDate = DateTime.Now.AddDays(30);
            ReturnDateDatePicker.SelectedDate = DateTime.Now.AddDays(37);
            LoadTours();
        }

        private void LoadTours()
        {
            try
            {
                List<Tour> tours = _tourAdapter.GetAllToursForAdmin();
                ToursDataGrid.ItemsSource = tours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTourButton_Click(object sender, RoutedEventArgs e)
        {
            string tourName = TourNameTextBox.Text.Trim();
            string country = CountryTextBox.Text.Trim();
            string city = CityTextBox.Text.Trim();
            string hotel = HotelTextBox.Text.Trim();

            if (string.IsNullOrEmpty(tourName) || string.IsNullOrEmpty(country))
            {
                MessageBox.Show("Заполните название и страну", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsPriceValid(price, out string priceError))
            {
                MessageBox.Show(priceError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(AvailableSeatsTextBox.Text, out int seats))
            {
                MessageBox.Show("Введите корректное количество мест", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsSeatsValid(seats, out string seatsError))
            {
                MessageBox.Show(seatsError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DepartureDateDatePicker.SelectedDate.HasValue || !ReturnDateDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите даты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime departure = DepartureDateDatePicker.SelectedDate.Value;
            DateTime returnDate = ReturnDateDatePicker.SelectedDate.Value;
            if (!Validator.IsDatesValid(departure, returnDate, out string dateError))
            {
                MessageBox.Show(dateError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Tour newTour = new Tour
                {
                    TourName = tourName,
                    Country = country,
                    City = city,
                    Hotel = hotel,
                    Price = price,
                    AvailableSeats = seats,
                    DepartureDate = departure,
                    ReturnDate = returnDate
                };

                if (_tourAdapter.AddTour(newTour))
                {
                    MessageBox.Show("Тур добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    TourNameTextBox.Clear();
                    CountryTextBox.Clear();
                    CityTextBox.Clear();
                    HotelTextBox.Clear();
                    PriceTextBox.Clear();
                    AvailableSeatsTextBox.Clear();
                    LoadTours();
                }
                else
                {
                    MessageBox.Show("Ошибка добавления тура", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteTourButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTour = ToursDataGrid.SelectedItem as Tour;
            if (selectedTour == null)
            {
                MessageBox.Show("Выберите тур для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить тур '{selectedTour.TourName}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_tourAdapter.DeleteTour(selectedTour.TourId))
                {
                    MessageBox.Show("Тур помечен как архивный", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadTours();
                }
                else
                {
                    MessageBox.Show("Ошибка удаления тура", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditTourButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTour = ToursDataGrid.SelectedItem as Tour;
            if (selectedTour == null)
            {
                MessageBox.Show("Выберите тур для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditTourWindow editWindow = new EditTourWindow(_tourAdapter, selectedTour);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
                LoadTours();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow(new UserAdapter(), new EmployeeAdapter(), _tourAdapter, new OrderAdapter(), new ReportAdapter());
            adminWindow.SetUser(_currentUser);
            adminWindow.Show();
            this.Close();
        }
    }
}