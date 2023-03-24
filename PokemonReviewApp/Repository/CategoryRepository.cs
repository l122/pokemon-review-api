using Microsoft.EntityFrameworkCore.Diagnostics;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext context;

        public CategoryRepository(DataContext dataContext)
        {
            this.context = dataContext;
        }
        public bool Exists(int categoryId)
        {
            return context.Categories.Any(p => p.Id == categoryId);
        }

        public bool Create(Category category)
        {
            this.context.Add(category);
            return this.Save();
        }


        public ICollection<Category> GetAll()
        {
            return this.context.Categories.OrderBy(p => p.Id).ToList();
        }

        public Category Get(int id)
        {
            return this.context.Categories.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
        {
            return this.context.PokemonCategories.Where(p => p.CategoryId == categoryId).Select(p => p.Pokemon).ToList();
        }
        public bool Save()
        {
            var result = this.context.SaveChanges();
            return result > 0;
        }

        public bool Update(Category category)
        {
            this.context.Update(category);
            return this.Save();
        }

        public bool Delete(Category category)
        {
            this.context.Remove(category);
            return this.Save();
        }
    }
}
