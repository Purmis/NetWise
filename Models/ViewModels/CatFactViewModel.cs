using System.ComponentModel.DataAnnotations;

namespace CatFactsWebApp.Models.ViewModels
{
    /// <summary>
    /// ViewModel dla wyświetlania faktów o kotach w widoku
    /// </summary>
    public class CatFactViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Fakt o kocie")]
        public string Fact { get; set; } = string.Empty;
        
        [Display(Name = "Długość")]
        public int Length { get; set; }
        
        [Display(Name = "Data dodania")]
        public DateTime CreatedAt { get; set; }
        
        [Display(Name = "Ulubiony")]
        public bool IsFavorite { get; set; }
        
        [Display(Name = "Kategoria")]
        public string? Category { get; set; }
        
        [Display(Name = "Ocena")]
        [Range(1, 5, ErrorMessage = "Ocena musi być między 1 a 5")]
        public int Rating { get; set; }
        
        [Display(Name = "Czas od dodania")]
        public string TimeAgo => GetTimeAgo(CreatedAt);

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            
            if (timeSpan.TotalMinutes < 1)
                return "przed chwilą";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min temu";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} godz. temu";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} dni temu";
            
            return dateTime.ToString("dd.MM.yyyy");
        }
    }

    /// <summary>
    /// ViewModel dla strony głównej z listą faktów
    /// </summary>
    public class HomeViewModel
    {
        public List<CatFactViewModel> RecentFacts { get; set; } = new();
        public List<CatFactViewModel> FavoriteFacts { get; set; } = new();
        public int TotalFactsCount { get; set; }
        public double AverageRating { get; set; }
        public string? CurrentFact { get; set; }
        public bool IsLoading { get; set; }
    }

    /// <summary>
    /// ViewModel dla filtrowania faktów
    /// </summary>
    public class FilterViewModel
    {
        [Display(Name = "Szukaj w treści")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Kategoria")]
        public string? Category { get; set; }
        
        [Display(Name = "Tylko ulubione")]
        public bool OnlyFavorites { get; set; }
        
        [Display(Name = "Minimalna ocena")]
        [Range(1, 5)]
        public int? MinRating { get; set; }
        
        [Display(Name = "Data od")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }
        
        [Display(Name = "Data do")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }
    }
}
