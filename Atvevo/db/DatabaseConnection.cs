using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Atvevo.db {
    public static class DateTimeHelper {
        public static long ToUnixTimestamp(this DateTime dateTime) {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            TimeSpan timeSpan = dateTime - unixEpoch;
            return (long)timeSpan.TotalSeconds;
        }
        public static DateTime FromUnixTimestamp(this long unixTimestamp) {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return unixEpoch.AddSeconds(unixTimestamp);
        }
    }
    public class DatabaseConnection {
        public static readonly string WorkDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db");

        private const string DbName = "Atvevo.db";
        private readonly SQLiteConnection _connection;

        public readonly SuppliersTable SuppliersTable;
        public readonly ProductsTable ProductsTable;
        public readonly SupplyArrivalsTable SupplyArrivalsTable;
        public readonly SupplierProductConnectionTable SupplierProductConnectionTable;
        public DatabaseConnection(bool withDummyData = false) {
            string connectionString = DbConnection();
            try {
                _connection = new SQLiteConnection(connectionString);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }

            _connection.Open();
            SuppliersTable = new SuppliersTable(this, withDummyData);
            ProductsTable = new ProductsTable(this, withDummyData);
            SupplyArrivalsTable = new SupplyArrivalsTable(this);
            SupplierProductConnectionTable = new SupplierProductConnectionTable(this, withDummyData);
        }
        private string DbConnection() {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder {
                DataSource =
                    Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db", DbName),
                Version = 3
            };
            return builder.ToString();
        }
        public void ExecuteWithoutReturn(string query) {
            using (var command = new SQLiteCommand(query, _connection)) {
                var affectedRows = command.ExecuteNonQuery();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query. Affected rows: " + affectedRows);
            }
        }
        public object ExecuteWithSingleReturn(string query) {
            using (var command = new SQLiteCommand(query, _connection)) {
                object result = command.ExecuteScalar();
                _connection.LogMessage(SQLiteErrorCode.Ok, "Successful query.");
                return result;
            }
        }
        public SQLiteDataReader ExecuteWithMultipleReturn(string query) {
            using (var command = new SQLiteCommand(query, _connection)) {
                SQLiteDataReader reader = command.ExecuteReader();
                return reader;
            }
        }
        public void DatabaseDisconnect() {
            _connection.Dispose();
        }
    }
    public abstract class DatabaseTable<TClass> where TClass : class {
        protected string _tableName;
        protected DatabaseConnection _connection;
        protected void WithDummyData(string dataFilePath) {
            if (TableEntriesCount() == 0) {
                using (StreamReader sr = new StreamReader(dataFilePath)) {
                    var headers = sr.ReadLine()?.Split(',');
                    var data = sr.ReadLine();
                    while (data != null) {
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
        protected int TableEntriesCount() {
            var result = _connection.ExecuteWithSingleReturn($"SELECT COUNT(*) FROM {_tableName};");
            return Convert.ToInt32(result);
        }
        public abstract TClass[] Read();
        public abstract bool Insert(TClass model);
        public abstract bool Update(TClass model);
        public abstract bool Delete(TClass model);
    }
    public class SuppliersTable : DatabaseTable<Supplier> {
        public const string TableName = "suppliers";
        public SuppliersTable(DatabaseConnection connection, bool withDummyData = false) {
            _connection = connection;
            _tableName = TableName;
            string createSuppliersTable =
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, post_code INTEGER NOT NULL, county TEXT NOT NULL, city TEXT NOT NULL, street TEXT NOT NULL, house_number INTEGER NOT NULL, phone TEXT NOT NULL,supplier_code TEXT NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData) {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, "besz.csv"));
            }
        }
        public override Supplier[] Read() {
            if (TableEntriesCount() == 0) {
                return Array.Empty<Supplier>();
            }
            List<Supplier> result = new List<Supplier>();
            var queryResult = _connection.ExecuteWithMultipleReturn($"SELECT * FROM {TableName};");
            while (queryResult.Read()) {
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < typeof(Supplier).GetProperties().Length; i++) {
                    values.Add(queryResult.GetName(i), queryResult.GetValue(i).ToString());
                }
                result.Add(new Supplier {
                    Id = Convert.ToInt32(values["id"]),
                    Name = values["name"],
                    City = values["city"],
                    County = values["county"],
                    Street = values["street"],
                    HouseNumber = Convert.ToByte(values["house_number"]),
                    ZipCode = values["post_code"],
                    Phone = values["phone"],
                    Code = values["supplier_code"],
                });
            }
            return result.ToArray();
        }
        public override bool Insert(Supplier model) {
            string query = $"INSERT INTO {TableName} (name, post_code, county, city, street, house_number, phone, supplier_code) VALUES('{model.Name}', '{model.ZipCode}', '{model.County}', '{model.City}', '{model.Street}', {model.HouseNumber}, '{model.Phone}', 'BESZ{(model.Id).ToString().PadLeft(4 - model.Id.ToString().Length, '0')}');";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            } catch (Exception) {
                return false;
            }
        }
        public override bool Update(Supplier model) {
            string update = $"name = '{model.Name}', post_code = '{model.ZipCode}', county = '{model.County}', city = '{model.City}', street = '{model.Street}', house_number = '{model.HouseNumber}', phone = '{model.Phone}', supplier_code = '{model.Code}'";
            string query = $"UPDATE {TableName} SET {update} WHERE id = {model.Id}";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
        public override bool Delete(Supplier model) {
            var qDelete = $"DELETE FROM {TableName} WHERE id = {model.Id};";
            var qConnectionDelete = $"DELETE FROM {SupplierProductConnectionTable.TableName} WHERE supplier_id = {model.Id};";
            try {
                _connection.ExecuteWithoutReturn(qDelete);
                _connection.ExecuteWithoutReturn(qConnectionDelete);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
    public class ProductsTable : DatabaseTable<Product> {
        public const string TableName = "products";
        public ProductsTable(DatabaseConnection connection, bool withDummyData = false) {
            _connection = connection;
            _tableName = TableName;
            string createSuppliersTable =
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, name TEXT NOT NULL, category TEXT NOT NULL, price REAL NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData) {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, "gyumolcs.csv"));
            }
        }
        public override Product[] Read() {
            if (TableEntriesCount() == 0) {
                return Array.Empty<Product>();
            }
            List<Product> result = new List<Product>();
            var queryResult = _connection.ExecuteWithMultipleReturn($"SELECT * FROM {TableName};");
            while (queryResult.Read()) {
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < typeof(Supplier).GetProperties().Length; i++) {
                    values.Add(queryResult.GetName(i), queryResult.GetValue(i).ToString());
                }
                result.Add(new Product() {
                    Id = Convert.ToInt32(values["id"]),
                    Name = values["name"],
                    Category = values["category"],
                    Price = Convert.ToDouble(values["price"]),
                });
            }
            return result.ToArray();
        }
        public override bool Insert(Product model) {
            string query = $"INSERT INTO {TableName} (name, category, price) VALUES('{model.Name}', '{model.Category}', '{model.Price}');";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            } catch (Exception) {
                return false;
            }
        }
        public Product[] GetBySupplier(Supplier supplier) {
            var query = $"SELECT p.id, p.name, p.category, p.price FROM products AS p INNER JOIN supplier_product_connections spc ON p.id = spc.product_id AND spc.supplier_id = {supplier.Id};";
            var queryResult = _connection.ExecuteWithMultipleReturn(query);
            List<Product> result = new List<Product>();
            while (queryResult.Read()) {
                Dictionary<string, string> values = new Dictionary<string, string>();
                for (int i = 0; i < typeof(Product).GetProperties().Length; i++) {
                    values.Add(queryResult.GetName(i), queryResult.GetValue(i).ToString());
                }
                result.Add(new Product {
                    Id = Convert.ToInt32(values["id"]),
                    Category = values["category"],
                    Name = values["name"],
                    Price = Convert.ToDouble(values["price"])
                });
            }
            return result.ToArray();
        }
        public override bool Update(Product model) {
            string update = $"name = '{model.Name}', category = '{model.Category}', price = '{model.Price}'";
            string query = $"UPDATE {TableName} SET {update} WHERE id = {model.Id}";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
        public override bool Delete(Product model) {
            var qDelete = $"DELETE FROM {TableName} WHERE id = {model.Id};";
            var qConnectionDelete = $"DELETE FROM {SupplierProductConnectionTable.TableName} WHERE product_id = {model.Id};";
            try {
                _connection.ExecuteWithoutReturn(qDelete);
                _connection.ExecuteWithoutReturn(qConnectionDelete);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
    public class SupplyArrivalsTable : DatabaseTable<SupplyArrival> {
        public const string TableName = "supply_arrivals";
        public SupplyArrivalsTable(DatabaseConnection connection) {
            _connection = connection;
            string createSuppliersTable =
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL, arrival_time NUMERIC NOT NULL, quantity INTEGER NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
        }
        public override SupplyArrival[] Read() { //TODO
            if (TableEntriesCount() == 0) {
                return Array.Empty<SupplyArrival>();
            }
            return Array.Empty<SupplyArrival>();
        }
        public override bool Insert(SupplyArrival model) {
            string query = $"INSERT INTO {TableName} (supplier_id, product_id, arrival_time, quantity) VALUES('{model.SupplierId}', '{model.ProductId}', '{model.ArrivalTime.ToUnixTimestamp()}', '{model.Quantity}');";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            } catch (Exception) {
                return false;
            }
        }
        public override bool Update(SupplyArrival model) {
            string update = $"supplier_id = '{model.SupplierId}', product_id = '{model.ProductId}', arrival_time = '{model.ArrivalTime.ToUnixTimestamp()}', quantity = '{model.Quantity}'";
            string query = $"UPDATE {TableName} SET {update} WHERE id = {model.Id};";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
        public override bool Delete(SupplyArrival model) {
            var query = $"DELETE FROM {TableName} WHERE id = {model.Id}";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
    public class SupplierProductConnectionTable : DatabaseTable<SupplierProductConnection> {
        public const string TableName = "supplier_product_connections";
        public SupplierProductConnectionTable(DatabaseConnection connection, bool withDummyData = false) {
            _tableName = TableName;
            _connection = connection;
            string createSuppliersTable =
                $"CREATE TABLE IF NOT EXISTS {TableName} (id INTEGER PRIMARY KEY, supplier_id INTEGER NOT NULL, product_id INTEGER NOT NULL);";
            _connection.ExecuteWithoutReturn(createSuppliersTable);
            if (withDummyData) {
                WithDummyData(Path.Combine(DatabaseConnection.WorkDir, "kot.csv"));
            }
        }
        public override SupplierProductConnection[] Read() {
            return Array.Empty<SupplierProductConnection>(); //TODO
        }
        public override bool Insert(SupplierProductConnection model) {
            string query = $"INSERT INTO {TableName} (supplier_id, product_id) VALUES('{model.SupplierId}', '{model.ProductId}');";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
        public override bool Update(SupplierProductConnection model) {
            string update = $"supplier_id = '{model.SupplierId}', product_id = '{model.ProductId}";
            string query = $"UPDATE {TableName} SET {update} WHERE id = {model.Id};";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
        public override bool Delete(SupplierProductConnection model) {
            var query = $"DELETE FROM {TableName} WHERE supplier_id = '{model.SupplierId}' AND product_id = '{model.ProductId}';";
            try {
                _connection.ExecuteWithoutReturn(query);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
}