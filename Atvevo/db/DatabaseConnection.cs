using System;
using System.IO;
using System.Data.SQLite;
namespace Atvevo.db
{
    public class DatabaseConnection
    {
        private const string _DB_NAME = "atvevo.db";
        private readonly string _connectionString = $"DataSource={_DB_NAME};Version=3;";
        private SQLiteConnection _connection;
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
            
            //create table if not exist
        }

        private void Execute(SQLiteConnection connection, string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                command.ExecuteNonQuery();
            }
        }
        private void CreateTables()
        {
            string createSuppliersTable = "CREATE TABLE IF NOT EXISTS suppliers(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone INTEGER NOT NULL,supplier_code TEXT NOT NULL)";
            Execute(_connection, createSuppliersTable);
            string createProductsTable = "";  //TODO
        }
        public void DatabaseDisconnect()
        {
            _connection.Close();
        }
    } 
}