using CatFactsWebApp.Models.Domain;
using CatFactsWebApp.Models.ViewModels;

namespace CatFactsWebApp.Services
{
    /// <summary>
    /// Interfejs serwisu biznesowego do zarządzania faktami o kotach
    /// Zgodny z zasadą Single Responsibility - odpowiada za logikę biznesową
    /// </summary>
    public interface ICatFactBusinessService
    {
        /// <summary>
        /// Pobiera dane dla strony głównej
        /// </summary>
        Task<HomeViewModel> GetHomeDataAsync();

        /// <summary>
        /// Pobiera i zapisuje nowy fakt z API
        /// </summary>
        Task<CatFactViewModel?> FetchAndSaveNewFactAsync();

        /// <summary>
        /// Pobiera i zapisuje wiele nowych faktów z API
        /// </summary>
        Task<IEnumerable<CatFactViewModel>> FetchAndSaveMultipleFactsAsync(int count = 5);

        /// <summary>
        /// Filtruje fakty według kryteriów
        /// </summary>
        Task<IEnumerable<CatFactViewModel>> FilterFactsAsync(FilterViewModel filter);

        /// <summary>
        /// Oznacza/odznacza fakt jako ulubiony
        /// </summary>
        Task<bool> ToggleFavoriteAsync(int id);

        /// <summary>
        /// Ocenia fakt
        /// </summary>
        Task<bool> RateFactAsync(int id, int rating);

        /// <summary>
        /// Usuwa fakt
        /// </summary>
        Task<bool> DeleteFactAsync(int id);

        /// <summary>
        /// Pobiera fakt po ID
        /// </summary>
        Task<CatFactViewModel?> GetFactByIdAsync(int id);

        /// <summary>
        /// Pobiera wszystkie dostępne kategorie
        /// </summary>
        Task<IEnumerable<string>> GetCategoriesAsync();

        /// <summary>
        /// Eksportuje fakty do pliku tekstowego
        /// </summary>
        Task<string> ExportFactsToFileAsync(FilterViewModel? filter = null);
    }
}
