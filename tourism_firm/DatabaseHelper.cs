using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace tourism_firm
{
    public class DatabaseHelper
    {
        private static string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=tourism_firm.accdb;";

        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
