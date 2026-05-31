using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tourism_firm.объекты
{
    public class Tour
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Hotel { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Status => IsDeleted ? "Да" : "";
    }
}
