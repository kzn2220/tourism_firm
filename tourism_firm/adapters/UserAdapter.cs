using System;
using System.Data.OleDb;
using tourism_firm.Interfaces;
using tourism_firm.объекты;

namespace tourism_firm
{
    public class UserAdapter : IUserAdapter
    {
        public User Authenticate(string login, string password)
        {
            string hashedPassword = PasswordHasher.HashPassword(password);
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT user_id, login, role, client_id, employee_id FROM users WHERE login = @login AND password_hash = @password",
                    connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", hashedPassword);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        Login = reader["login"].ToString(),
                        Role = reader["role"].ToString(),
                        ClientId = reader["client_id"] != DBNull.Value ? Convert.ToInt32(reader["client_id"]) : (int?)null,
                        EmployeeId = reader["employee_id"] != DBNull.Value ? Convert.ToInt32(reader["employee_id"]) : (int?)null
                    };
                }
                return null;
            }
        }

        public bool RegisterClient(string login, string password, Client client)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var clientCommand = new OleDbCommand(
                        "INSERT INTO clients (last_name, first_name, middle_name, phone, email) VALUES (@lastName, @firstName, @middleName, @phone, @email)",
                        connection, transaction);
                    clientCommand.Parameters.AddWithValue("@lastName", client.LastName);
                    clientCommand.Parameters.AddWithValue("@firstName", client.FirstName);
                    clientCommand.Parameters.AddWithValue("@middleName", client.MiddleName ?? "");
                    clientCommand.Parameters.AddWithValue("@phone", client.Phone ?? "");
                    clientCommand.Parameters.AddWithValue("@email", client.Email ?? "");
                    clientCommand.ExecuteNonQuery();
                    var getIdCommand = new OleDbCommand("SELECT @@IDENTITY", connection, transaction);
                    int clientId = Convert.ToInt32(getIdCommand.ExecuteScalar());
                    string hashedPassword = PasswordHasher.HashPassword(password);
                    var userCommand = new OleDbCommand(
                        "INSERT INTO users (login, password_hash, role, client_id, employee_id) VALUES (@login, @password, 'Клиент', @clientId, NULL)",
                        connection, transaction);
                    userCommand.Parameters.AddWithValue("@login", login);
                    userCommand.Parameters.AddWithValue("@password", hashedPassword);
                    userCommand.Parameters.AddWithValue("@clientId", clientId);
                    userCommand.ExecuteNonQuery();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool RegisterEmployee(string login, string password, int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string hashedPassword = PasswordHasher.HashPassword(password);
                var command = new OleDbCommand(
                    "INSERT INTO users (login, password_hash, role, employee_id, client_id) VALUES (@login, @password, 'Сотрудник', @employeeId, NULL)",
                    connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.Parameters.AddWithValue("@employeeId", employeeId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool RegisterAdmin(string login, string password, int employeeId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string hashedPassword = PasswordHasher.HashPassword(password);
                var command = new OleDbCommand(
                    "INSERT INTO users (login, password_hash, role, employee_id, client_id) VALUES (@login, @password, 'Администратор', @employeeId, NULL)",
                    connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.Parameters.AddWithValue("@employeeId", employeeId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateClientData(int clientId, string lastName, string firstName, string middleName, string phone, string email)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE clients SET last_name = @lastName, first_name = @firstName, middle_name = @middleName, phone = @phone, email = @email WHERE client_id = @id",
                    connection);
                command.Parameters.AddWithValue("@id", clientId);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@middleName", middleName ?? "");
                command.Parameters.AddWithValue("@phone", phone ?? "");
                command.Parameters.AddWithValue("@email", email ?? "");
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string hashedPassword = PasswordHasher.HashPassword(newPassword);
                var command = new OleDbCommand(
                    "UPDATE users SET password_hash = @password WHERE user_id = @id",
                    connection);
                command.Parameters.AddWithValue("@id", userId);
                command.Parameters.AddWithValue("@password", hashedPassword);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateEmployeeRole(int employeeId, string role)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE users SET role = ? WHERE employee_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", role);
                command.Parameters.AddWithValue("?", employeeId);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}