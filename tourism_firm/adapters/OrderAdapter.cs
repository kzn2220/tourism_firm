using System;
using System.Collections.Generic;
using System.Data.OleDb;
using tourism_firm.Interfaces;
using tourism_firm.объекты;

namespace tourism_firm
{
    public class OrderAdapter : IOrderAdapter
    {
        public bool CreateOrder(int clientId, int tourId, int employeeId, int seatsAmount, decimal totalCost)
        {
            if (clientId <= 0 || tourId <= 0 || employeeId <= 0 || seatsAmount <= 0)
                return false;

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var checkSeatsCmd = new OleDbCommand("SELECT available_seats FROM tours WHERE tour_id = ? AND is_deleted = 0", connection, transaction))
                        {
                            checkSeatsCmd.Parameters.AddWithValue("?", tourId);
                            object result = checkSeatsCmd.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                transaction.Rollback();
                                return false;
                            }
                            int availableSeats = Convert.ToInt32(result);
                            if (availableSeats < seatsAmount)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        using (var orderCommand = new OleDbCommand(
                            "INSERT INTO orders (client_id, tour_id, employee_id, order_date, total_cost, seats_amount, is_deleted) VALUES (?, ?, ?, ?, ?, ?, 0)",
                            connection, transaction))
                        {
                            orderCommand.Parameters.AddWithValue("?", clientId);
                            orderCommand.Parameters.AddWithValue("?", tourId);
                            orderCommand.Parameters.AddWithValue("?", employeeId);
                            orderCommand.Parameters.AddWithValue("?", DateTime.Now.Date);
                            orderCommand.Parameters.AddWithValue("?", Convert.ToDouble(totalCost));
                            orderCommand.Parameters.AddWithValue("?", seatsAmount);
                            orderCommand.ExecuteNonQuery();
                        }

                        using (var updateSeats = new OleDbCommand(
                            "UPDATE tours SET available_seats = available_seats - ? WHERE tour_id = ? AND available_seats >= ?",
                            connection, transaction))
                        {
                            updateSeats.Parameters.AddWithValue("?", seatsAmount);
                            updateSeats.Parameters.AddWithValue("?", tourId);
                            updateSeats.Parameters.AddWithValue("?", seatsAmount);
                            int rows = updateSeats.ExecuteNonQuery();
                            if (rows == 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

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
        }

        public List<Order> GetClientOrders(int clientId, bool includeDeleted = true)
        {
            var orders = new List<Order>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT o.order_id, o.client_id, o.tour_id, o.employee_id, o.order_date, o.total_cost, o.seats_amount,
                           t.tour_name, t.departure_date, t.return_date, t.available_seats, t.price, t.country, t.city, t.hotel
                    FROM orders o
                    INNER JOIN tours t ON o.tour_id = t.tour_id
                    WHERE o.client_id = ?";
                if (!includeDeleted)
                    query += " AND o.is_deleted = 0";
                query += " ORDER BY o.order_date DESC";

                var command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("?", clientId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = Convert.ToInt32(reader["order_id"]),
                        ClientId = Convert.ToInt32(reader["client_id"]),
                        TourId = Convert.ToInt32(reader["tour_id"]),
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        OrderDate = Convert.ToDateTime(reader["order_date"]),
                        TotalCost = Convert.ToDecimal(reader["total_cost"]),
                        TourName = reader["tour_name"].ToString(),
                        TourStartDate = Convert.ToDateTime(reader["departure_date"]),
                        TourEndDate = Convert.ToDateTime(reader["return_date"]),
                        AvailableSeats = Convert.ToInt32(reader["available_seats"]),
                        Country = reader["country"]?.ToString() ?? "",
                        City = reader["city"]?.ToString() ?? "",
                        Hotel = reader["hotel"]?.ToString() ?? "",
                        SeatsAmount = reader["seats_amount"] != DBNull.Value ? Convert.ToInt32(reader["seats_amount"]) : 1
                    });
                }
            }
            return orders;
        }

        public List<Order> GetAllOrders(bool includeDeleted = false)
        {
            var orders = new List<Order>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT o.order_id, o.client_id, o.tour_id, o.employee_id, o.order_date, o.total_cost, o.seats_amount,
                           c.last_name & ' ' & c.first_name & ' ' & c.middle_name AS ClientFullName,
                           e.last_name & ' ' & e.first_name & ' ' & e.middle_name AS EmployeeFullName,
                           t.tour_name, t.departure_date, t.return_date
                    FROM ((orders o
                    INNER JOIN clients c ON o.client_id = c.client_id)
                    INNER JOIN employees e ON o.employee_id = e.employee_id)
                    INNER JOIN tours t ON o.tour_id = t.tour_id";
                if (!includeDeleted)
                    query += " WHERE o.is_deleted = 0";
                query += " ORDER BY o.order_date DESC";

                var command = new OleDbCommand(query, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = Convert.ToInt32(reader["order_id"]),
                        ClientId = Convert.ToInt32(reader["client_id"]),
                        TourId = Convert.ToInt32(reader["tour_id"]),
                        EmployeeId = Convert.ToInt32(reader["employee_id"]),
                        OrderDate = Convert.ToDateTime(reader["order_date"]),
                        TotalCost = Convert.ToDecimal(reader["total_cost"]),
                        ClientFullName = reader["ClientFullName"].ToString(),
                        EmployeeFullName = reader["EmployeeFullName"].ToString(),
                        TourName = reader["tour_name"].ToString(),
                        TourStartDate = Convert.ToDateTime(reader["departure_date"]),
                        TourEndDate = Convert.ToDateTime(reader["return_date"]),
                        SeatsAmount = reader["seats_amount"] != DBNull.Value ? Convert.ToInt32(reader["seats_amount"]) : 1
                    });
                }
            }
            return orders;
        }

        public bool DeleteOrder(int orderId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE orders SET is_deleted = 1 WHERE order_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", orderId);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}