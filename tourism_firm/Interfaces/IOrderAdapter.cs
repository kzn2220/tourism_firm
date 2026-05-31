using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_firm.объекты;

namespace tourism_firm
{
    public interface IOrderAdapter
    {
        bool CreateOrder(int clientId, int tourId, int employeeId, int seatsAmount, decimal totalCost);
        List<Order> GetClientOrders(int clientId, bool includeDeleted = true);
        List<Order> GetAllOrders(bool includeDeleted = false);
        bool DeleteOrder(int orderId);
    }
}
