using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tourism_firm.объекты
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public int TourId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalCost { get; set; }

        public string ClientFullName { get; set; }
        public string EmployeeFullName { get; set; }
        public string TourName { get; set; }
        public DateTime TourStartDate { get; set; }
        public DateTime TourEndDate { get; set; }
        public int AvailableSeats { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Hotel { get; set; }
        public int SeatsAmount { get; set; }
    }
}
