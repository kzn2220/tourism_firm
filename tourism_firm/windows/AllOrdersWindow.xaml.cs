using System;
using System.Collections.Generic;
using System.Windows;
using tourism_firm.объекты;

namespace tourism_firm
{
    public partial class AllOrdersWindow : Window
    {
        private readonly IOrderAdapter _orderAdapter;
        private readonly User _currentUser;

        public AllOrdersWindow(IOrderAdapter orderAdapter, User user)
        {
            InitializeComponent();
            _orderAdapter = orderAdapter;
            _currentUser = user;
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                List<Order> orders = _orderAdapter.GetAllOrders(includeDeleted: false);
                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = OrdersDataGrid.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить заказ №{selectedOrder.OrderId}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_orderAdapter.DeleteOrder(selectedOrder.OrderId))
                {
                    MessageBox.Show("Заказ помечен как удалённый", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrders();
                }
                else
                {
                    MessageBox.Show("Ошибка удаления заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeWindow employeeWindow = new EmployeeWindow(new UserAdapter(), new ClientAdapter(), new TourAdapter(), _orderAdapter);
            employeeWindow.SetUser(_currentUser);
            employeeWindow.Show();
            this.Close();
        }
    }
}