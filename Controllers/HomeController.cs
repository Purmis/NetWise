using CatFactsWebApp.Models.ViewModels;
using CatFactsWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CatFactsWebApp.Controllers
{
    /// <summary>
    /// Główny kontroler aplikacji Cat Facts
    /// Implementuje zasady SOLID - Single Responsibility, Dependency Inversion
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ICatFactBusinessService _businessService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ICatFactBusinessService businessService, ILogger<HomeController> logger)
        {
            _businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Strona główna z przeglądem faktów
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Ładowanie strony głównej");
                var homeData = await _businessService.GetHomeDataAsync();
                return View(homeData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas ładowania strony głównej");
                return View(new HomeViewModel());
            }
        }

        /// <summary>
        /// Pobiera nowy fakt z API i zapisuje do bazy
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> FetchNewFact()
        {
            try
            {
                _logger.LogInformation("Pobieranie nowego faktu z API");
                var newFact = await _businessService.FetchAndSaveNewFactAsync();

                if (newFact != null)
                {
                    TempData["SuccessMessage"] = "Pomyślnie pobrano i zapisano nowy fakt o kocie!";
                    return Json(new { success = true, fact = newFact });
                }
                else
                {
                    TempData["ErrorMessage"] = "Nie udało się pobrać nowego faktu. Spróbuj ponownie.";
                    return Json(new { success = false, message = "Nie udało się pobrać faktu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania nowego faktu");
                return Json(new { success = false, message = "Wystąpił błąd podczas pobierania faktu" });
            }
        }

        /// <summary>
        /// Pobiera wiele faktów z API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> FetchMultipleFacts(int count = 5)
        {
            try
            {
                if (count < 1 || count > 10)
                {
                    return Json(new { success = false, message = "Liczba faktów musi być między 1 a 10" });
                }

                _logger.LogInformation("Pobieranie {Count} faktów z API", count);
                var newFacts = await _businessService.FetchAndSaveMultipleFactsAsync(count);

                TempData["SuccessMessage"] = $"Pomyślnie pobrano {newFacts.Count()} nowych faktów!";
                return Json(new { success = true, facts = newFacts, count = newFacts.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania wielu faktów");
                return Json(new { success = false, message = "Wystąpił błąd podczas pobierania faktów" });
            }
        }

        /// <summary>
        /// Przełącza status ulubionego dla faktu
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            try
            {
                var result = await _businessService.ToggleFavoriteAsync(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas przełączania statusu ulubionego dla faktu {Id}", id);
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Ocenia fakt
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RateFact(int id, int rating)
        {
            try
            {
                if (rating < 1 || rating > 5)
                {
                    return Json(new { success = false, message = "Ocena musi być między 1 a 5" });
                }

                var result = await _businessService.RateFactAsync(id, rating);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas oceniania faktu {Id}", id);
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Usuwa fakt
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteFact(int id)
        {
            try
            {
                var result = await _businessService.DeleteFactAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Fakt został pomyślnie usunięty.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Nie udało się usunąć faktu.";
                }
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania faktu {Id}", id);
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Strona z filtrowaniem faktów
        /// </summary>
        public async Task<IActionResult> Browse(FilterViewModel filter)
        {
            try
            {
                var facts = await _businessService.FilterFactsAsync(filter ?? new FilterViewModel());
                var categories = await _businessService.GetCategoriesAsync();

                ViewBag.Categories = categories;
                ViewBag.Filter = filter ?? new FilterViewModel();

                return View(facts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas filtrowania faktów");
                return View(Enumerable.Empty<CatFactViewModel>());
            }
        }

        /// <summary>
        /// Eksportuje fakty do pliku
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExportFacts(FilterViewModel? filter)
        {
            try
            {
                var filePath = await _businessService.ExportFactsToFileAsync(filter);
                var fileName = Path.GetFileName(filePath);
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Usuń tymczasowy plik
                System.IO.File.Delete(filePath);

                return File(fileBytes, "text/plain", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas eksportowania faktów");
                TempData["ErrorMessage"] = "Nie udało się wyeksportować faktów.";
                return RedirectToAction("Browse");
            }
        }

        /// <summary>
        /// Strona informacyjna
        /// </summary>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Polityka prywatności
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Strona błędu
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// Model dla strony błędu
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
