using System;
using System.Collections.Generic;
using System.Data.OleDb;
using tourism_firm.Interfaces;
using tourism_firm.объекты;

namespace tourism_firm
{
    public class ClientAdapter : IClientAdapter
    {
        public List<Client> GetAllClients()
        {
            var clients = new List<Client>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    @"SELECT c.client_id, c.last_name, c.first_name, c.middle_name, c.phone, c.email, u.login
                      FROM clients c
                      LEFT JOIN users u ON c.client_id = u.client_id
                      ORDER BY c.last_name",
                    connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(new Client
                    {
                        ClientId = Convert.ToInt32(reader["client_id"]),
                        LastName = reader["last_name"].ToString(),
                        FirstName = reader["first_name"].ToString(),
                        MiddleName = reader["middle_name"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Email = reader["email"].ToString(),
                        Login = reader["login"] != DBNull.Value ? reader["login"].ToString() : ""
                    });
                }
            }
            return clients;
        }

        public Client GetClientById(int clientId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    @"SELECT c.client_id, c.last_name, c.first_name, c.middle_name, c.phone, c.email, u.login
                      FROM clients c
                      LEFT JOIN users u ON c.client_id = u.client_id
                      WHERE c.client_id = @id",
                    connection);
                command.Parameters.AddWithValue("@id", clientId);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Client
                    {
                        ClientId = Convert.ToInt32(reader["client_id"]),
                        LastName = reader["last_name"].ToString(),
                        FirstName = reader["first_name"].ToString(),
                        MiddleName = reader["middle_name"].ToString(),
                        Phone = reader["phone"].ToString(),
                        Email = reader["email"].ToString(),
                        Login = reader["login"] != DBNull.Value ? reader["login"].ToString() : ""
                    };
                }
                return null;
            }
        }

        public bool AddClient(Client client)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "INSERT INTO clients (last_name, first_name, middle_name, phone, email) VALUES (@lastName, @firstName, @middleName, @phone, @email)",
                    connection);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@middleName", client.MiddleName ?? "");
                command.Parameters.AddWithValue("@phone", client.Phone ?? "");
                command.Parameters.AddWithValue("@email", client.Email ?? "");
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateClient(Client client)
        {
            if (client == null || client.ClientId <= 0)
                return false;

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE clients SET last_name = ?, first_name = ?, middle_name = ?, phone = ?, email = ? WHERE client_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", client.LastName ?? "");
                command.Parameters.AddWithValue("?", client.FirstName ?? "");
                command.Parameters.AddWithValue("?", client.MiddleName ?? "");
                command.Parameters.AddWithValue("?", client.Phone ?? "");
                command.Parameters.AddWithValue("?", client.Email ?? "");
                command.Parameters.AddWithValue("?", client.ClientId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteClient(int clientId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var deleteOrders = new OleDbCommand("DELETE FROM orders WHERE client_id = @id", connection);
                deleteOrders.Parameters.AddWithValue("@id", clientId);
                deleteOrders.ExecuteNonQuery();
                var deleteUser = new OleDbCommand("DELETE FROM users WHERE client_id = @id", connection);
                deleteUser.Parameters.AddWithValue("@id", clientId);
                deleteUser.ExecuteNonQuery();
                var deleteClient = new OleDbCommand("DELETE FROM clients WHERE client_id = @id", connection);
                deleteClient.Parameters.AddWithValue("@id", clientId);
                return deleteClient.ExecuteNonQuery() > 0;
            }
        }

        public int GetClientIdByFullName(string fullName)
        {
            var parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return 0;
            string lastName = parts[0];
            string firstName = parts[1];
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT client_id FROM clients WHERE LCASE(last_name) = LCASE(@lastName) AND LCASE(first_name) = LCASE(@firstName)",
                    connection);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@firstName", firstName);
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }
}