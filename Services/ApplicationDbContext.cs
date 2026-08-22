using BestStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreAPI.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Contact>Contacts { get; set; }
        public DbSet<Prodect> Prodects { get; set; }
        public DbSet <Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
    
}
