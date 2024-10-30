using ElectricityBillService.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityBillService.DataAccess
{
     

    public class ElectricityBillDBContext : DbContext
    {
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        public ElectricityBillDBContext(DbContextOptions<ElectricityBillDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define entity configurations here if necessary
            modelBuilder.Entity<Bill>()
                .Property(b => b.Status)
                .HasConversion<int>(); // Enum to int conversion

            base.OnModelCreating(modelBuilder);
        }
    }
}
