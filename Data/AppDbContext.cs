using Microsoft.EntityFrameworkCore;
using EventManagementAPI.Models;
using System.Collections.Generic;

namespace EventManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<TicketTransaction> TicketTransactions { get; set; }

    }
}
