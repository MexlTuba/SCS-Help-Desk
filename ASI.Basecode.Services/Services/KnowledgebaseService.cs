using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class KnowledgebaseService : IKnowledgebaseService
    {
        private readonly IKnowledgebaseRepository _repository;
        private readonly IMapper _mapper;
        private readonly SCSHelpDeskContext _context;

        public KnowledgebaseService(IKnowledgebaseRepository repository, IMapper mapper, SCSHelpDeskContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        // Repository-based CreateKnowledgebase method
        public void CreateKnowledgebase(Knowledgebase model, string userName)
        {
            model.CreatedBy = userName;
            model.CreatedAt = DateTime.UtcNow;
            _repository.Add(model);
        }


        // Direct-context GetAllArticles method with additional properties
        public List<KnowledgeBaseModel> GetAllArticles()
        {
            var articles = (from kb in _context.Knowledgebase
                            join category in _context.Categories on kb.CategoryId equals category.CategoryId
                            select new KnowledgeBaseModel
                            {
                                ArticleId = kb.ArticleId,
                                Title = kb.Title,
                                Content = kb.Content,
                                CategoryName = category.CategoryType,
                                CreatedBy = kb.CreatedBy,
                                CreatedAt = kb.CreatedAt,
                                UpdatedBy = kb.UpdatedBy,
                                UpdatedAt = kb.UpdatedAt
                            }).ToList();

            return articles;
        }

        // Direct-context GetArticleById method
        public KnowledgeBaseModel GetArticleById(int id)
        {
            var article = (from kb in _context.Knowledgebase
                           join category in _context.Categories on kb.CategoryId equals category.CategoryId
                           where kb.ArticleId == id
                           select new KnowledgeBaseModel
                           {
                               ArticleId = kb.ArticleId,
                               Title = kb.Title,
                               Content = kb.Content,
                               CategoryId = kb.CategoryId,
                               CategoryName = category.CategoryType,
                               CreatedBy = kb.CreatedBy,
                               CreatedAt = kb.CreatedAt,
                               UpdatedBy = kb.UpdatedBy,
                               UpdatedAt = kb.UpdatedAt
                           }).FirstOrDefault();

            return article;
        }

        public List<KnowledgeBaseModel> GetArticlesByCategory(int categoryId)
        {
            // Query the database or repository to get articles for the specified category
            return _context.Knowledgebase
                .Where(a => a.CategoryId == categoryId)
                .Join(_context.Categories,
                    kb => kb.CategoryId,
                    category => category.CategoryId,
                    (kb, category) => new KnowledgeBaseModel
                    {
                        ArticleId = kb.ArticleId,
                        Title = kb.Title,
                        Content = kb.Content,
                        CategoryName = category.CategoryType,
                        CreatedBy = kb.CreatedBy,
                        CreatedAt = kb.CreatedAt,
                        UpdatedBy = kb.UpdatedBy,
                        UpdatedAt = kb.UpdatedAt
                    })
                .ToList();
        }



        // Repository-based UpdateKnowledgebase method
        public void UpdateKnowledgebase(KnowledgeBaseViewModel model, string userName)
        {
            var knowledgebase = _repository.GetArticleById(model.ArticleId);
            if (knowledgebase == null)
            {
                throw new Exception("Knowledgebase article not found");
            }

            knowledgebase.Title = model.Title;
            knowledgebase.Content = model.Content;
            knowledgebase.CategoryId = model.CategoryId;
            knowledgebase.UpdatedBy = userName;
            knowledgebase.UpdatedAt = DateTime.Now;

            _repository.Update(knowledgebase);
        }

        // Repository-based DeleteKnowledgebase method
        public void DeleteKnowledgebase(int id)
        {
            var knowledgebase = _repository.GetArticleById(id);
            if (knowledgebase != null)
            {
                _repository.Delete(id);
            }
        }

        public IEnumerable<CategoryViewModel> GetCategories()
        {
            return _context.Categories
                .Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryType = c.CategoryType
                })
                .ToList();
        }
    }
}
