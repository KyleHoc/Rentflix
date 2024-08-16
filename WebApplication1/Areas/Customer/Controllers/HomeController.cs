using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Rentflix.Models;
using Rentflix.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using System.Security.Cryptography;
using Rentflix.DataAccess.Data;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Rentflix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            IEnumerable<Movie> movies = _unitOfWork.Movie.GetAll().ToList();

            movies = movies.Shuffle();
            
            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Details(int id) 
        {
            //Get the movie using the provided ID
            Movie movie = _unitOfWork.Movie.Get(n => n.MovieId == id);

            ViewBag.isRented = false;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var searchRental = from r in _db.Rentals
                               where (r.MovieId == id) && (r.UserId == userId)
                               select r;

            foreach(var r in searchRental)
            {
                if (r.ReturnDate == null)
                {
                    ViewBag.isRented = true;
                }
            }

            //Return the details view, passing in the corresponding movie
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rent(int id)
        {
            //Create a new rental
            Rental newRental = new Rental();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Fill out rental information using provided user and movie id
            newRental.MovieId = id;
            newRental.UserId = userId;
            newRental.RentDate = DateTime.Now;
            newRental.ReturnDate = null;

            //If model state is valid, create rental
            if (ModelState.IsValid)
            {
                _db.Rentals.Add(newRental);
                _db.SaveChanges();
                TempData["success"] = "Movie has been rented";
            }

            //Redirect to the rentals page
            return RedirectToAction($"YourRentals", new {user = newRental.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(int id)
        {
            var rental = _db.Rentals.Find(id);

            if (rental == null || id == 0)
            {
                return NotFound();
            }

            rental.ReturnDate = DateTime.Now;

            //If model state is valid, create rental
            if (ModelState.IsValid)
            {
                _db.Rentals.Update(rental);
                _db.SaveChanges();
                TempData["success"] = "Movie has been returned";
            }

            //Refresh the page
            return RedirectToAction($"YourRentals", new { user = rental.UserId });
        }

        public IActionResult YourRentals(string user)
        {
            IQueryable<Rental> rentals = from r in _db.Rentals
                                         where r.UserId == user
                                         select r;

            rentals = rentals.Include(r => r.Movie);

            ViewBag.currentRentals = true;
            ViewBag.pastRentals = true;

            IQueryable<Rental> current = from r in rentals
                                         where r.ReturnDate == null
                                         select r;

            IQueryable<Rental> history = from r in rentals
                                         where r.ReturnDate != null
                                         select r;

            var result = current.Any();




           if (!result)
            {
               ViewBag.currentRentals = false;
            }


            if (!history.Any())
            {
                ViewBag.pastRentals = false;
            }

            return View(rentals);
        }

       


        public async Task<IActionResult> List(string? searchString)
        {
            IQueryable<Movie> movies = from m in _db.Movies
                         orderby m.Title
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
                return View(await movies.ToListAsync());
            }

            return View (movies);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }



    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (rng == null) throw new ArgumentNullException(nameof(rng));

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
