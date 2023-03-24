using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext context;

        public ReviewerRepository(DataContext context)
        {
            this.context = context;
        }

        public bool Create(Reviewer reviewer)
        {
            this.context.Add(reviewer);
            return this.Save();
        }

        public Reviewer Get(int id)
        {
            return this.context.Reviewers.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Reviewer> GetAll()
        {
            return this.context.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return this.context.Reviews.Where(p => p.Reviewer.Id == reviewerId).ToList();
        }

        public bool Exists(int id)
        {
            return this.context.Reviewers.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = this.context.SaveChanges();
            return saved > 0;
        }

        public bool Update(Reviewer reviewer)
        {
            this.context.Update(reviewer);
            return this.Save();
        }

        public bool Delete(Reviewer arg)
        {
            this.context.Remove(arg);
            return this.Save();
        }
    }
}
