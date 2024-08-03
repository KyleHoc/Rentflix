using Rentflix.DataAccess.Data;
using Rentflix.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentflix.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IMovieRepository Movie { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Movie = new MovieRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
