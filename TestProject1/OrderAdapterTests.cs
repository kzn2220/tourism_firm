using Moq;
using tourism_firm.объекты;

namespace tourism_firm
{
    [TestClass]
    public class OrderAdapterTests
    {
        [TestMethod]
        public void CreateOrder_ValidData_ReturnsTrue()
        {
            var mock = new Mock<IOrderAdapter>();
            mock.Setup(a => a.CreateOrder(1, 10, 5, 2, 50000m)).Returns(true);

            bool result = mock.Object.CreateOrder(1, 10, 5, 2, 50000m);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CreateOrder_InvalidClientId_ReturnsFalse()
        {
            var mock = new Mock<IOrderAdapter>();
            mock.Setup(a => a.CreateOrder(0, 10, 5, 2, 50000m)).Returns(false);

            bool result = mock.Object.CreateOrder(0, 10, 5, 2, 50000m);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetAllOrders_ReturnsList()
        {
            var mock = new Mock<IOrderAdapter>();
            var orders = new List<Order>
            {
                new Order { OrderId = 1, ClientFullName = "Иванов Иван", TotalCost = 25000m },
                new Order { OrderId = 2, ClientFullName = "Петров Пётр", TotalCost = 30000m }
            };
            mock.Setup(a => a.GetAllOrders(false)).Returns(orders);

            var result = mock.Object.GetAllOrders(false);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(25000m, result[0].TotalCost);
        }

        [TestMethod]
        public void GetClientOrders_ReturnsOrdersForSpecificClient()
        {
            var mock = new Mock<IOrderAdapter>();
            var orders = new List<Order>
            {
                new Order { OrderId = 5, ClientId = 1, TotalCost = 12000m }
            };
            mock.Setup(a => a.GetClientOrders(1, true)).Returns(orders);

            var result = mock.Object.GetClientOrders(1, true);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(12000m, result[0].TotalCost);
        }
    }
}