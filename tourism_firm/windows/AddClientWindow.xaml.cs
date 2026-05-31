using System;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class AddClientWindow : Window
    {
        private readonly IUserAdapter _userAdapter;

        public AddClientWindow(IUserAdapter userAdapter)
        {
            InitializeComponent();
            _userAdapter = userAdapter;
        }

        private void RegisterClientButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string lastName = LastNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();

            if (!Validator.IsLoginValid(login, out string loginError))
            {
                MessageBox.Show(loginError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsPasswordValid(password, out string passError))
            {
                MessageBox.Show(passError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsNameValid(lastName, "Фамилия", out string lastNameError))
            {
                MessageBox.Show(lastNameError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsNameValid(firstName, "Имя", out string firstNameError))
            {
                MessageBox.Show(firstNameError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(middleName) && !Validator.IsNameValid(middleName, "Отчество", out string middleError))
            {
                MessageBox.Show(middleError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsPhoneValid(phone, out string phoneError))
            {
                MessageBox.Show(phoneError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Validator.IsEmailValid(email, out string emailError))
            {
                MessageBox.Show(emailError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Client newClient = new Client
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName,
                    Phone = phone,
                    Email = email
                };

                bool result = _userAdapter.RegisterClient(login, password, newClient);

                if (result)
                {
                    MessageBox.Show("Клиент успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка регистрации. Возможно, логин уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}