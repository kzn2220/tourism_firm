using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using tourism_firm.Interfaces;
using tourism_firm.windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class AdminWindow : Window
    {
        private readonly IEmployeeAdapter _employeeAdapter;
        private readonly IReportAdapter _reportAdapter;
        private readonly IUserAdapter _userAdapter;
        private readonly ITourAdapter _tourAdapter;
        private readonly IOrderAdapter _orderAdapter;
        private User _currentUser;

        public AdminWindow(IUserAdapter userAdapter, IEmployeeAdapter employeeAdapter, ITourAdapter tourAdapter, IOrderAdapter orderAdapter, IReportAdapter reportAdapter)
        {
            InitializeComponent();
            _userAdapter = userAdapter;
            _employeeAdapter = employeeAdapter;
            _tourAdapter = tourAdapter;
            _orderAdapter = orderAdapter;
            _reportAdapter = reportAdapter;
        }

        public void SetUser(User user)
        {
            _currentUser = user;
            FromDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            ToDatePicker.SelectedDate = DateTime.Now;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeAdapter.GetAllEmployees();
                var employeeList = new List<Employee>();
                foreach (var emp in employees)
                {
                    var empInfo = new Employee
                    {
                        EmployeeId = emp.EmployeeId,
                        LastName = emp.LastName,
                        FirstName = emp.FirstName,
                        MiddleName = emp.MiddleName,
                        Phone = emp.Phone,
                        Email = emp.Email,
                        Login = GetLoginByEmployeeId(emp.EmployeeId),
                        RoleName = GetRoleByEmployeeId(emp.EmployeeId)
                    };
                    employeeList.Add(empInfo);
                }
                EmployeesDataGrid.ItemsSource = employeeList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}");
            }
        }

        private string GetLoginByEmployeeId(int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new System.Data.OleDb.OleDbCommand(
                    "SELECT login FROM users WHERE employee_id = @id", connection);
                command.Parameters.AddWithValue("@id", employeeId);
                var result = command.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }

        private string GetRoleByEmployeeId(int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new System.Data.OleDb.OleDbCommand(
                    "SELECT role FROM users WHERE employee_id = @id", connection);
                command.Parameters.AddWithValue("@id", employeeId);
                var result = command.ExecuteScalar();
                return result?.ToString() ?? "";
            }
        }

        private void RegisterEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow(_employeeAdapter, _userAdapter);
            addEmployeeWindow.Owner = this;
            addEmployeeWindow.ShowDialog();
            LoadEmployees();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить сотрудника {selectedEmployee.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (_employeeAdapter.DeleteEmployee(selectedEmployee.EmployeeId))
                    {
                        MessageBox.Show("Сотрудник удалён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployees();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка удаления сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FromDatePicker.SelectedDate.HasValue || !ToDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime fromDate = FromDatePicker.SelectedDate.Value;
            DateTime toDate = ToDatePicker.SelectedDate.Value;
            string reportType = (ReportTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(reportType)) return;

            try
            {
                string reportContent = "";
                switch (reportType)
                {
                    case "Продажи по турам":
                        reportContent = _reportAdapter.GenerateSalesReport(fromDate, toDate);
                        break;
                    case "Статистика сотрудников":
                        reportContent = _reportAdapter.GenerateEmployeeStatsReport(fromDate, toDate);
                        break;
                }

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt",
                    Title = "Сохранить отчёт",
                    FileName = $"Отчёт_{reportType}_{fromDate:yyyyMMdd}-{toDate:yyyyMMdd}.txt"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveDialog.FileName, reportContent);
                    MessageBox.Show($"Отчёт сохранён: {saveDialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageToursButton_Click(object sender, RoutedEventArgs e)
        {
            AdminToursWindow toursWindow = new AdminToursWindow(_tourAdapter, _currentUser);
            toursWindow.Show();
            this.Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditEmployeeWindow editWindow = new EditEmployeeWindow(_employeeAdapter, _userAdapter, selectedEmployee);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
                LoadEmployees();
        }

        private void BackToAutorizationButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}