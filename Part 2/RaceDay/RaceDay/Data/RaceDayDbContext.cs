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

            //modelBuilder for User entity
            modelBuilder.Entity<User>()
            .ToTable(t=>t.HasCheckConstraint("CK_User_Role", "[Role] IN ('Organiser', 'Participant','Admin')"));

            modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt

            //modelBuilder for Event entity
            modelBuilder.Entity<Event>()
            .HasOne(c => c.User)        // Category has one Event
            .WithMany(e => e.Events) // Event has many Categories
            .HasForeignKey(c => c.CreatedBy);

            modelBuilder.Entity<Event>()
            .Property(e=>e.EntryFee)
            .HasPrecision(10, 2); // Set precision for EntryFee

            modelBuilder.Entity<Event>()
            .Property(e => e.EarlyBirdFee)
            .HasPrecision(10, 2); // Set precision for EarlyBirdFee

            modelBuilder.Entity<Event>()
            .ToTable(t=>t.HasCheckConstraint("CK_Event_Status", "[Status] IN ('upcoming', 'ongoing', 'completed', 'cancelled')"));

            modelBuilder.Entity<Event>()
            .Property(e=>e.Status)
            .HasDefaultValue("upcoming"); // Set default value for Status

            modelBuilder.Entity<Event>()
            .Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            modelBuilder.Entity<Event>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt

            //modelBuilder for Category entity
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
