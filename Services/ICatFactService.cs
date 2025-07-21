using CatFactsWebApp.Models.Domain;

namespace CatFactsWebApp.Services
{
    /// <summary>
    /// Interfejs serwisu do pobierania faktów o kotach z zewnętrznego API
    /// Zgodny z zasadą Single Responsibility i Interface Segregation (SOLID)
    /// </summary>
    public interface ICatFactService
    {
        /// <summary>
        /// Pobiera losowy fakt o kocie z API
        /// </summary>
        Task<CatFact?> GetRandomCatFactAsync();

        /// <summary>
        /// Pobiera wiele losowych faktów o kotach
        /// </summary>
        Task<IEnumerable<CatFact>> GetMultipleCatFactsAsync(int count = 5);

        /// <summary>
        /// Sprawdza dostępność API
        /// </summary>
        Task<bool> IsApiAvailableAsync();
    }
}
