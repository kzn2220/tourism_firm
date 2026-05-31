using System;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class EditClientDataWindow : Window
    {
        private readonly IClientAdapter _clientAdapter;
        private readonly Client _client;
        private readonly int _clientId;

        public EditClientDataWindow(IClientAdapter clientAdapter, Client client)
        {
            InitializeComponent();
            _clientAdapter = clientAdapter;
            _client = client;
            _clientId = client.ClientId;
            LoadData();
        }

        private void LoadData()
        {
            if (_client == null) return;
            LastNameTextBox.Text = _client.LastName;
            FirstNameTextBox.Text = _client.FirstName;
            MiddleNameTextBox.Text = _client.MiddleName;
            PhoneTextBox.Text = _client.Phone;
            EmailTextBox.Text = _client.Email;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastNameTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string middleName = MiddleNameTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();

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

            var updatedClient = new Client
            {
                ClientId = _clientId,
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                Phone = phone,
                Email = email
            };

            try
            {
                bool result = _clientAdapter.UpdateClient(updatedClient);
                if (result)
                {
                    MessageBox.Show("Данные обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка обновления данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}