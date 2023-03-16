using BankRestAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace BankRestAPI.Data
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bank> Bank { get; set; }
        public DbSet<Customer> Customer { get; set; }  
        public DbSet<Account> Account { get; set; }
        public DbSet<Transfer> Transfer { get; set; }

    }
}
