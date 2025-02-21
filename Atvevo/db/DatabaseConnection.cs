using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Atvevo.db
{
    public abstract class Database
    {
        private string DatabaseFilename { get; }
        public void Create(){}
        public virtual void Read(){}
        public virtual void Update(){}
        public virtual void Delete(){}
    }
    public class DatabaseConnection : Database
    {
        private string DatabaseFilename
        {
            get { return "atvevo.db"; }
        }
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
            CreateTables();
            if (withDummyData)
            {
                InsertDummyData("besz.csv");
            }
        }
        private string DbConnection()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."), "db", DatabaseFilename);
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
            using (StreamReader sr = new StreamReader(csvFileName))
            {
                var headers = sr.ReadLine()?.Split(',');
                var data = sr.ReadLine();
                while (data != null)
                {
                    var splitedData = data.Split(',');
                    Dictionary<string, string> dict = headers?.Zip(splitedData, (first, second) => new { first, second }).ToDictionary(x => x.first, x => x.second);
                    Create("suppliers", dict);
                    data = sr.ReadLine();
                }
            }
        }
        public void Create(string table, Dictionary<string, string> columns)
        {
            string databaseData = "";
            foreach (var data in columns)
            {
                databaseData += data.Value + ", ";
            }
            string command = $"INSERT INTO {table} VALUES({databaseData})";
            try
            {
                Execute(_connection, command);
            }
            catch (Exception e)
            {
                _connection.LogMessage(SQLiteErrorCode.Error, $"Failed to insert into table {table}");
                throw;
            }
            //TODO add a functionality so it can handle dummy data with header line not in order
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