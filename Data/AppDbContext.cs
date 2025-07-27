using Microsoft.EntityFrameworkCore;
using ActivityPerson.Models;
using System.Threading.Tasks;

namespace ActivityPerson.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<DataDay> DataDays { get; set; }
        public DbSet<Models.Task> Tasks { get; set; } // Явное указание пространства имен

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=host;Port=port;Database=ActivityDB;Username=postgres;Password=password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DataDay>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.ActivityLevel).IsRequired();
            });

            modelBuilder.Entity<Models.Task>(entity => // Явное указание пространства имен
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Description).IsRequired().HasMaxLength(255);
                entity.Property(t => t.DataDayId).IsRequired();
                entity.HasOne(t => t.DataDay)
                    .WithMany(d => d.Tasks)
                    .HasForeignKey(t => t.DataDayId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
