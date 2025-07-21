using CatFactsWebApp.Models.Domain;
using CatFactsWebApp.Models.ViewModels;

namespace CatFactsWebApp.Repositories
{
    /// <summary>
    /// Interfejs repozytorium dla faktów o kotach - zgodny z zasadą Dependency Inversion (SOLID)
    /// </summary>
    public interface ICatFactRepository
    {
        /// <summary>
        /// Pobiera wszystkie fakty o kotach
        /// </summary>
        Task<IEnumerable<CatFact>> GetAllAsync();

        /// <summary>
        /// Pobiera fakt o kocie po ID
        /// </summary>
        Task<CatFact?> GetByIdAsync(int id);

        /// <summary>
        /// Pobiera najnowsze fakty o kotach
        /// </summary>
        Task<IEnumerable<CatFact>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Pobiera ulubione fakty o kotach
        /// </summary>
        Task<IEnumerable<CatFact>> GetFavoritesAsync();

        /// <summary>
        /// Filtruje fakty według kryteriów
        /// </summary>
        Task<IEnumerable<CatFact>> FilterAsync(FilterViewModel filter);

        /// <summary>
        /// Dodaje nowy fakt o kocie
        /// </summary>
        Task<CatFact> AddAsync(CatFact catFact);

        /// <summary>
        /// Aktualizuje fakt o kocie
        /// </summary>
        Task<CatFact> UpdateAsync(CatFact catFact);

        /// <summary>
        /// Usuwa fakt o kocie
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Oznacza/odznacza fakt jako ulubiony
        /// </summary>
        Task<bool> ToggleFavoriteAsync(int id);

        /// <summary>
        /// Ocenia fakt o kocie
        /// </summary>
        Task<bool> RateAsync(int id, int rating);

        /// <summary>
        /// Pobiera statystyki faktów
        /// </summary>
        Task<(int TotalCount, double AverageRating)> GetStatisticsAsync();

        /// <summary>
        /// Sprawdza czy fakt już istnieje w bazie
        /// </summary>
        Task<bool> ExistsAsync(string fact);
    }
}
