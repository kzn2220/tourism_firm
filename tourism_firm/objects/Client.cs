using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tourism_firm.объекты
{
    public class Client
    {
        public int ClientId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
