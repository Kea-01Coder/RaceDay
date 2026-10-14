using Microsoft.EntityFrameworkCore;
using RaceDay.Models;
namespace RaceDay.Data
{
    public class RaceDayDbContext : DbContext
    {

        
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .ToTable(t=>t.HasCheckConstraint("CK_User_Role", "[Role] IN ('Organiser', 'Participant')"));

            modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)        // Category has one Event
            .WithMany(e => e.Categories) // Event has many Categories
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade); // This creates ON DELETE CASCADE

            modelBuilder.Entity<Category>()
            .Property(c=>c.DistanceKM)
            .HasPrecision(5,2); // Set precision for DistanceKM

            modelBuilder.Entity<Category>()
            .Property(c => c.EntryFee)
            .HasPrecision(10, 2); // Set precision for EntryFee

            modelBuilder.Entity<Category>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<WeatherInfo> WeatherInfos { get; set; }

    }
}
