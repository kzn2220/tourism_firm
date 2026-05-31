using Moq;
using tourism_firm.Interfaces;

namespace tourism_firm
{
    [TestClass]
    public class ReportAdapterTests
    {
        [TestMethod]
        public void GenerateSalesReport_ReturnsNonEmptyString()
        {
            var mock = new Mock<IReportAdapter>();
            mock.Setup(r => r.GenerateSalesReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns("Отчёт по продажам");

            string report = mock.Object.GenerateSalesReport(DateTime.Now.AddMonths(-1), DateTime.Now);

            Assert.IsFalse(string.IsNullOrEmpty(report));
            Assert.IsTrue(report.Contains("Отчёт"));
        }

        [TestMethod]
        public void GenerateEmployeeStatsReport_ReturnsNonEmptyString()
        {
            var mock = new Mock<IReportAdapter>();
            mock.Setup(r => r.GenerateEmployeeStatsReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns("Статистика сотрудников");

            string report = mock.Object.GenerateEmployeeStatsReport(DateTime.Now.AddMonths(-1), DateTime.Now);

            Assert.IsFalse(string.IsNullOrEmpty(report));
        }
    }
}