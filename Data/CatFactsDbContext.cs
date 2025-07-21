using CatFactsWebApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CatFactsWebApp.Data
{
    /// <summary>
    /// Kontekst bazy danych dla aplikacji Cat Facts
    /// </summary>
    public class CatFactsDbContext : DbContext
    {
        public CatFactsDbContext(DbContextOptions<CatFactsDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Zbiór faktów o kotach
        /// </summary>
        public DbSet<CatFact> CatFacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja encji CatFact
            modelBuilder.Entity<CatFact>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Fact)
                    .IsRequired()
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Category)
                    .HasMaxLength(100);
                
                entity.Property(e => e.CreatedAt);
                
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsFavorite);
                entity.HasIndex(e => e.Rating);
            });

            // Dane przykładowe
            modelBuilder.Entity<CatFact>().HasData(
                new CatFact
                {
                    Id = 1,
                    Fact = "Koty mają 32 mięśnie w każdym uchu, co pozwala im obracać uszami o 180 stopni.",
                    Length = 78,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    IsFavorite = true,
                    Category = "Anatomia",
                    Rating = 5
                },
                new CatFact
                {
                    Id = 2,
                    Fact = "Kot może biegać z prędkością do 48 km/h na krótkich dystansach.",
                    Length = 64,
                    CreatedAt = DateTime.Now.AddHours(-2),
                    IsFavorite = false,
                    Category = "Zachowanie",
                    Rating = 4
                }
            );
        }
    }
}
