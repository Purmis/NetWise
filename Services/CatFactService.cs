using System.Text.Json;
using CatFactsWebApp.Models.Domain;

namespace CatFactsWebApp.Services
{
    /// <summary>
    /// Serwis do pobierania faktów o kotach z API catfact.ninja
    /// Implementuje zasady SOLID:
    /// - Single Responsibility: odpowiada tylko za komunikację z zewnętrznym API
    /// - Open/Closed: można rozszerzyć bez modyfikacji
    /// - Dependency Inversion: zależy od abstrakcji HttpClient
    /// </summary>
    public class CatFactService : ICatFactService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatFactService> _logger;
        private const string ApiUrl = "https://catfact.ninja/fact";

        public CatFactService(HttpClient httpClient, ILogger<CatFactService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CatFact?> GetRandomCatFactAsync()
        {
            try
            {
                _logger.LogInformation("Pobieranie losowego faktu o kocie z API");
                
                var response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiCatFactResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null || string.IsNullOrWhiteSpace(apiResponse.Fact))
                {
                    _logger.LogWarning("API zwróciło pustą odpowiedź");
                    return null;
                }

                var catFact = new CatFact
                {
                    Fact = apiResponse.Fact.Trim(),
                    Length = apiResponse.Length,
                    CreatedAt = DateTime.Now,
                    Category = DetermineCategory(apiResponse.Fact),
                    Rating = 0,
                    IsFavorite = false
                };

                _logger.LogInformation("Pomyślnie pobrano fakt o kocie: {FactLength} znaków", catFact.Length);
                return catFact;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Błąd HTTP podczas pobierania danych z API");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Błąd podczas deserializacji JSON");
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout podczas pobierania danych z API");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nieoczekiwany błąd podczas pobierania faktu o kocie");
                return null;
            }
        }

        public async Task<IEnumerable<CatFact>> GetMultipleCatFactsAsync(int count = 5)
        {
            if (count <= 0 || count > 20)
                throw new ArgumentOutOfRangeException(nameof(count), "Liczba faktów musi być między 1 a 20");

            var facts = new List<CatFact>();
            var tasks = new List<Task<CatFact?>>();

            // Równoległe pobieranie faktów
            for (int i = 0; i < count; i++)
            {
                tasks.Add(GetRandomCatFactAsync());
                // Małe opóźnienie aby uniknąć rate limiting
                await Task.Delay(100);
            }

            var results = await Task.WhenAll(tasks);
            
            foreach (var fact in results)
            {
                if (fact != null)
                {
                    facts.Add(fact);
                }
            }

            _logger.LogInformation("Pobrano {ActualCount} z {RequestedCount} faktów o kotach", facts.Count, count);
            return facts;
        }

        public async Task<bool> IsApiAvailableAsync()
        {
            try
            {
                _logger.LogInformation("Sprawdzanie dostępności API");
                
                var response = await _httpClient.GetAsync(ApiUrl);
                var isAvailable = response.IsSuccessStatusCode;
                
                _logger.LogInformation("API jest {Status}", isAvailable ? "dostępne" : "niedostępne");
                return isAvailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas sprawdzania dostępności API");
                return false;
            }
        }

        /// <summary>
        /// Próbuje określić kategorię faktu na podstawie jego treści
        /// </summary>
        private string? DetermineCategory(string fact)
        {
            if (string.IsNullOrWhiteSpace(fact))
                return null;

            var factLower = fact.ToLowerInvariant();

            if (factLower.Contains("muscle") || factLower.Contains("mięsień") || 
                factLower.Contains("ear") || factLower.Contains("ucho") ||
                factLower.Contains("eye") || factLower.Contains("oko") ||
                factLower.Contains("tail") || factLower.Contains("ogon"))
                return "Anatomia";

            if (factLower.Contains("sleep") || factLower.Contains("śpi") ||
                factLower.Contains("hunt") || factLower.Contains("poluj") ||
                factLower.Contains("play") || factLower.Contains("baw"))
                return "Zachowanie";

            if (factLower.Contains("year") || factLower.Contains("rok") ||
                factLower.Contains("age") || factLower.Contains("wiek") ||
                factLower.Contains("live") || factLower.Contains("żyj"))
                return "Długość życia";

            if (factLower.Contains("breed") || factLower.Contains("rasa") ||
                factLower.Contains("species") || factLower.Contains("gatunek"))
                return "Rasy";

            return "Ogólne";
        }
    }

    /// <summary>
    /// Model odpowiedzi z API catfact.ninja
    /// </summary>
    internal class ApiCatFactResponse
    {
        public string Fact { get; set; } = string.Empty;
        public int Length { get; set; }
    }
}
