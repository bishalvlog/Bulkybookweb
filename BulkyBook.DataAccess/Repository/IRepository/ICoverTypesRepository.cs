using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public interface ICoverTypesRepository :IRepository<CoverType>
    {
        void Update (CoverType coverType);
        void Save();

    }
}
