using Moq;
using tourism_firm.объекты;

namespace tourism_firm
{
    [TestClass]
    public class TourAdapterTests
    {
        [TestMethod]
        public void SearchTours_ByCountry_ReturnsFilteredList()
        {
            var mock = new Mock<ITourAdapter>();
            var expected = new List<Tour>
            {
                new Tour { TourId = 1, Country = "Турция", Price = 50000m },
                new Tour { TourId = 2, Country = "Турция", Price = 60000m }
            };
            mock.Setup(a => a.SearchTours("Турция", null, null)).Returns(expected);

            var result = mock.Object.SearchTours("Турция", null, null);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Турция", result[0].Country);
        }

        [TestMethod]
        public void SearchTours_WithMaxPrice_ReturnsFiltered()
        {
            var mock = new Mock<ITourAdapter>();
            var expected = new List<Tour> { new Tour { Price = 40000m } };
            mock.Setup(a => a.SearchTours("", 50000m, null)).Returns(expected);

            var result = mock.Object.SearchTours("", 50000m, null);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(40000m, result[0].Price);
        }

        [TestMethod]
        public void GetAllTours_ReturnsOnlyAvailable()
        {
            var mock = new Mock<ITourAdapter>();
            var tours = new List<Tour>
            {
                new Tour { AvailableSeats = 5 },
                new Tour { AvailableSeats = 0 }
            };
            mock.Setup(a => a.GetAllTours()).Returns(tours);

            var result = mock.Object.GetAllTours();

            Assert.AreEqual(2, result.Count);
        }
    }
}