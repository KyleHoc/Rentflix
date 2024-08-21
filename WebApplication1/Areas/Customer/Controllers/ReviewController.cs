using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rentflix.DataAccess.Data;
using Rentflix.Models;
using System.Security.Claims;

namespace Rentflix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: ReviewController/Upsert
        //public IActionResult Upsert(int? id)
        //{
        //    Review review = new Review();

        //    if (id == null || id == 0)
        //    {
        //        return View(review);
        //    }
        //    else
        //    {
        //        review = _db.Reviews.Find(id);

        //        if (review == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(review);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id)
        {
            var review = new Review();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            review.MovieId = id;
            review.UserId = userId;
            review.Text = Request.Form["text"];
            review.ReviewDate = DateTime.Now;
            review.LastEdited = null;

            if (review.Text == null)
            {
                ModelState.AddModelError("text", "Please write a review.");
            }

            if (ModelState.IsValid)
            {
                _db.Reviews.Add(review);
                _db.SaveChanges();
                TempData["success"] = "Review successfully submitted";
            }

            return RedirectToAction("Details", "Home", new { id = review.MovieId });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id)
        {
            IQueryable<Review> reviewQuery = from r in _db.Reviews
                                             where r.Id == id
                                             select r;

            reviewQuery = reviewQuery.Include(r => r.Movie).Include(r => r.User);

            Review review = new Review();

            foreach (Review r in reviewQuery)
            {
                review = r;
            }

            review.Text = Request.Form["edit-text"];

            if (review.Text == null)
            {
                ModelState.AddModelError("text", "Please write a review.");
            }

            if (ModelState.IsValid)
            {
                _db.Reviews.Update(review);
                _db.SaveChanges();
                TempData["success"] = "Review successfully updated";
            }

            return RedirectToAction("Details", "Home", new { id = review.MovieId });
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }

            IQueryable<Review> reviewQuery = from r in _db.Reviews
                                        where r.Id == id
                                        select r;

            reviewQuery = reviewQuery.Include(r => r.User).Include(r => r.Movie);

            Review review = new Review();

            foreach (Review r in reviewQuery)
            {
                review = r;
            }


            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // GET: ReviewController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePOST(int? id)
        {
            var review = _db.Reviews.Find(id);

            if (review == null) {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Reviews.Remove(review);
                _db.SaveChanges();
                TempData["success"] = "Review successfully deleted";
            }

            return RedirectToAction("Details", "Home", new { id = review.MovieId });;
        }
    }
}
