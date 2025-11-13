using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_wchen60.Models
{
    public class Actor
    {
        public int Id { get; init; }

        [Required]
        [StringLength(1000, ErrorMessage = "Name cannot exceed 1000 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Gender cannot exceed 100 characters")]
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL (e.g., https://www.imdb.com/name/nm...")]
        [StringLength(2000, ErrorMessage = "URL cannot exceed 2000 characters")]
        public string ImdbLink { get; set; } = string.Empty;

        public byte[]? Photo { get; set; }

        public ICollection<ActorMovie>? ActorMovies { get; init; }
    }
}