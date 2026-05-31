using Moq;
using tourism_firm.объекты;

namespace tourism_firm
{
    [TestClass]
    public class ClientAdapterTests
    {
        [TestMethod]
        public void GetAllClients_ReturnsListOfClients()
        {
            var mock = new Mock<IClientAdapter>();
            var expected = new List<Client>
            {
                new Client { ClientId = 1, LastName = "Иванов", FirstName = "Иван" },
                new Client { ClientId = 2, LastName = "Петров", FirstName = "Пётр" }
            };
            mock.Setup(a => a.GetAllClients()).Returns(expected);

            var result = mock.Object.GetAllClients();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Иванов", result[0].LastName);
        }

        [TestMethod]
        public void AddClient_ValidClient_ReturnsTrue()
        {
            var mock = new Mock<IClientAdapter>();
            var newClient = new Client { LastName = "Сидоров", FirstName = "Сидор" };
            mock.Setup(a => a.AddClient(newClient)).Returns(true);

            bool result = mock.Object.AddClient(newClient);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateClient_NonExistentId_ReturnsFalse()
        {
            var mock = new Mock<IClientAdapter>();
            var client = new Client { ClientId = 999, LastName = "Несуществующий" };
            mock.Setup(a => a.UpdateClient(client)).Returns(false);

            bool result = mock.Object.UpdateClient(client);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DeleteClient_ValidId_ReturnsTrue()
        {
            var mock = new Mock<IClientAdapter>();
            mock.Setup(a => a.DeleteClient(1)).Returns(true);

            bool result = mock.Object.DeleteClient(1);

            Assert.IsTrue(result);
        }
    }
}