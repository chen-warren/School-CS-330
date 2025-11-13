using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_wchen60.Models
{
    public class Movie
    {
        public int Id { get; init; }

        [Required]
        [StringLength(1000, ErrorMessage = "Name cannot exceed 1000 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Gender cannot exceed 1000 characters")]
        public string Genre { get; set; } = string.Empty;
        public int Year { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL (e.g., https://www.imdb.com/title/tt...")]
        [StringLength(2000, ErrorMessage = "URL cannot exceed 2000 characters")]
        public string ImdbLink { get; set; } = string.Empty;

        public byte[]? Poster { get; set; }

        public ICollection<ActorMovie>? ActorMovies { get; set; }
    }
}