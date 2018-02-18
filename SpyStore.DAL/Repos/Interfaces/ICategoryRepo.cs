using SpyStore.DAL.Repos.Base;
using SpyStore.Models.Entities;
using System.Collections.Generic;

namespace SpyStore.DAL.Repos.Interfaces
{
    public interface ICategoryRepo : IRepo<Category>
    {
        IEnumerable<Category> GetAllWithProduct();
        Category GetOneWithProducts(int? id);
    }
}
