using System;
using ElectronicsStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStoreMVC.Services
{
	public class ApplicationDbContext:DbContext
	{
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        //public DbSet<Order> Orders { get; set; }
    }
}

