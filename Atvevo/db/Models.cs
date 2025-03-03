using System;

namespace Atvevo.db
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public byte HouseNumber { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
    }
    public class SupplyArrival
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Quantity { get; set; }
    }
    public class SupplierProductConnection
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
    }
}