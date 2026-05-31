using System;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm.windows
{
    public partial class EditClientFromEmployeeWindow : Window
    {
        private readonly IClientAdapter _clientAdapter;
        private readonly Client _client;

        public EditClientFromEmployeeWindow(IClientAdapter clientAdapter, Client client)
        {
            InitializeComponent();
            _clientAdapter = clientAdapter;
            _client = client;
            LoadData();
        }

        private void LoadData()
        {
            LastNameTextBox.Text = _client.LastName;
            FirstNameTextBox.Text = _client.FirstName;
            MiddleNameTextBox.Text = _client.MiddleName;
            PhoneTextBox.Text = _client.Phone;
            EmailTextBox.Text = _client.Email;
        }

        private void SaveClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _client.LastName = LastNameTextBox.Text.Trim();
            _client.FirstName = FirstNameTextBox.Text.Trim();
            _client.MiddleName = MiddleNameTextBox.Text.Trim();
            _client.Phone = PhoneTextBox.Text.Trim();
            _client.Email = EmailTextBox.Text.Trim();

            try
            {
                if (_clientAdapter.UpdateClient(_client))
                {
                    MessageBox.Show("Данные клиента обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка обновления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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