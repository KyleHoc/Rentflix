using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentflix.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IMovieRepository Movie { get; }

        void Save();
    }
}
