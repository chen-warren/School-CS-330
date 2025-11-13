using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_wchen60.Data;
using Fall2025_Project3_wchen60.Models;
using Fall2025_Project3_wchen60.Models.ViewModels;
using Fall2025_Project3_wchen60.Services;

namespace Fall2025_Project3_wchen60.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAiService _aiService;

        public ActorsController(ApplicationDbContext context, OpenAiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // GET: /Actors/
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        // GET: /Actors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.ActorMovies)!
                .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
                return NotFound();

            var tweets = await _aiService.GetActorTweetsAsync(actor.Name);

            var vm = new ActorDetailsViewModel
            {
                Actor = actor,
                Movies = actor.ActorMovies?.Select(am => am.Movie).ToList() ?? [],
                Tweets = tweets
            };

            return View(vm);
        }

        // GET: /Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Gender,Age,ImdbLink")] Actor actor, IFormFile photoFile)
        {
            ModelState.Remove("Photo");
            ModelState.Remove("ActorMovies");

            if (string.IsNullOrWhiteSpace(actor.ImdbLink))
            {
                ModelState.Remove("ImdbLink");
            }

            if (ModelState.IsValid)
            {
                if (photoFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await photoFile.CopyToAsync(memoryStream);
                    actor.Photo = memoryStream.ToArray();
                }

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: /Actors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();
            return View(actor);
        }

        // POST: /Actors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,ImdbLink")] Actor actor, IFormFile? photoFile)
        {
            if (id != actor.Id)
                return NotFound();

            ModelState.Remove("Photo");
            ModelState.Remove("ActorMovies");

            if (string.IsNullOrWhiteSpace(actor.ImdbLink))
            {
                ModelState.Remove("ImdbLink");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingActor = await _context.Actors.FindAsync(id);
                    if (existingActor != null)
                    {
                        existingActor.Name = actor.Name;
                        existingActor.Gender = actor.Gender;
                        existingActor.Age = actor.Age;
                        existingActor.ImdbLink = actor.ImdbLink;

                        if (photoFile is { Length: > 0 })
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await photoFile.CopyToAsync(memoryStream);
                                existingActor.Photo = memoryStream.ToArray();
                            }
                        }

                        _context.Update(existingActor);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Actors.Any(e => e.Id == actor.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/GetPhoto/5
        public async Task<IActionResult> GetPhoto(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null || actor.Photo == null || actor.Photo.Length == 0)
            {
                return NotFound();
            }

            return File(actor.Photo, "image/jpeg");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Actors.Remove(actor);
            _context.SaveChanges();

            return RedirectToAction("Index"); 
        }
    }
}
