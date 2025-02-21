using System;
using System.Data.SQLite;
using System.IO;

namespace Atvevo.db
{
    public abstract class Database
    {
        private string _databaseFilename { get; }
        public virtual void Create(){}
        public virtual void Read(){}
        public virtual void Update(){}
        public virtual void Delete(){}
    }
    public class DatabaseConnection : Database
    {
        private string _databaseFilename = "atvevo.db";
        private SQLiteConnection _connection;
        public DatabaseConnection(bool withDummyData = false)
        {
            string connectionString = DbConnection();
            try
            {
                _connection = new SQLiteConnection(connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            _connection.Open();
            CreateTables();
            if (withDummyData)
            {
                InsertDummyData("besz.csv");
            }
        }
        private string DbConnection()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db", _databaseFilename);
            builder.Version = 3;

            return builder.ToString();
        }
        private void Execute(SQLiteConnection connection, string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                var affectedRows = command.ExecuteNonQuery();
                connection.LogMessage(SQLiteErrorCode.Ok, "Successful query. Affected rows: " + affectedRows);
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
        private void InsertDummyData(string csvFileName)
        {
            //TODO read csv files
        }
        public override void Create()
        {
            base.Create();
            //TODO
        }
        public override void Read()
        {
            base.Read();
            //TODO
        }
        public override void Update()
        {
            base.Update();
            //TODO
        }
        public override void Delete()
        {
            base.Delete();
            //TODO
        }
    } 
}