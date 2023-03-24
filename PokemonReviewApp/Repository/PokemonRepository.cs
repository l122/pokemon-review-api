using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext context;

        public PokemonRepository(DataContext context)
        {
            this.context = context;
        }

        public bool Create(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = this.context.Owners.Where(p => p.Id == ownerId).FirstOrDefault();
            var category = this.context.Categories.Where(p => p.Id == categoryId).FirstOrDefault();
            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
            };

            this.context.Add(pokemonCategory);
            this.context.Add(pokemon);

            return this.Save();
        }

        public Pokemon Get(int id)
        {
            return this.context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon Get(string name)
        {
            return this.context.Pokemon.Where(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public decimal GetRating(int pokeId)
        {
            var review = this.context.Reviews.Where(p => p.Pokemon.Id == pokeId);

            if (!review.Any())
            {
                return 0;
            }

            return (decimal)review.Sum(r => r.Rating) / review.Count();
        }

        public ICollection<Pokemon> GetAll()
        {
            return this.context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public bool Exists(int pokeId)
        {
            return this.context.Pokemon.Any(p => p.Id == pokeId);
        }

        public bool Save()
        {
            var saved = this.context.SaveChanges();
            return saved > 0;
        }

        public bool Update(Pokemon arg)
        {
            this.context.Update(arg);
            return this.Save();
        }

        public bool Delete(Pokemon arg)
        {
            this.context.Remove(arg);
            return this.Save();
        }
    }
}
