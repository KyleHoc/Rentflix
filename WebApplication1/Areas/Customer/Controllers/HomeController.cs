using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Rentflix.Models;
using Rentflix.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using System.Security.Cryptography;
using Rentflix.DataAccess.Data;
using System.Reflection.Metadata;

namespace Rentflix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
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
            Movie movie = _unitOfWork.Movie.Get(n => n.MovieId == id);
            return View(movie);
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
