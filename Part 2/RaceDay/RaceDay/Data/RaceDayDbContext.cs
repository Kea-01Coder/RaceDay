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
            .ToTable(t => t.HasCheckConstraint("CK_User_Role", "[Role] IN ('Organiser', 'Participant','Admin')"));

            modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            modelBuilder.Entity<User>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt

            //modelBuilder for Event entity
            modelBuilder.Entity<Event>()
            .HasOne(u => u.User)        // Category has one Event
            .WithMany(ev => ev.Events) // Event has many Categories
            .HasForeignKey(u => u.CreatedBy);

            modelBuilder.Entity<Event>()
            .Property(ev => ev.EntryFee)
            .HasPrecision(10, 2); // Set precision for EntryFee

            modelBuilder.Entity<Event>()
            .Property(ev => ev.EarlyBirdFee)
            .HasPrecision(10, 2); // Set precision for EarlyBirdFee

            modelBuilder.Entity<Event>()
            .ToTable(t => t.HasCheckConstraint("CK_Event_Status", "[Status] IN ('upcoming', 'ongoing', 'completed', 'cancelled')"));

            modelBuilder.Entity<Event>()
            .Property(ev => ev.Status)
            .HasDefaultValue("upcoming"); // Set default value for Status

            modelBuilder.Entity<Event>()
            .Property(ev => ev.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            modelBuilder.Entity<Event>()
            .Property(ev => ev.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt

            //modelBuilder for Category entity
            modelBuilder.Entity<Category>()
            .HasOne(c => c.Event)        // Category has one Event
            .WithMany(ev => ev.Categories) // Event has many Categories
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Cascade); // This creates ON DELETE CASCADE

            modelBuilder.Entity<Category>()
            .Property(c => c.DistanceKM)
            .HasPrecision(5, 2); // Set precision for DistanceKM

            modelBuilder.Entity<Category>()
            .Property(c => c.EntryFee)
            .HasPrecision(10, 2); // Set precision for EntryFee

            modelBuilder.Entity<Category>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            //modelBuilder for Enrollment entity
            modelBuilder.Entity<Enrollment>()
             .HasOne(e => e.Category)        // Enrollment has one Category
             .WithMany(c => c.Enrollments) // Category has many Enrollments
             .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Enrollment>()
             .HasOne(u => u.User)        // Category has one Event
             .WithMany(e => e.Enrollments) // Event has many Enrollments
             .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Event)        // Category has one Event
            .WithMany(ev => ev.Enrollments) // Event has many Enrollments
            .HasForeignKey(ev => ev.EventId);

            modelBuilder.Entity<Enrollment>()
            .ToTable(t=> t.HasCheckConstraint("CK_Enrollment_Status", "[Status] IN ('pending', 'confirmed', 'cancelled', 'completed', 'DNS', 'DNF')"));

            modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasDefaultValue("pending"); // Set default value for Status

            modelBuilder.Entity<Enrollment>()
            .ToTable(t => t.HasCheckConstraint("CK_Enrollment_PaymentStatus", "[PaymentStatus] IN ('pending', 'paid', 'refunded' ,'failed')"));
            
            modelBuilder.Entity<Enrollment>()
             .Property(e => e.PaymentStatus)
             .HasDefaultValue("pending"); // Set default value for PaymentStatus
            
            modelBuilder.Entity<Enrollment>()
            .ToTable(t => t.HasCheckConstraint("CK_Enrollment_PaymentMethod", "[PaymentMethod] IN ('card', 'eft', 'snapscan', 'zapper', 'cash', 'voucher')"));

            modelBuilder.Entity<Enrollment>()
             .Property(e => e.AmountPaid)
             .HasPrecision(10, 2); // Set precision for AmountPaid

            modelBuilder.Entity<Enrollment>()
            .ToTable(t => t.HasCheckConstraint("CK_Enrollment_Gender", "[Gender] IN ('Male', 'Female', 'Other')"));

            modelBuilder.Entity<Enrollment>()
            .Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt

            modelBuilder.Entity<Enrollment>()
            .HasIndex(en => new { en.EventId, en.CategoryId, en.UserId })
            .IsUnique()
            .HasDatabaseName("UC_Enrollment"); // Unique index on EventId, CategoryId, and UserId

            //modelBuilder for Result entity
            modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrollment)        // Result has one Enrollment      
            .WithMany()
            .HasForeignKey(r => r.EnrollmentId);

            modelBuilder.Entity<Result>()
            .Property(r => r.Certified)
            .HasDefaultValue(false); // Set default value for Certified

            modelBuilder.Entity<Result>()
            .Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for CreatedAt

            modelBuilder.Entity<Result>()
            .Property(r => r.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt

            modelBuilder.Entity<WeatherInfo>()
            .HasOne(w => w.Event)        // WeatherInfo has one Event
            .WithMany(ev => ev.WeatherInfos) // Event has many WeatherInfos
            .HasForeignKey(w => w.EventId);

            modelBuilder.Entity<WeatherInfo>()
            .Property(w => w.Temperature)
            .HasPrecision(5, 2); // Set precision for Temperature

            modelBuilder.Entity<WeatherInfo>()
            .Property(w => w.WindSpeed)
            .HasPrecision(5, 2); // Set precision for WindSpeed

            modelBuilder.Entity<WeatherInfo>()
            .Property(w => w.Humidity)
            .HasPrecision(5, 2); // Set precision for Humidity

            modelBuilder.Entity<WeatherInfo>()
            .Property(w => w.UpdatedAt)
            .HasDefaultValueSql("GETDATE()"); // Set default value for UpdatedAt    
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<WeatherInfo> WeatherInfos { get; set; }

    }
}
