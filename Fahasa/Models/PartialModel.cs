using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class PartialModel
    {
        public List<Author> Authors { get; set; }
        public List<Category> Categories { get; set; }
        public List<Cover> Covers { get; set; }
        public List<Discount> Discounts { get; set; }
        public List<Gallery> Galleries { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Person> People { get; set; }
        public List<Product> Products { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public List<Group> Groups { get; set; }
    }
}