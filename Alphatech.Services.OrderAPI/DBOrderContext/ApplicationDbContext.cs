using Alphatech.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Alphatech.Services.OrderAPI.Models.Dto;

namespace Alphatech.Services.OrderAPI.DBOrderContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        // Override OnConfiguring to enable logging
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Data Source=TEMP7070;Initial Catalog=OrderServiceDB;User ID=sa;Password=mis@123;MultipleActiveResultSets=True;TrustServerCertificate=Yes;Connection Timeout=60", sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(60); // Set the timeout to 60 seconds
                    })
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine);
            }
        }
    }
}
