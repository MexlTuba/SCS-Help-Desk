using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{

    public class KnowledgebaseRepository : BaseRepository, IKnowledgebaseRepository
    {
        public KnowledgebaseRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Knowledgebase> GetArticles()
        {
            return this.GetDbSet<Knowledgebase>()
                       .Include(k => k.Category); // Eager loading the Category if needed
        }

        public Knowledgebase GetArticleById(int articleId)
        {
            return this.GetDbSet<Knowledgebase>()
                       .Include(k => k.Category) // Eager loading the Category if needed
                       .FirstOrDefault(k => k.ArticleId == articleId);
        }

        public void Add(Knowledgebase knowledgebase)
        {
            this.GetDbSet<Knowledgebase>().Add(knowledgebase);
            UnitOfWork.SaveChanges(); // Commit changes
        }

        public void Update(Knowledgebase knowledgebase)
        {
            this.GetDbSet<Knowledgebase>().Update(knowledgebase);
            UnitOfWork.SaveChanges(); // Commit changes
        }

        public void Delete(int articleId)
        {
            var knowledgebase = this.GetDbSet<Knowledgebase>().FirstOrDefault(k => k.ArticleId == articleId);
            if (knowledgebase != null)
            {
                this.GetDbSet<Knowledgebase>().Remove(knowledgebase);
                UnitOfWork.SaveChanges(); // Commit changes
            }
        }
    }

}
