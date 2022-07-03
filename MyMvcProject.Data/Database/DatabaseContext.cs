using Microsoft.EntityFrameworkCore;
using MyMvcProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMvcProject.Data.Database
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Production> Production { get; set; }
        public DbSet<ProductionDetail> ProductionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {

                relationship.DeleteBehavior = DeleteBehavior.SetNull;
                relationship.IsRequired = false;

            }
        }
    }
}
