using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewRepository
    {
        public bool Exists(int id);
        public Review Get(int id);
        public ICollection<Review> GetAll();
        public ICollection<Review> GetReviewsOfAPokemon(int pokeId);
        public bool Create(Review review);
        public bool Save();
        public bool Update(Review review);
        public bool Delete(Review arg);
    }
}
