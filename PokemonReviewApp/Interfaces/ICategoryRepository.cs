using PokemonReviewApp.Dto;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICategoryRepository
    {
        public ICollection<Category> GetAll();
        public Category Get(int id);
        public ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        public bool Exists(int categoryId);
        public bool Create(Category category);
        public bool Save();
        public bool Update(Category category);
        public bool Delete(Category category);
    }
}
