using System;
using System.Collections.Generic;
using System.Data.OleDb;
using tourism_firm.Interfaces;
using tourism_firm.объекты;

namespace tourism_firm
{
    public class EmployeeAdapter : IEmployeeAdapter
    {
        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT employee_id, last_name, first_name, middle_name, phone, email FROM employees ORDER BY last_name",
                    connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        LastName = reader["last_name"].ToString(),
                        FirstName = reader["first_name"].ToString(),
                        MiddleName = reader["middle_name"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Email = reader["email"]?.ToString() ?? ""
                    });
                }
            }
            return employees;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT employee_id, last_name, first_name, middle_name, phone, email FROM employees WHERE employee_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", employeeId);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Employee
                    {
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        LastName = reader["last_name"].ToString(),
                        FirstName = reader["first_name"].ToString(),
                        MiddleName = reader["middle_name"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Email = reader["email"]?.ToString() ?? ""
                    };
                }
                return null;
            }
        }

        public bool AddEmployee(Employee employee)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "INSERT INTO employees (last_name, first_name, middle_name, phone, email) VALUES (?, ?, ?, ?, ?)",
                    connection);
                command.Parameters.AddWithValue("?", employee.LastName);
                command.Parameters.AddWithValue("?", employee.FirstName);
                command.Parameters.AddWithValue("?", employee.MiddleName ?? "");
                command.Parameters.AddWithValue("?", employee.Phone ?? "");
                command.Parameters.AddWithValue("?", employee.Email ?? "");
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            if (employee == null || employee.EmployeeId <= 0)
                return false;

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE employees SET last_name = ?, first_name = ?, middle_name = ?, phone = ?, email = ? WHERE employee_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", employee.LastName ?? "");
                command.Parameters.AddWithValue("?", employee.FirstName ?? "");
                command.Parameters.AddWithValue("?", employee.MiddleName ?? "");
                command.Parameters.AddWithValue("?", employee.Phone ?? "");
                command.Parameters.AddWithValue("?", employee.Email ?? "");
                command.Parameters.AddWithValue("?", employee.EmployeeId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteEmployee(int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var deleteOrders = new OleDbCommand("DELETE FROM orders WHERE employee_id = ?", connection);
                deleteOrders.Parameters.AddWithValue("?", employeeId);
                deleteOrders.ExecuteNonQuery();
                var deleteUser = new OleDbCommand("DELETE FROM users WHERE employee_id = ?", connection);
                deleteUser.Parameters.AddWithValue("?", employeeId);
                deleteUser.ExecuteNonQuery();
                var deleteEmployee = new OleDbCommand("DELETE FROM employees WHERE employee_id = ?", connection);
                deleteEmployee.Parameters.AddWithValue("?", employeeId);
                return deleteEmployee.ExecuteNonQuery() > 0;
            }
        }

        public int GetLastEmployeeId()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand("SELECT MAX(employee_id) FROM employees", connection);
                var result = command.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }
    }
}