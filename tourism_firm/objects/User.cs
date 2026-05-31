using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tourism_firm.объекты
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int? ClientId { get; set; }
        public int? EmployeeId { get; set; }
    }
}
