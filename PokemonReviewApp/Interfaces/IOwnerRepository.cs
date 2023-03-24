using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IOwnerRepository
    {
        public bool Exists(int id);
        public ICollection<Owner> GetAll();
        public Owner Get(int id);
        public ICollection<Owner> GetOwnersOfAPokemon(int pokeId);
        public ICollection<Pokemon> GetPokemonByOwner(int ownerId);
        public bool Create(Owner arg);
        public bool Save();
        public bool Update(Owner arg);
        public bool Delete(Owner arg);
    }
}
