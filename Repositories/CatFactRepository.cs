using CatFactsWebApp.Data;
using CatFactsWebApp.Models.Domain;
using CatFactsWebApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CatFactsWebApp.Repositories
{
    /// <summary>
    /// Implementacja repozytorium dla faktów o kotach - zgodna z zasadami SOLID
    /// Single Responsibility: odpowiada tylko za dostęp do danych
    /// Open/Closed: można rozszerzyć bez modyfikacji
    /// Liskov Substitution: implementuje interfejs ICatFactRepository
    /// Interface Segregation: implementuje tylko potrzebne metody
    /// Dependency Inversion: zależy od abstrakcji (DbContext)
    /// </summary>
    public class CatFactRepository : ICatFactRepository
    {
        private readonly CatFactsDbContext _context;

        public CatFactRepository(CatFactsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CatFact>> GetAllAsync()
        {
            return await _context.CatFacts
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<CatFact?> GetByIdAsync(int id)
        {
            return await _context.CatFacts.FindAsync(id);
        }

        public async Task<IEnumerable<CatFact>> GetRecentAsync(int count = 10)
        {
            return await _context.CatFacts
                .OrderByDescending(f => f.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatFact>> GetFavoritesAsync()
        {
            return await _context.CatFacts
                .Where(f => f.IsFavorite)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatFact>> FilterAsync(FilterViewModel filter)
        {
            var query = _context.CatFacts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(f => f.Fact.Contains(filter.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(f => f.Category == filter.Category);
            }

            if (filter.OnlyFavorites)
            {
                query = query.Where(f => f.IsFavorite);
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(f => f.Rating >= filter.MinRating.Value);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(f => f.CreatedAt >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                query = query.Where(f => f.CreatedAt <= filter.DateTo.Value.AddDays(1));
            }

            return await query
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<CatFact> AddAsync(CatFact catFact)
        {
            if (catFact == null)
                throw new ArgumentNullException(nameof(catFact));

            catFact.CreatedAt = DateTime.Now;
            _context.CatFacts.Add(catFact);
            await _context.SaveChangesAsync();
            return catFact;
        }

        public async Task<CatFact> UpdateAsync(CatFact catFact)
        {
            if (catFact == null)
                throw new ArgumentNullException(nameof(catFact));

            _context.CatFacts.Update(catFact);
            await _context.SaveChangesAsync();
            return catFact;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var catFact = await _context.CatFacts.FindAsync(id);
            if (catFact == null)
                return false;

            _context.CatFacts.Remove(catFact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleFavoriteAsync(int id)
        {
            var catFact = await _context.CatFacts.FindAsync(id);
            if (catFact == null)
                return false;

            catFact.IsFavorite = !catFact.IsFavorite;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RateAsync(int id, int rating)
        {
            if (rating < 1 || rating > 5)
                return false;

            var catFact = await _context.CatFacts.FindAsync(id);
            if (catFact == null)
                return false;

            catFact.Rating = rating;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(int TotalCount, double AverageRating)> GetStatisticsAsync()
        {
            var facts = await _context.CatFacts.ToListAsync();
            var totalCount = facts.Count;
            var averageRating = facts.Any() ? facts.Where(f => f.Rating > 0).Average(f => f.Rating) : 0;
            
            return (totalCount, Math.Round(averageRating, 2));
        }

        public async Task<bool> ExistsAsync(string fact)
        {
            if (string.IsNullOrWhiteSpace(fact))
                return false;

            return await _context.CatFacts
                .AnyAsync(f => f.Fact.Equals(fact, StringComparison.OrdinalIgnoreCase));
        }
    }
}
