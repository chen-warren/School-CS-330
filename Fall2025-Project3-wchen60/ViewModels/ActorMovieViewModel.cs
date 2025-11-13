using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fall2025_Project3_wchen60.Models.ViewModels
{
    public class ActorMovieViewModel
    {
        public ActorMovie? ActorMovie { get; init; }

        public IEnumerable<SelectListItem>? Actors { get; set; }

        public IEnumerable<SelectListItem>? Movies { get; set; }
    }
}