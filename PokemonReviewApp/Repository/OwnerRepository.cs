using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext context;

        public OwnerRepository(DataContext context)
        {
            this.context = context;
        }

        public bool Create(Owner arg)
        {
            this.context.Add(arg);
            return this.Save();
        }

        public Owner Get(int id)
        {
            return this.context.Owners.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Owner> GetAll()
        {
            return this.context.Owners.ToList();
        }

        public ICollection<Owner> GetOwnersOfAPokemon(int pokeId)
        {
            return this.context.PokemonOwners.Where(p => p.PokemonId == pokeId).Select(p => p.Owner).ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return this.context.PokemonOwners.Where(p => p.OwnerId == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool Exists(int id)
        {
            return this.context.Owners.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var result = this.context.SaveChanges();
            return result > 0;
        }

        public bool Update(Owner arg)
        {
            this.context.Update(arg);
            return this.Save();
        }

        public bool Delete(Owner arg)
        {
            this.context.Remove(arg);
            return this.Save();
        }
    }
}
