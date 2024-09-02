using Microsoft.EntityFrameworkCore;
using interviewProject.Models;

namespace interviewProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Mba> Mbas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>()
                .HasIndex(c => c.CountryCode)
                .IsUnique();

            modelBuilder.Entity<Mba>()
                .HasIndex(m => m.Name)
                .IsUnique();
        }
    }
}
