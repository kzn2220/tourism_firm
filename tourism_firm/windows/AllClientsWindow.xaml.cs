using System;
using System.Collections.Generic;
using System.Windows;
using tourism_firm.windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class AllClientsWindow : Window
    {
        private readonly IClientAdapter _clientAdapter;
        private readonly IUserAdapter _userAdapter;
        private readonly User _currentUser;

        public AllClientsWindow(IClientAdapter clientAdapter, User user)
        {
            InitializeComponent();
            _clientAdapter = clientAdapter;
            _currentUser = user;
            _userAdapter = new UserAdapter();
            LoadClients();
        }

        public AllClientsWindow(IClientAdapter clientAdapter, IUserAdapter userAdapter, User user) : this(clientAdapter, user)
        {
            _userAdapter = userAdapter;
        }

        private void LoadClients()
        {
            try
            {
                List<Client> clients = _clientAdapter.GetAllClients();
                ClientsDataGrid.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterClientButton_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow addClientWindow = new AddClientWindow(_userAdapter);
            addClientWindow.Owner = this;
            addClientWindow.ShowDialog();
            LoadClients();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsDataGrid.SelectedItem as Client;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить клиента {selectedClient.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (_clientAdapter.DeleteClient(selectedClient.ClientId))
                    {
                        MessageBox.Show("Клиент удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadClients();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка удаления клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsDataGrid.SelectedItem as Client;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditClientFromEmployeeWindow editWindow = new EditClientFromEmployeeWindow(_clientAdapter, selectedClient);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
                LoadClients();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeWindow employeeWindow = new EmployeeWindow(_userAdapter, _clientAdapter, new TourAdapter(), new OrderAdapter());
            employeeWindow.SetUser(_currentUser);
            employeeWindow.Show();
            this.Close();
        }
    }
}