using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Atvevo.db
{
    public class DatabaseConnection
    {
        public static readonly string WorkDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db");
        
        private const string DbName = "Atvevo.db";
        private readonly SQLiteConnection _connection;
        
        public SuppliersTable SuppliersTable;
        public ProductsTable ProductsTable;
        public SupplyArrivalsTable SupplyArrivalsTable;
        public SupplierProductConnectionTable SupplierProductConnectionTable;
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
            SuppliersTable = new SuppliersTable(this, withDummyData); 
            //ProductsTable = new ProductsTable(this);
            //SupplyArrivalsTable = new SupplyArrivalsTable(this);
            //SupplierProductConnectionTable = new SupplierProductConnectionTable(this);
        }
        private string DbConnection()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
            {
                DataSource = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db", DbName),
                Version = 3
            };
            return builder.ToString();
        }
        public void ExecuteWithoutReturn(string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                var affectedRows = command.ExecuteNonQuery();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query. Affected rows: " + affectedRows);
            }
        }
        public object ExecuteWithReturn(string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                object result = command.ExecuteScalar();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query.");
                return result;
            }
        }
        public void DatabaseDisconnect()
        {
            _connection.Close();
        }
    }
    public abstract class DatabaseTable
    {
        protected string _tableName;
        protected DatabaseConnection _connection;
        protected void WithDummyData(string dataFilePath)
        {
            if (TableEntriesCount() == 0)
            {
                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    var headers = sr.ReadLine()?.Split(',');
                    var data = sr.ReadLine();
                    while (data != null)
                    {
                        var splitData = data.Split(',');
                        Dictionary<string, string> columnsWithValues = headers?.Zip(splitData, (first, second) => new { first, second }).ToDictionary(x => x.first, x => x.second);
                    
                        string keysAsString = columnsWithValues?.Keys.Aggregate("", (current, key) => current + "," + key);
                        keysAsString = keysAsString?.TrimStart(',');
                        string valuesAsString = columnsWithValues?.Values.Aggregate("", (current, value) => current + ",'" + value + "'");
                        valuesAsString = valuesAsString?.TrimStart(',');
                    
                        _connection.ExecuteWithoutReturn($"INSERT INTO {_tableName} ({keysAsString}) VALUES({valuesAsString});");
                        data = sr.ReadLine();
                    }
                }
            }
        }
        protected int TableEntriesCount()
        {
            var result = _connection.ExecuteWithReturn($"SELECT COUNT(*) FROM {_tableName};");
            return (int)result;
        }
    }
    public class SuppliersTable : DatabaseTable
    { 
        private const string TableName = "suppliers";
        public SuppliersTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            _tableName = TableName;
            string createSuppliersTable = 
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone INTEGER NOT NULL,supplier_code TEXT NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, "besz.csv"));
            }
        }
    }
    public class ProductsTable : DatabaseTable
    {
        private const string TableName = "products";
        public ProductsTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            _tableName = TableName;
            string createSuppliersTable = 
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, category TEXT NOT NULL, price REAL NOT NULL);"; 
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, ""));
            }
        }
    }
    public class SupplyArrivalsTable : DatabaseTable
    {
        private const string TableName = "supply_arrivals";
        public SupplyArrivalsTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            string createSuppliersTable = 
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL, arrival_time NUMERIC NOT NULL, quantity INTEGER NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, ""));
            }
        }
    }
    public class SupplierProductConnectionTable : DatabaseTable
    {
        private const string TableName = "supplier_product_connections";
        public SupplierProductConnectionTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            string createSuppliersTable =
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL, arrival_time NUMERIC NOT NULL, quantity INTEGER NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, ""));
            }
        }
    }
}