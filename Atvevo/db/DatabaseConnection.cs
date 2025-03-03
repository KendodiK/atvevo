using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Atvevo.db;

namespace Atvevo.db
{
    public class DatabaseConnection
    {
        public static readonly string WorkDir =
            Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db");

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
            ProductsTable = new ProductsTable(this, withDummyData);
            SupplyArrivalsTable = new SupplyArrivalsTable(this);
            SupplierProductConnectionTable = new SupplierProductConnectionTable(this);
        }

        private string DbConnection()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
            {
                DataSource =
                    Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db", DbName),
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

        public object ExecuteWithSingleReturn(string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                object result = command.ExecuteScalar();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query.");
                return result;
            }
        }

        public SQLiteDataReader ExecuteWithMultipleReturn(string query)
        {
            using (var command = new SQLiteCommand(query, _connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                return reader;
            }
        }

        public void DatabaseDisconnect()
        {
            _connection.Close();
        }
    }

    public abstract class DatabaseTable<TClass> where TClass : class
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
                        Dictionary<string, string> columnsWithValues = headers
                            ?.Zip(splitData, (first, second) => new { first, second })
                            .ToDictionary(x => x.first, x => x.second);

                        string keysAsString =
                            columnsWithValues?.Keys.Aggregate("", (current, key) => current + "," + key);
                        keysAsString = keysAsString?.TrimStart(',');
                        string valuesAsString =
                            columnsWithValues?.Values.Aggregate("", (current, value) => current + ",'" + value + "'");
                        valuesAsString = valuesAsString?.TrimStart(',');

                        _connection.ExecuteWithoutReturn(
                            $"INSERT INTO {_tableName} ({keysAsString}) VALUES({valuesAsString});");
                        data = sr.ReadLine();
                    }
                }
            }
        }

        protected int TableEntriesCount()
        {
            var result = _connection.ExecuteWithSingleReturn($"SELECT COUNT(*) FROM {_tableName};");
            return Convert.ToInt32(result);
        }
        public abstract TClass[] Read();
    }

    public class SuppliersTable : DatabaseTable<Supplier>
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

        public override Supplier[] Read()
        {
            if (TableEntriesCount() == 0)
            {
                return Array.Empty<Supplier>();
            }
            List<Supplier> result = new List<Supplier>();
            var queryResult = _connection.ExecuteWithMultipleReturn($"SELECT * FROM {TableName}");
            while (queryResult.Read())
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < typeof(Supplier).GetProperties().Length; i++)
                {
                    values.Add(queryResult.GetName(i), queryResult.GetValue(i).ToString());
                }
                result.Add(new Supplier
                {
                    Id = Convert.ToInt32(values["id"]),
                    Name = values["name"],
                    City = values["city"],
                    County = values["county"],
                    Street = values["street"],
                    HouseNumber = Convert.ToByte(values["house_number"]),
                    ZipCode = values["post_code"],
                    Phone = Convert.ToInt32(values["phone"]),
                    Code = values["supplier_code"],
                });
            }
            return result.ToArray();
        }
    }
    public class ProductsTable : DatabaseTable<Product>
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
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, "gyumolcs.csv"));
            }
        }
        public override Product[] Read()
        {
            if (TableEntriesCount() == 0)
            {
                return Array.Empty<Product>();
            }
            List<Product> result = new List<Product>();
            var queryResult = _connection.ExecuteWithMultipleReturn($"SELECT * FROM {TableName}");
            while (queryResult.Read())
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < typeof(Supplier).GetProperties().Length; i++)
                {
                    values.Add(queryResult.GetName(i), queryResult.GetValue(i).ToString());
                }
                result.Add(new Product()
                {
                    Id = Convert.ToInt32(values["id"]),
                    Name = values["name"],
                    Category = values["category"],
                    Price = Convert.ToDouble(values["price"]),
                });
            }
            return result.ToArray();
        }
    }

    public class SupplyArrivalsTable : DatabaseTable<SupplyArrival>
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

        public override SupplyArrival[] Read()
        {
            return Array.Empty<SupplyArrival>();
        }
    }

    public class SupplierProductConnectionTable : DatabaseTable<SupplierProductConnection>
    {
        private const string TableName = "supplier_product_connections";

        public SupplierProductConnectionTable(DatabaseConnection connection, bool withDummyData = false)
        {
            _connection = connection;
            string createSuppliersTable =
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData)
            {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, ""));
            }
        }

        public override SupplierProductConnection[] Read()
        {
            return Array.Empty<SupplierProductConnection>();
        }
    }
}