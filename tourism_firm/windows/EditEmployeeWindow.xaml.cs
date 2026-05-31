using System;
using System.Windows;
using System.Windows.Controls;
using tourism_firm.объекты;

namespace tourism_firm.windows
{
    public partial class EditEmployeeWindow : Window
    {
        private readonly IEmployeeAdapter _employeeAdapter;
        private readonly IUserAdapter _userAdapter;
        private readonly Employee _employee;

        public EditEmployeeWindow(IEmployeeAdapter employeeAdapter, IUserAdapter userAdapter, Employee employee)
        {
            InitializeComponent();
            _employeeAdapter = employeeAdapter;
            _userAdapter = userAdapter;
            _employee = employee;
            LoadData();
        }

        private void LoadData()
        {
            LastNameTextBox.Text = _employee.LastName;
            FirstNameTextBox.Text = _employee.FirstName;
            MiddleNameTextBox.Text = _employee.MiddleName;
            PhoneTextBox.Text = _employee.Phone;
            EmailTextBox.Text = _employee.Email;
            if (_employee.RoleName == "Администратор")
                RoleComboBox.SelectedIndex = 1;
            else
                RoleComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _employee.LastName = LastNameTextBox.Text.Trim();
            _employee.FirstName = FirstNameTextBox.Text.Trim();
            _employee.MiddleName = MiddleNameTextBox.Text.Trim();
            _employee.Phone = PhoneTextBox.Text.Trim();
            _employee.Email = EmailTextBox.Text.Trim();
            string newRole = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Сотрудник";

            try
            {
                if (_employeeAdapter.UpdateEmployee(_employee))
                {
                    if (_employee.RoleName != newRole)
                        _userAdapter.UpdateEmployeeRole(_employee.EmployeeId, newRole);
                    MessageBox.Show("Данные сотрудника обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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