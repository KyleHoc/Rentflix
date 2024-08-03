using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rentflix.DataAccess.Data;
using Rentflix.DataAccess.Repository;
using Rentflix.DataAccess.Repository.IRepository;
using Rentflix.Models;

namespace Rentflix.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: MovieController
        public IActionResult Index()
        {
            IEnumerable<Movie> objMovieList = _unitOfWork.Movie.GetAll().ToList();

            return View(objMovieList);
        }

        // GET: MovieController/Upsert
        public IActionResult Upsert(int? id)
        { 
            Movie movie = new Movie();

            if (id == null || id == 0)
            {
                return View(movie);
            } else
            {
                movie = _unitOfWork.Movie.Get(i => i.MovieId == id);

                if (movie == null)
                {
                    return NotFound();
                }

                return View(movie);
            }
        }

        // POST: MovieController/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Movie obj, IFormFile? file)
        {
            if (ModelState.IsValid) 
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string moviePath = Path.Combine(wwwRootPath, @"images\movie\");

                    if (file != null && !string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(moviePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    obj.ImageUrl = @"\images\movie\" + fileName;
                }

                if (obj.MovieId == 0) 
                {
                    _unitOfWork.Movie.Add(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Movie created successfully";
                    return RedirectToAction("Index");
                } else
                {
                    _unitOfWork.Movie.Update(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Movie edited successfully";
                    return RedirectToAction("Index");
                }
            }
          
            return View(obj);
        }

        // GET: MovieController/Delete/5
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }

        //    Movie? movie = _unitOfWork.Movie.Get(i => i.MovieId == id);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(movie);
        //}

        // POST: MovieController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            Movie? movie = _unitOfWork.Movie.Get(i => i.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Movie.Remove(movie);
                _unitOfWork.Save();
                TempData["success"] = "Movie deleted successfully";
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        #region APICALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Movie> objMovieList = _unitOfWork.Movie.GetAll().ToList();
            return Json(new { data = objMovieList });
        }

        [HttpDelete]
        public IActionResult Delete(int ? id)
        {
            Movie? movie = _unitOfWork.Movie.Get(i => i.MovieId == id);

            if (movie == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }

            //delete old image
            if (movie.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Movie.Remove(movie);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful"});
        }


        #endregion
    }

}
