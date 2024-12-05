using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public interface IKnowledgebaseRepository
    {
        IQueryable<Knowledgebase> GetArticles();
        Knowledgebase GetArticleById(int id);
        void Add(Knowledgebase knowledgebase);
        void Update(Knowledgebase knowledgebase);
        void Delete(int id);
    }
}
