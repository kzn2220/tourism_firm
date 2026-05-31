using System;
using System.Collections.Generic;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class ClientProfileWindow : Window
    {
        private readonly IClientAdapter _clientAdapter;
        private readonly IOrderAdapter _orderAdapter;
        private readonly IUserAdapter _userAdapter;
        private readonly int _clientId;
        private Client _currentClient;
        private readonly User _user;

        public ClientProfileWindow(IClientAdapter clientAdapter, IOrderAdapter orderAdapter, IUserAdapter userAdapter, User user)
        {
            InitializeComponent();
            _clientAdapter = clientAdapter;
            _orderAdapter = orderAdapter;
            _userAdapter = userAdapter;
            _user = user;
            _clientId = user.ClientId.Value;
            LoadClientData();
            LoadOrders();
        }

        private void LoadClientData()
        {
            try
            {
                _currentClient = _clientAdapter.GetClientById(_clientId);
                if (_currentClient != null)
                {
                    LastNameTextBox.Text = _currentClient.LastName;
                    FirstNameTextBox.Text = _currentClient.FirstName;
                    MiddleNameTextBox.Text = _currentClient.MiddleName;
                    PhoneTextBox.Text = _currentClient.Phone;
                    EmailTextBox.Text = _currentClient.Email;
                    LoginTextBox.Text = _user.Login;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                List<Order> orders = _orderAdapter.GetClientOrders(_clientId, includeDeleted: true);
                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentClient == null || _currentClient.ClientId <= 0)
            {
                MessageBox.Show("Ошибка: данные клиента не загружены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            EditClientDataWindow editWindow = new EditClientDataWindow(_clientAdapter, _currentClient);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
            {
                LoadClientData();
                LoadOrders();
            }
        }

        private void BackToAutorization_Button(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow(new TourAdapter(), _userAdapter, _clientAdapter, _orderAdapter);
            clientWindow.SetUser(_user);
            clientWindow.Show();
            this.Close();
        }
    }
}