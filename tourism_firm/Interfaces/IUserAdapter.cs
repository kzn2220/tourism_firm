using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_firm.объекты;

namespace tourism_firm
{
    public interface IUserAdapter
    {
        User Authenticate(string login, string password);
        bool RegisterClient(string login, string password, Client client);
        bool RegisterEmployee(string login, string password, int employeeId);
        bool RegisterAdmin(string login, string password, int employeeId);
        bool UpdateClientData(int clientId, string lastName, string firstName, string middleName, string phone, string email);
        bool UpdatePassword(int userId, string newPassword);
        bool UpdateEmployeeRole(int employeeId, string role);
    }
}
