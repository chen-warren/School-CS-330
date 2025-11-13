using System.ComponentModel.DataAnnotations;

namespace Fall2025_Project3_wchen60.Models
{
    public class ActorMovie
    {
        [Required]
        public int ActorId { get; set; }
        [Required]
        public Actor? Actor { get; init; }

        [Required]
        public int MovieId { get; set; }
        [Required]
        public Movie? Movie { get; init; }
    }
}