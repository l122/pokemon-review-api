using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface ICountryRepository
    {
        public ICollection<Country> GetAll();
        public Country Get(int id);
        public Country GetCountryByOwner(int ownerId);
        public ICollection<Owner> GetOwnersFromACountry(int countryId);
        public bool Exists(int id);
        public bool Create(Country arg);
        public bool Save();
        public bool Update(Country arg);
        public bool Delete(Country arg);
    }
}
