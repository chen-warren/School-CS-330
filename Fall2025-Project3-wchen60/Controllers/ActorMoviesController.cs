using Fall2025_Project3_wchen60.Models.ViewModels;  
using Fall2025_Project3_wchen60.Models;           
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_wchen60.Data;

namespace Fall2025_Project3_wchen60.Controllers
{
    public class ActorMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActorMovies
        public async Task<IActionResult> Index()
        {
            var relationships = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .OrderBy(am => am.Actor!.Name)
                .ToListAsync();

            return View(relationships);
        }

        // GET: ActorMovies/Create
        public IActionResult Create()
        {
            var actors = _context.Actors.OrderBy(a => a.Name).ToList();
            var movies = _context.Movies.OrderBy(m => m.Title).ToList();

            var actorList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "-- Select an Actor --" } };
            actorList.AddRange(actors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }));
            
            var movieList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "-- Select a Movie --" } };
            movieList.AddRange(movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }));
            
            var viewModel = new ActorMovieViewModel
            {
                ActorMovie = new ActorMovie(),
                Actors = actorList,
                Movies = movieList
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorMovieViewModel viewModel)
        {

            var actorMovie = viewModel.ActorMovie;

            ModelState.Remove("Actors");
            ModelState.Remove("Movies");
            ModelState.Remove("ActorMovie.Actor");
            ModelState.Remove("ActorMovie.Movie");

            if (actorMovie is { ActorId: 0 })
            {
                var actorIdValue = Request.Form["ActorMovie.ActorId"].FirstOrDefault();
                if (!string.IsNullOrEmpty(actorIdValue) && int.TryParse(actorIdValue, out int actorId))
                {
                    actorMovie.ActorId = actorId;
                }
            }

            if (actorMovie is { MovieId: 0 })
            {
                var movieIdValue = Request.Form["ActorMovie.MovieId"].FirstOrDefault();
                if (!string.IsNullOrEmpty(movieIdValue) && int.TryParse(movieIdValue, out int movieId))
                {
                    actorMovie.MovieId = movieId;
                }
            }

            if (actorMovie is { ActorId: 0 })
            {
                ModelState.AddModelError("ActorMovie.ActorId", "The Actor field is required.");
            }

            if (actorMovie is { MovieId: 0 })
            {
                ModelState.AddModelError("ActorMovie.MovieId", "The Movie field is required.");
            }

            if (actorMovie is { ActorId: > 0, MovieId: > 0 })
            {
                bool exists = await _context.ActorMovies
                    .AnyAsync(am => am.ActorId == actorMovie.ActorId && am.MovieId == actorMovie.MovieId);

                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "This actor is already associated with that movie.");
                }
            }

            if (!ModelState.IsValid)
            {
                var actors = _context.Actors.OrderBy(a => a.Name).ToList();
                var movies = _context.Movies.OrderBy(m => m.Title).ToList();
                
                var actorList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "-- Select an Actor --" } };
                actorList.AddRange(actors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, Selected = actorMovie != null && a.Id == actorMovie.ActorId }));
                
                var movieList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "-- Select a Movie --" } };
                movieList.AddRange(movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title, Selected = actorMovie != null && m.Id == actorMovie.MovieId }));
                
                viewModel.Actors = actorList;
                viewModel.Movies = movieList;

                return View(viewModel);
            }

            try
            {
                _context.Add(actorMovie);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Actor-Movie relationship created successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
                
                var actors = _context.Actors.OrderBy(a => a.Name).ToList();
                var movies = _context.Movies.OrderBy(m => m.Title).ToList();
                
                var actorList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "-- Select an Actor --" } };
                actorList.AddRange(actors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name, Selected = actorMovie != null && a.Id == actorMovie.ActorId }));
                
                var movieList = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "-- Select a Movie --" } };
                movieList.AddRange(movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title, Selected = actorMovie != null && m.Id == actorMovie.MovieId }));
                
                viewModel.Actors = actorList;
                viewModel.Movies = movieList;

                return View(viewModel);
            }
        }

        // GET: ActorMovies/Delete/5
        public async Task<IActionResult> Delete(int? actorId, int? movieId)
        {
            if (actorId == null || movieId == null)
                return NotFound();

            var actorMovie = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .FirstOrDefaultAsync(am => am.ActorId == actorId && am.MovieId == movieId);

            if (actorMovie == null)
                return NotFound();

            return View(actorMovie);
        }

        // POST: ActorMovies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int actorId, int movieId)
        {
            var actorMovie = await _context.ActorMovies
                .FirstOrDefaultAsync(am => am.ActorId == actorId && am.MovieId == movieId);
            if (actorMovie != null)
            {
                _context.ActorMovies.Remove(actorMovie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
