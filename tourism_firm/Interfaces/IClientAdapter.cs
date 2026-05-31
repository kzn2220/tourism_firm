using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_firm.объекты;

namespace tourism_firm
{
    public interface IClientAdapter
    {
        List<Client> GetAllClients();
        Client GetClientById(int clientId);
        bool AddClient(Client client);
        bool UpdateClient(Client client);
        bool DeleteClient(int clientId);
        int GetClientIdByFullName(string fullName);
    }
}
