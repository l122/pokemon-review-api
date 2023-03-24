using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext context;

        public CountryRepository(DataContext context)
        {
            this.context = context;
        }

        public bool Exists(int id)
        {
            return this.context.Countries.Any(p => p.Id == id);
        }

        public bool Create(Country arg)
        {
            this.context.Add(arg);
            return this.Save();
        }

        public ICollection<Country> GetAll()
        {
            return this.context.Countries.OrderBy(p => p.Id).ToList();
        }

        public Country Get(int id)
        {
            return this.context.Countries.Where(p => p.Id == id).FirstOrDefault();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return this.context.Owners.Where(p => p.Id == ownerId).Select(p => p.Country).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersFromACountry(int countryId)
        {
            return this.context.Owners.Where(p => p.Country.Id == countryId).ToList();
        }

        public bool Save()
        {
            var result = this.context.SaveChanges();
            return result > 0;
        }

        public bool Update(Country arg)
        {
            var result = this.context.Update(arg);
            return this.Save();
        }

        public bool Delete(Country arg)
        {
            this.context.Remove(arg);
            return this.Save();
        }
    }
}
