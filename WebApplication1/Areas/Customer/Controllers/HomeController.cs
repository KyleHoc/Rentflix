using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Rentflix.Models;
using Rentflix.DataAccess.Repository.IRepository;

namespace Rentflix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Movie> movies = _unitOfWork.Movie.GetAll().ToList();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
