using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Atvevo.db
{
    public class DatabaseConnection
    {
        public static string WorkDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db");
        private const string DbName = "Atvevo.db";
        private readonly SQLiteConnection _connection;
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
        }
        private string DbConnection()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db", DbName);
            builder.Version = 3;

            return builder.ToString();
        }
        public void Execute(string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                var affectedRows = command.ExecuteNonQuery();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query. Affected rows: " + affectedRows);
            }
        }
        private void CreateTables()
        {
            string createProductsTable = 
                "CREATE TABLE IF NOT EXISTS products(id INTEGER PRIMARY KEY, name TEXT NOT NULL, category TEXT NOT NULL, price REAL NOT NULL);"; 
            Execute(createProductsTable);
            string createProductSupplierConnectionsTable =
                "CREATE TABLE IF NOT EXISTS supplier_product_connection(id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL);";
            Execute(createProductSupplierConnectionsTable);
            string createSupplyArrivailsTable =
                "CREATE TABLE IF NOT EXISTS supply_arrivals(id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL, arrival_time NUMERIC NOT NULL, quantity INTEGER NOT NULL);";
            Execute(createSupplyArrivailsTable);
        }
        public void DatabaseDisconnect()
        {
            _connection.Close();
        }
        public void Create(string table, Dictionary<string, string> columns)
        {
            string keysAsString = columns.Keys.Aggregate("", (current, key) => current + "," + key);
            keysAsString = keysAsString.TrimStart(',');
            string valuesAsString = columns.Values.Aggregate("", (current, value) => current + ",'" + value + "'");
            valuesAsString = valuesAsString.TrimStart(',');
            string command = $"INSERT INTO {table} ({keysAsString}) VALUES({valuesAsString});";
            try
            {
                Execute(command);
            }
            catch (Exception e)
            {
                _connection.LogMessage(SQLiteErrorCode.Error, $"Failed to insert into table {table}");
                throw;
            }
            //TODO add a functionality so it can handle dummy data with header line not in order
        }
    }
    public class SuppliersTable
    {
        private readonly string _tableName = "suppliers";
        private readonly DatabaseConnection _connection;
        public SuppliersTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            string createSuppliersTable = 
                $"CREATE TABLE IF NOT EXISTS {_tableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone INTEGER NOT NULL,supplier_code TEXT NOT NULL);";
            _connection.Execute(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, "besz.csv"));
            }
        }
        private void WithDummyData(string dataFilePath)
        {
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                var headers = sr.ReadLine()?.Split(',');
                var data = sr.ReadLine();
                while (data != null)
                {
                    var splitedData = data.Split(',');
                    Dictionary<string, string> columnsWithValues = headers?.Zip(splitedData, (first, second) => new { first, second }).ToDictionary(x => x.first, x => x.second);
                    
                    string keysAsString = columnsWithValues?.Keys.Aggregate("", (current, key) => current + "," + key);
                    keysAsString = keysAsString?.TrimStart(',');
                    string valuesAsString = columnsWithValues?.Values.Aggregate("", (current, value) => current + ",'" + value + "'");
                    valuesAsString = valuesAsString?.TrimStart(',');
                    
                    _connection.Execute($"INSERT INTO {_tableName} ({keysAsString}) VALUES({valuesAsString});");
                    data = sr.ReadLine();
                }
            }
        }
    }
    public class ProductsTable
    {
        private readonly string _tableName = "products";
        private readonly DatabaseConnection _connection;
        public ProductsTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            string createSuppliersTable = 
                $"CREATE TABLE IF NOT EXISTS {_tableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone INTEGER NOT NULL,supplier_code TEXT NOT NULL);";
            _connection.Execute(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, ""));
            }
        }
        private void WithDummyData(string dataFilePath)
        {
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                var headers = sr.ReadLine()?.Split(',');
                var data = sr.ReadLine();
                while (data != null)
                {
                    var splitedData = data.Split(',');
                    Dictionary<string, string> columnsWithValues = headers?.Zip(splitedData, (first, second) => new { first, second }).ToDictionary(x => x.first, x => x.second);
                    
                    string keysAsString = columnsWithValues?.Keys.Aggregate("", (current, key) => current + "," + key);
                    keysAsString = keysAsString?.TrimStart(',');
                    string valuesAsString = columnsWithValues?.Values.Aggregate("", (current, value) => current + ",'" + value + "'");
                    valuesAsString = valuesAsString?.TrimStart(',');
                    
                    _connection.Execute($"INSERT INTO {_tableName} ({keysAsString}) VALUES({valuesAsString});");
                    data = sr.ReadLine();
                }
            }
        }
    }
    public class SupplyArrivalsTable
    {
        private readonly string _tableName = "products";
        private readonly DatabaseConnection _connection;
        public SupplyArrivalsTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            string createSuppliersTable = 
                $"CREATE TABLE IF NOT EXISTS {_tableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone INTEGER NOT NULL,supplier_code TEXT NOT NULL);";
            _connection.Execute(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, ""));
            }
        }
        private void WithDummyData(string dataFilePath)
        {
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                var headers = sr.ReadLine()?.Split(',');
                var data = sr.ReadLine();
                while (data != null)
                {
                    var splitedData = data.Split(',');
                    Dictionary<string, string> columnsWithValues = headers?.Zip(splitedData, (first, second) => new { first, second }).ToDictionary(x => x.first, x => x.second);
                    
                    string keysAsString = columnsWithValues?.Keys.Aggregate("", (current, key) => current + "," + key);
                    keysAsString = keysAsString?.TrimStart(',');
                    string valuesAsString = columnsWithValues?.Values.Aggregate("", (current, value) => current + ",'" + value + "'");
                    valuesAsString = valuesAsString?.TrimStart(',');
                    
                    _connection.Execute($"INSERT INTO {_tableName} ({keysAsString}) VALUES({valuesAsString});");
                    data = sr.ReadLine();
                }
            }
        }
    }
}