using PokemonReviewApp.Models;
using System.Diagnostics.Eventing.Reader;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        public ICollection<Pokemon> GetAll();
        public Pokemon Get(int id);
        public Pokemon Get(string name);
        public decimal GetRating(int pokeId);
        public bool Exists(int pokeId);
        public bool Create(int ownerId, int categoryId, Pokemon pokemon);
        public bool Save();
        public bool Update(Pokemon arg);
        public bool Delete(Pokemon arg);
    }
}
