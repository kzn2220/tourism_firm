using System;
using System.Data.OleDb;
using System.Text;
using tourism_firm.Interfaces;

namespace tourism_firm
{
    public class ReportAdapter : IReportAdapter
    {
        public string GenerateSalesReport(DateTime fromDate, DateTime toDate)
        {
            var sb = new StringBuilder();
            sb.AppendLine("╔════════════════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                              ОТЧЁТ ПО ПРОДАЖАМ                                          ");
            sb.AppendLine($"   Период: {fromDate:dd.MM.yyyy} - {toDate:dd.MM.yyyy}                                   ");
            sb.AppendLine($"   Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}                                 ");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string sql = @"
                    SELECT t.tour_name, o.order_date, o.total_cost
                    FROM orders o
                    INNER JOIN tours t ON o.tour_id = t.tour_id
                    WHERE o.order_date >= ? AND o.order_date <= ?
                    ORDER BY o.order_date";
                using (var command = new OleDbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("?", fromDate.Date);
                    command.Parameters.AddWithValue("?", toDate.Date.AddDays(1).AddSeconds(-1));
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            sb.AppendLine("  Продаж за указанный период не найдено.");
                        }
                        else
                        {
                            sb.AppendLine("  Название тура".PadRight(40) + "Дата".PadRight(12) + "Сумма");
                            sb.AppendLine("  " + new string('-', 70));
                            decimal total = 0;
                            int count = 0;
                            while (reader.Read())
                            {
                                string name = reader["tour_name"].ToString();
                                if (name.Length > 36) name = name.Substring(0, 36);
                                string date = Convert.ToDateTime(reader["order_date"]).ToString("dd.MM.yyyy");
                                decimal cost = Convert.ToDecimal(reader["total_cost"]);
                                sb.AppendLine($"  {name,-36} {date,-12} {cost,12:N2} руб.");
                                total += cost;
                                count++;
                            }
                            sb.AppendLine("  " + new string('-', 70));
                            sb.AppendLine($"  Итого заказов: {count}");
                            sb.AppendLine($"  Общая выручка: {total:N2} руб.");
                        }
                    }
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public string GenerateEmployeeStatsReport(DateTime fromDate, DateTime toDate)
        {
            var sb = new StringBuilder();
            sb.AppendLine("╔════════════════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                          СТАТИСТИКА ПО СОТРУДНИКАМ                                      ");
            sb.AppendLine($"   Период: {fromDate:dd.MM.yyyy} - {toDate:dd.MM.yyyy}                                   ");
            sb.AppendLine($"   Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}                                 ");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                DateTime startDate = fromDate.Date;
                DateTime endDate = toDate.Date.AddDays(1).AddSeconds(-1);

                string checkSql = "SELECT COUNT(*) FROM orders WHERE order_date >= ? AND order_date <= ?";
                using (var checkCmd = new OleDbCommand(checkSql, connection))
                {
                    checkCmd.Parameters.AddWithValue("?", startDate);
                    checkCmd.Parameters.AddWithValue("?", endDate);
                    int orderCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (orderCount == 0)
                    {
                        sb.AppendLine("  За указанный период нет заказов.");
                        sb.AppendLine();
                        return sb.ToString();
                    }
                }

                string sql = @"
            SELECT e.last_name, e.first_name,
                   COUNT(o.order_id) AS OrderCount,
                   SUM(o.total_cost) AS TotalSales
            FROM orders o
            INNER JOIN employees e ON o.employee_id = e.employee_id
            WHERE o.order_date >= ? AND o.order_date <= ?
            GROUP BY e.employee_id, e.last_name, e.first_name
            ORDER BY 4 DESC";

                using (var command = new OleDbCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("?", startDate);
                    command.Parameters.AddWithValue("?", endDate);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            sb.AppendLine("  Данных по сотрудникам за указанный период не найдено.");
                        }
                        else
                        {
                            sb.AppendLine("  Сотрудник".PadRight(40) + "Путёвок".PadRight(12) + "Сумма");
                            sb.AppendLine("  " + new string('-', 65));
                            while (reader.Read())
                            {
                                string lastName = reader["last_name"].ToString();
                                string firstName = reader["first_name"].ToString();
                                string fullName = $"{lastName} {firstName}".Trim();
                                if (fullName.Length > 36) fullName = fullName.Substring(0, 36);
                                int count = Convert.ToInt32(reader["OrderCount"]);
                                decimal sum = Convert.ToDecimal(reader["TotalSales"]);
                                sb.AppendLine($"  {fullName,-36} {count,8}    {sum,12:N2} руб.");
                            }
                        }
                    }
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}