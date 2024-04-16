using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trainbookingsystem.Models;

namespace Trainbookingsystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<BookingTicket> BookingTickets { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }      
        
    }
}

