using Moq;
using tourism_firm.объекты;

namespace tourism_firm
{
    [TestClass]
    public class UserAdapterTests
    {
        [TestMethod]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            var mock = new Mock<IUserAdapter>();
            var expectedUser = new User { UserId = 1, Login = "admin", Role = "Администратор" };
            mock.Setup(a => a.Authenticate("admin", "123")).Returns(expectedUser);

            var result = mock.Object.Authenticate("admin", "123");

            Assert.IsNotNull(result);
            Assert.AreEqual("Администратор", result.Role);
        }

        [TestMethod]
        public void Authenticate_InvalidCredentials_ReturnsNull()
        {
            var mock = new Mock<IUserAdapter>();
            mock.Setup(a => a.Authenticate("wrong", "pass")).Returns((User)null);

            var result = mock.Object.Authenticate("wrong", "pass");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void RegisterClient_NewClient_ReturnsTrue()
        {
            var mock = new Mock<IUserAdapter>();
            var client = new Client { LastName = "Новый", FirstName = "Клиент" };
            mock.Setup(a => a.RegisterClient("newlogin", "pass", client)).Returns(true);

            bool result = mock.Object.RegisterClient("newlogin", "pass", client);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RegisterEmployee_ValidData_ReturnsTrue()
        {
            var mock = new Mock<IUserAdapter>();
            mock.Setup(a => a.RegisterEmployee("empl", "pass", 100)).Returns(true);

            bool result = mock.Object.RegisterEmployee("empl", "pass", 100);

            Assert.IsTrue(result);
        }
    }
}