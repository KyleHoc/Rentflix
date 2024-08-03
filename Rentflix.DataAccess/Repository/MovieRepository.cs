using Microsoft.EntityFrameworkCore;
using Rentflix.DataAccess.Data;
using Rentflix.DataAccess.Repository.IRepository;
using Rentflix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rentflix.DataAccess.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _db;

        public MovieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Movie movie)
        {
            var objFromDb = _db.Movies.FirstOrDefault(u => u.MovieId == movie.MovieId);
            if (objFromDb != null)
            {
                objFromDb.Title = movie.Title;
                objFromDb.Director = movie.Director;
                objFromDb.Synopsis = movie.Synopsis;
                objFromDb.Genre = movie.Genre;
                objFromDb.ReleaseYear = movie.ReleaseYear;

                if (objFromDb.ImageUrl != null) 
                {
                    objFromDb.ImageUrl = movie.ImageUrl;
                }
            }
        }
    }
}
