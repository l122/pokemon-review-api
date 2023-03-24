using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewerRepository
    {
        public bool Exists(int id);
        public ICollection<Reviewer> GetAll();
        public Reviewer Get(int id);
        public ICollection<Review> GetReviewsByReviewer(int reviewerId);
        public bool Create(Reviewer reviewer);
        public bool Save();
        public bool Update(Reviewer reviewer);
        public bool Delete(Reviewer arg);
    }
}
