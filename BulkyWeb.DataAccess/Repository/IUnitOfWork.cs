using BulkyWeb.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public interface IUnitOfWork
    {
        // List out all interface/classes here
        ICategoryRepository Category { get; }

        IProductRepository Product { get; }

        ICompanyRepository Company { get; }
        void Save();

    }
}
