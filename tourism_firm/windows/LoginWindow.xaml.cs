using System;
using System.Windows;
using tourism_firm.Interfaces;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class LoginWindow : Window
    {
        private readonly IUserAdapter _userAdapter;
        private readonly IClientAdapter _clientAdapter;
        private readonly IEmployeeAdapter _employeeAdapter;
        private readonly ITourAdapter _tourAdapter;
        private readonly IOrderAdapter _orderAdapter;
        private readonly IReportAdapter _reportAdapter;

        public LoginWindow()
        {
            InitializeComponent();
            // Ручное внедрение зависимостей (без DI-контейнера)
            _userAdapter = new UserAdapter();
            _clientAdapter = new ClientAdapter();
            _employeeAdapter = new EmployeeAdapter();
            _tourAdapter = new TourAdapter();
            _orderAdapter = new OrderAdapter();
            _reportAdapter = new ReportAdapter();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                User user = _userAdapter.Authenticate(login, password);
                if (user != null)
                {
                    if (user.Role == "Администратор")
                    {
                        AdminWindow adminWindow = new AdminWindow(_userAdapter, _employeeAdapter, _tourAdapter, _orderAdapter, _reportAdapter);
                        adminWindow.SetUser(user);
                        adminWindow.Show();
                        this.Close();
                    }
                    else if (user.Role == "Сотрудник")
                    {
                        EmployeeWindow employeeWindow = new EmployeeWindow(_userAdapter, _clientAdapter, _tourAdapter, _orderAdapter);
                        employeeWindow.SetUser(user);
                        employeeWindow.Show();
                        this.Close();
                    }
                    else if (user.Role == "Клиент")
                    {
                        ClientWindow clientWindow = new ClientWindow(_tourAdapter, _userAdapter, _clientAdapter, _orderAdapter);
                        clientWindow.SetUser(user);
                        clientWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(_userAdapter);
            registerWindow.Owner = this;
            registerWindow.ShowDialog();
        }
    }
}