using System;
using System.Collections.Generic;
using System.Data.OleDb;
using tourism_firm.Interfaces;
using tourism_firm.объекты;

namespace tourism_firm
{
    public class TourAdapter : ITourAdapter
    {
        public List<Tour> GetAllTours()
        {
            var tours = new List<Tour>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT tour_id, tour_name, country, city, hotel, price, available_seats, departure_date, return_date FROM tours WHERE available_seats > 0 AND is_deleted = 0 ORDER BY departure_date",
                    connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tours.Add(MapReaderToTour(reader, false));
                }
            }
            return tours;
        }

        public List<Tour> GetAllToursForAdmin()
        {
            var tours = new List<Tour>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT tour_id, tour_name, country, city, hotel, price, available_seats, departure_date, return_date, is_deleted FROM tours ORDER BY departure_date",
                    connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tours.Add(MapReaderToTour(reader, true));
                }
            }
            return tours;
        }

        public List<Tour> SearchTours(string country, decimal? maxPrice, int? minSeats)
        {
            var tours = new List<Tour>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT tour_id, tour_name, country, city, hotel, price, available_seats, departure_date, return_date FROM tours WHERE available_seats > 0 AND is_deleted = 0";
                var command = new OleDbCommand();
                command.Connection = connection;
                if (!string.IsNullOrEmpty(country))
                {
                    query += " AND LCASE(country) LIKE LCASE(@country)";
                    command.Parameters.AddWithValue("@country", "%" + country + "%");
                }
                if (maxPrice.HasValue && maxPrice.Value > 0)
                {
                    query += " AND price <= @maxPrice";
                    command.Parameters.AddWithValue("@maxPrice", maxPrice.Value);
                }
                if (minSeats.HasValue && minSeats.Value > 0)
                {
                    query += " AND available_seats >= @minSeats";
                    command.Parameters.AddWithValue("@minSeats", minSeats.Value);
                }
                query += " ORDER BY departure_date";
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tours.Add(MapReaderToTour(reader, false));
                }
            }
            return tours;
        }

        public Tour GetTourById(int tourId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "SELECT tour_id, tour_name, country, city, hotel, price, available_seats, departure_date, return_date FROM tours WHERE tour_id = @id",
                    connection);
                command.Parameters.AddWithValue("@id", tourId);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapReaderToTour(reader, false);
                }
                return null;
            }
        }

        public bool AddTour(Tour tour)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    @"INSERT INTO tours (tour_name, country, city, hotel, price, available_seats, departure_date, return_date, is_deleted) 
                      VALUES (@name, @country, @city, @hotel, @price, @seats, @departure, @return, 0)",
                    connection);
                command.Parameters.AddWithValue("@name", tour.TourName);
                command.Parameters.AddWithValue("@country", tour.Country);
                command.Parameters.AddWithValue("@city", tour.City ?? "");
                command.Parameters.AddWithValue("@hotel", tour.Hotel ?? "");
                command.Parameters.AddWithValue("@price", tour.Price);
                command.Parameters.AddWithValue("@seats", tour.AvailableSeats);
                command.Parameters.AddWithValue("@departure", tour.DepartureDate);
                command.Parameters.AddWithValue("@return", tour.ReturnDate);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteTour(int tourId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE tours SET is_deleted = 1 WHERE tour_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", tourId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateTour(Tour tour)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE tours SET tour_name = ?, country = ?, city = ?, hotel = ?, price = ?, available_seats = ?, departure_date = ?, return_date = ? WHERE tour_id = ?",
                    connection);
                command.Parameters.AddWithValue("?", tour.TourName);
                command.Parameters.AddWithValue("?", tour.Country);
                command.Parameters.AddWithValue("?", tour.City ?? "");
                command.Parameters.AddWithValue("?", tour.Hotel ?? "");
                command.Parameters.AddWithValue("?", tour.Price);
                command.Parameters.AddWithValue("?", tour.AvailableSeats);
                command.Parameters.AddWithValue("?", tour.DepartureDate);
                command.Parameters.AddWithValue("?", tour.ReturnDate);
                command.Parameters.AddWithValue("?", tour.TourId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool DecreaseSeats(int tourId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE tours SET available_seats = available_seats - 1 WHERE tour_id = @id AND available_seats > 0",
                    connection);
                command.Parameters.AddWithValue("@id", tourId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool IncreaseSeats(int tourId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE tours SET available_seats = available_seats + 1 WHERE tour_id = @id",
                    connection);
                command.Parameters.AddWithValue("@id", tourId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateTourSeats(int tourId, int newSeats)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = new OleDbCommand(
                    "UPDATE tours SET available_seats = @seats WHERE tour_id = @id",
                    connection);
                command.Parameters.AddWithValue("@seats", newSeats);
                command.Parameters.AddWithValue("@id", tourId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        private Tour MapReaderToTour(OleDbDataReader reader, bool includeIsDeleted)
        {
            Tour tour = new Tour
            {
                TourId = Convert.ToInt32(reader["tour_id"]),
                TourName = reader["tour_name"].ToString(),
                Country = reader["country"].ToString(),
                City = reader["city"].ToString(),
                Hotel = reader["hotel"].ToString(),
                Price = Convert.ToDecimal(reader["price"]),
                AvailableSeats = Convert.ToInt32(reader["available_seats"]),
                DepartureDate = Convert.ToDateTime(reader["departure_date"]),
                ReturnDate = Convert.ToDateTime(reader["return_date"])
            };
            if (includeIsDeleted && reader["is_deleted"] != DBNull.Value)
            {
                tour.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
            }
            return tour;
        }
    }
}