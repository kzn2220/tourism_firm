using System;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class EditTourWindow : Window
    {
        private readonly ITourAdapter _tourAdapter;
        private readonly Tour _tour;

        public EditTourWindow(ITourAdapter tourAdapter, Tour tour)
        {
            InitializeComponent();
            _tourAdapter = tourAdapter;
            _tour = tour;
            LoadData();
        }

        private void LoadData()
        {
            TourNameTextBox.Text = _tour.TourName;
            CountryTextBox.Text = _tour.Country;
            CityTextBox.Text = _tour.City;
            HotelTextBox.Text = _tour.Hotel;
            PriceTextBox.Text = _tour.Price.ToString();
            SeatsTextBox.Text = _tour.AvailableSeats.ToString();
            DepartureDateDatePicker.SelectedDate = _tour.DepartureDate;
            ReturnDateDatePicker.SelectedDate = _tour.ReturnDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TourNameTextBox.Text) || string.IsNullOrWhiteSpace(CountryTextBox.Text))
            {
                MessageBox.Show("Заполните название и страну", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string priceError = null;
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || !Validator.IsPriceValid(price, out priceError))
            {
                MessageBox.Show(priceError ?? "Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string seatsError = null;
            if (!int.TryParse(SeatsTextBox.Text, out int seats) || !Validator.IsSeatsValid(seats, out seatsError))
            {
                MessageBox.Show(seatsError ?? "Введите корректное количество мест", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            _tour.TourName = TourNameTextBox.Text.Trim();
            _tour.Country = CountryTextBox.Text.Trim();
            _tour.City = CityTextBox.Text.Trim();
            _tour.Hotel = HotelTextBox.Text.Trim();
            _tour.Price = price;
            _tour.AvailableSeats = seats;
            _tour.DepartureDate = departure;
            _tour.ReturnDate = returnDate;

            try
            {
                if (_tourAdapter.UpdateTour(_tour))
                {
                    MessageBox.Show("Тур обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка обновления тура", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}