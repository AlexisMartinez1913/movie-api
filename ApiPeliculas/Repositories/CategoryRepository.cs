using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;

namespace ApiPeliculas.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _bd;
        public CategoryRepository(ApplicationDbContext bd)
        {
            _bd = bd;
        }
        //crear una categoria
        public bool CreateCategory(Category category)
        {
            category.DateCreation = DateTime.Now;   
            _bd.Category.Add(category);
            return Save();
        }
        //eliminar categoria
        public bool DeleteCategory(Category category)
        {
            _bd.Category.Remove(category);
            return Save();
        }
        //verificar si existe categoria
        public bool ExistsCategory(string categoryName)
        {
            bool value = _bd.Category.Any(c => c.Name.ToLower().Trim() == categoryName.ToLower().Trim());
            return value;
        }

        //verificar si existe por id

        public bool ExistsCategory(int id)
        {
            //any: cualquier coincidencia
            return _bd.Category.Any(c => c.Id == id);
        }

        public ICollection<Category> GetCategories()
        {
            return _bd.Category.OrderBy(c => c.Name).ToList();
        }
        //obtiene categoria por id
        public Category GetCategory(int categoryId)
        {
            return _bd.Category.FirstOrDefault(c => c.Id == categoryId);

        }

        public bool Save()
        {
            return _bd.SaveChanges() >= 0 ? true: false;
        }

        //actualiza

        public bool UpdateCategory(Category category)
        {
            category.DateCreation = DateTime.Now;
            _bd.Category.Update(category);
            return Save();
        }
    }
}
