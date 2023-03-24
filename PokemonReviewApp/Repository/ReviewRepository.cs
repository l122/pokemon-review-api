using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext context;

        public ReviewRepository(DataContext context)
        {
            this.context = context;
        }

        public bool Create(Review review)
        {
            this.context.Add(review);
            return this.Save();
        }

        public Review Get(int id)
        {
            return this.context.Reviews.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Review> GetAll()
        {
            return this.context.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokeId)
        {
            return this.context.Reviews.Where(p => p.Pokemon.Id == pokeId).ToList();
        }

        public bool Exists(int id)
        {
            return this.context.Reviews.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = this.context.SaveChanges();
            return saved > 0;
        }

        public bool Update(Review review)
        {
            this.context.Update(review);
            return this.Save();
        }

        public bool Delete(Review arg)
        {
            this.context.Remove(arg);
            return this.Save();
        }
    }
}
