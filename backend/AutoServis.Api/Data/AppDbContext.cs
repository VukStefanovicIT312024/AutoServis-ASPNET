using AutoServis.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoServis.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.PlateNumber)
                .IsUnique();

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    Name = "Mali servis",
                    Category = "Održavanje",
                    Description = "Zamena ulja, filtera ulja i osnovna provera vozila.",
                    Price = 7500,
                    Duration = 60,
                    Includes = "Zamena motornog ulja|Zamena filtera ulja|Provera tečnosti|Vizuelni pregled vozila"
                },
                new Service
                {
                    Id = 2,
                    Name = "Veliki servis",
                    Category = "Održavanje",
                    Description = "Detaljan servis vozila koji obuhvata zamenu ključnih potrošnih delova.",
                    Price = 28000,
                    Duration = 180,
                    Includes = "Zamena zupčastog kaiša|Zamena španera|Zamena vodene pumpe|Kontrola sistema hlađenja"
                },
                new Service
                {
                    Id = 3,
                    Name = "Dijagnostika vozila",
                    Category = "Dijagnostika",
                    Description = "Računarska dijagnostika i očitavanje grešaka na vozilu.",
                    Price = 3500,
                    Duration = 45,
                    Includes = "Očitavanje grešaka|Analiza parametara|Brisanje grešaka|Preporuka za popravku"
                },
                new Service
                {
                    Id = 4,
                    Name = "Zamena kočnica",
                    Category = "Popravka",
                    Description = "Pregled i zamena kočionih pločica ili diskova po potrebi.",
                    Price = 12000,
                    Duration = 90,
                    Includes = "Pregled kočionog sistema|Zamena pločica|Kontrola diskova|Test kočenja"
                },
                new Service
                {
                    Id = 5,
                    Name = "Priprema za tehnički pregled",
                    Category = "Registracija",
                    Description = "Provera osnovne ispravnosti vozila pre tehničkog pregleda.",
                    Price = 5000,
                    Duration = 60,
                    Includes = "Provera svetlosne signalizacije|Provera kočnica|Provera pneumatika|Provera osnovne dokumentacije"
                }
            );
        }
    }
}