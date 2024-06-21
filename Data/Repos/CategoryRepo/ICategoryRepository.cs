using Application.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.CategoryRepo
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetCategoriesParent();
    }
}
