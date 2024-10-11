using Alphatech.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Alphatech.Services.ProductAPI.Models.Dto;

namespace Alphatech.Services.ProductAPI.DBProductContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Alphatech.Services.ProductAPI.Models.Dto.ProductDto> ProductDto { get; set; } = default!;

        // Override OnConfiguring to enable logging
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Data Source=TEMP7070;Initial Catalog=AlphaTech;User ID=sa;Password=mis@123;MultipleActiveResultSets=True;TrustServerCertificate=Yes;Connection Timeout=60", sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(60); // Set the timeout to 60 seconds
                    })
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine);
            }
        }
    }
}
