using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tourism_firm.объекты;

namespace tourism_firm
{
    public interface IEmployeeAdapter
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(int employeeId);
        bool AddEmployee(Employee employee);
        bool UpdateEmployee(Employee employee);
        bool DeleteEmployee(int employeeId);
        int GetLastEmployeeId();
    }
}
