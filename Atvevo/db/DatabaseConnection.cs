using System;
using System.Data.SQLite;

namespace Atvevo.db
{
    public class DatabaseConnection
    {
        private const string DB_NAME = "atvevo.db";
        private readonly string _connectionString = $"DataSource={DB_NAME};Version=3;";
        private readonly SQLiteConnection _connection;
        public DatabaseConnection()
        {
            try
            {
                _connection = new SQLiteConnection(_connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            _connection.Open();
            CreateTables();
        }

        private void Execute(SQLiteConnection connection, string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                var affectedRows = command.ExecuteNonQuery();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query.");
            }
        }
        private void CreateTables()
        {
            string createSuppliersTable = 
                "CREATE TABLE IF NOT EXISTS suppliers(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone INTEGER NOT NULL,supplier_code TEXT NOT NULL)";
            Execute(_connection, createSuppliersTable);
            string createProductsTable = 
                "CREATE TABLE IF NOT EXISTS products(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, category TEXT NOT NULL, price REAL NOT NULL)"; 
            Execute(_connection, createProductsTable);
            string createProductSupplierConnectionsTable =
                "CREATE TABLE IF NOT EXISTS supplier_product_connection(id INTEGER PRIMARY KEY AUTOINCREMENT, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL)";
            Execute(_connection, createProductSupplierConnectionsTable);
            string createSupplyArrivailsTable =
                "CREATE TABLE IF NOT EXISTS supply_arrivals(id INTEGER PRIMARY KEY AUTOINCREMENT, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL, arrival_time NUMERIC NOT NULL, quantity INTEGER NOT NULL)";
            Execute(_connection, createSupplyArrivailsTable);
        }
        public void DatabaseDisconnect()
        {
            _connection.Close();
        }
    } 
}