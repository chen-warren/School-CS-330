using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_wchen60.Data;
using Fall2025_Project3_wchen60.Models;
using Fall2025_Project3_wchen60.Models.ViewModels;
using Fall2025_Project3_wchen60.Services;

namespace Fall2025_Project3_wchen60.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAiService _aiService;

        public MoviesController(ApplicationDbContext context, OpenAiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync(); // Get the list of movies
            return View(movies); // Make sure a view exists for Index
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.ActorMovies)!
                .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            // Get reviews from OpenAI (10 reviews in one API call)
            var reviews = await _aiService.GetMovieReviewsAsync(movie.Title, movie.Year, "Unknown");
            
            var vm = new MovieDetailsViewModel
            {
                Movie = movie,
                Actors = movie.ActorMovies?.Select(am => am.Actor).ToList() ?? [],
                Reviews = reviews
            };

            return View(vm);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Year,Genre,ImdbLink")] Movie movie, IFormFile? posterFile)
        {
            ModelState.Remove("ActorMovies");

            if (!ModelState.IsValid) return View(movie);
            if (posterFile is { Length: > 0 })
            {
                using var memoryStream = new MemoryStream();
                await posterFile.CopyToAsync(memoryStream);
                movie.Poster = memoryStream.ToArray();
            }

            _context.Add(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction("Index"); 
        }


        // GET: /Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: /Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Genre,ImdbLink")] Movie movie, IFormFile? posterFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Poster");
            ModelState.Remove("ActorMovies");

            if (string.IsNullOrWhiteSpace(movie.ImdbLink))
            {
                ModelState.Remove("ImdbLink");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = await _context.Movies.FindAsync(id);
                    if (existingMovie != null)
                    {
                        existingMovie.Title = movie.Title;
                        existingMovie.Year = movie.Year;
                        existingMovie.Genre = movie.Genre;
                        existingMovie.ImdbLink = movie.ImdbLink;

                        if (posterFile is { Length: > 0 })
                        {
                            using var memoryStream = new MemoryStream();
                            await posterFile.CopyToAsync(memoryStream);
                            existingMovie.Poster = memoryStream.ToArray();
                        }

                        _context.Update(existingMovie);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movies.Any(e => e.Id == movie.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index)); 
            }
            return View(movie);
        }

        // GET: Movies/GetPoster/5
        public async Task<IActionResult> GetPoster(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie?.Poster == null || movie.Poster.Length == 0)
            {
                return NotFound();
            }

            return File(movie.Poster, "image/jpeg");
        }
    }

}
