using CatFactsWebApp.Models.Domain;
using CatFactsWebApp.Models.ViewModels;
using CatFactsWebApp.Repositories;

namespace CatFactsWebApp.Services
{
    /// <summary>
    /// Serwis biznesowy do zarządzania faktami o kotach
    /// Implementuje zasady SOLID:
    /// - Single Responsibility: logika biznesowa aplikacji
    /// - Open/Closed: można rozszerzyć bez modyfikacji
    /// - Dependency Inversion: zależy od abstrakcji (interfejsów)
    /// </summary>
    public class CatFactBusinessService : ICatFactBusinessService
    {
        private readonly ICatFactRepository _repository;
        private readonly ICatFactService _catFactService;
        private readonly ILogger<CatFactBusinessService> _logger;

        public CatFactBusinessService(
            ICatFactRepository repository,
            ICatFactService catFactService,
            ILogger<CatFactBusinessService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _catFactService = catFactService ?? throw new ArgumentNullException(nameof(catFactService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HomeViewModel> GetHomeDataAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie danych dla strony głównej");

                var recentFactsTask = _repository.GetRecentAsync(10);
                var favoriteFactsTask = _repository.GetFavoritesAsync();
                var statisticsTask = _repository.GetStatisticsAsync();

                await Task.WhenAll(recentFactsTask, favoriteFactsTask, statisticsTask);

                var recentFacts = await recentFactsTask;
                var favoriteFacts = await favoriteFactsTask;
                var statistics = await statisticsTask;

                var viewModel = new HomeViewModel
                {
                    RecentFacts = recentFacts.Select(MapToViewModel).ToList(),
                    FavoriteFacts = favoriteFacts.Take(5).Select(MapToViewModel).ToList(),
                    TotalFactsCount = statistics.TotalCount,
                    AverageRating = statistics.AverageRating
                };

                _logger.LogInformation("Pobrano dane dla strony głównej: {TotalFacts} faktów", statistics.TotalCount);
                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania danych dla strony głównej");
                return new HomeViewModel();
            }
        }

        public async Task<CatFactViewModel?> FetchAndSaveNewFactAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie i zapisywanie nowego faktu z API");

                var newFact = await _catFactService.GetRandomCatFactAsync();
                if (newFact == null)
                {
                    _logger.LogWarning("Nie udało się pobrać faktu z API");
                    return null;
                }

                // Sprawdź czy fakt już istnieje
                if (await _repository.ExistsAsync(newFact.Fact))
                {
                    _logger.LogInformation("Fakt już istnieje w bazie danych");
                    return null;
                }

                var savedFact = await _repository.AddAsync(newFact);
                var viewModel = MapToViewModel(savedFact);

                _logger.LogInformation("Pomyślnie zapisano nowy fakt: ID {FactId}", savedFact.Id);
                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania i zapisywania nowego faktu");
                return null;
            }
        }

        public async Task<IEnumerable<CatFactViewModel>> FetchAndSaveMultipleFactsAsync(int count = 5)
        {
            try
            {
                _logger.LogInformation("Pobieranie i zapisywanie {Count} faktów z API", count);

                var newFacts = await _catFactService.GetMultipleCatFactsAsync(count);
                var savedFacts = new List<CatFact>();

                foreach (var fact in newFacts)
                {
                    if (fact != null && !await _repository.ExistsAsync(fact.Fact))
                    {
                        var savedFact = await _repository.AddAsync(fact);
                        savedFacts.Add(savedFact);
                    }
                }

                var viewModels = savedFacts.Select(MapToViewModel).ToList();
                _logger.LogInformation("Pomyślnie zapisano {SavedCount} z {RequestedCount} faktów", savedFacts.Count, count);
                
                return viewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania i zapisywania wielu faktów");
                return Enumerable.Empty<CatFactViewModel>();
            }
        }

        public async Task<IEnumerable<CatFactViewModel>> FilterFactsAsync(FilterViewModel filter)
        {
            try
            {
                _logger.LogInformation("Filtrowanie faktów według kryteriów");

                var facts = await _repository.FilterAsync(filter);
                return facts.Select(MapToViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas filtrowania faktów");
                return Enumerable.Empty<CatFactViewModel>();
            }
        }

        public async Task<bool> ToggleFavoriteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Przełączanie statusu ulubionego dla faktu ID {FactId}", id);
                return await _repository.ToggleFavoriteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas przełączania statusu ulubionego dla faktu ID {FactId}", id);
                return false;
            }
        }

        public async Task<bool> RateFactAsync(int id, int rating)
        {
            try
            {
                _logger.LogInformation("Ocenianie faktu ID {FactId} na {Rating}", id, rating);
                return await _repository.RateAsync(id, rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas oceniania faktu ID {FactId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteFactAsync(int id)
        {
            try
            {
                _logger.LogInformation("Usuwanie faktu ID {FactId}", id);
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania faktu ID {FactId}", id);
                return false;
            }
        }

        public async Task<CatFactViewModel?> GetFactByIdAsync(int id)
        {
            try
            {
                var fact = await _repository.GetByIdAsync(id);
                return fact != null ? MapToViewModel(fact) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania faktu ID {FactId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                var facts = await _repository.GetAllAsync();
                return facts
                    .Where(f => !string.IsNullOrWhiteSpace(f.Category))
                    .Select(f => f.Category!)
                    .Distinct()
                    .OrderBy(c => c);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania kategorii");
                return Enumerable.Empty<string>();
            }
        }

        public async Task<string> ExportFactsToFileAsync(FilterViewModel? filter = null)
        {
            try
            {
                _logger.LogInformation("Eksportowanie faktów do pliku");

                var facts = filter != null 
                    ? await _repository.FilterAsync(filter)
                    : await _repository.GetAllAsync();

                var fileName = $"cat_facts_export_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                var lines = new List<string>
                {
                    "=== EKSPORT FAKTÓW O KOTACH ===",
                    $"Data eksportu: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                    $"Liczba faktów: {facts.Count()}",
                    ""
                };

                foreach (var fact in facts.OrderByDescending(f => f.CreatedAt))
                {
                    lines.Add($"[{fact.CreatedAt:dd.MM.yyyy HH:mm}] {fact.Fact}");
                    lines.Add($"   Kategoria: {fact.Category ?? "Brak"} | Ocena: {fact.Rating}/5 | Ulubiony: {(fact.IsFavorite ? "Tak" : "Nie")}");
                    lines.Add("");
                }

                await File.WriteAllLinesAsync(filePath, lines);
                
                _logger.LogInformation("Wyeksportowano {Count} faktów do pliku {FilePath}", facts.Count(), filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas eksportowania faktów do pliku");
                throw;
            }
        }

        /// <summary>
        /// Mapuje model domenowy na ViewModel
        /// </summary>
        private static CatFactViewModel MapToViewModel(CatFact fact)
        {
            return new CatFactViewModel
            {
                Id = fact.Id,
                Fact = fact.Fact,
                Length = fact.Length,
                CreatedAt = fact.CreatedAt,
                IsFavorite = fact.IsFavorite,
                Category = fact.Category,
                Rating = fact.Rating
            };
        }
    }
}
