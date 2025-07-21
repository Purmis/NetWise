using System.ComponentModel.DataAnnotations;

namespace CatFactsWebApp.Models.Domain
{
    /// <summary>
    /// Model domenowy reprezentujący fakt o kocie
    /// </summary>
    public class CatFact
    {
        /// <summary>
        /// Unikalny identyfikator faktu
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Treść faktu o kocie
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Fact { get; set; } = string.Empty;

        /// <summary>
        /// Długość faktu w znakach
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Data i czas pobrania faktu
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Czy fakt został oznaczony jako ulubiony
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Kategoria faktu (jeśli dostępna)
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Ocena faktu (1-5 gwiazdek)
        /// </summary>
        [Range(0, 5)]
        public int Rating { get; set; }
    }
}
