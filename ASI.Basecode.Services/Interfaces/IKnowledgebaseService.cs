using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IKnowledgebaseService
    {
        List<KnowledgeBaseModel> GetAllArticles();
        KnowledgeBaseModel GetArticleById(int id);
        void CreateKnowledgebase(Knowledgebase model, string userName);
        void UpdateKnowledgebase(KnowledgeBaseViewModel model, string userName);
        void DeleteKnowledgebase(int id);
        List<KnowledgeBaseModel> GetArticlesByCategory(int categoryId);
    }
}
