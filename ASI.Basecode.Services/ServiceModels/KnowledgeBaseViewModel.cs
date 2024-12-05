using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class KnowledgeBaseViewModel
    {
        // Single Article Properties
        public int ArticleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public List<KnowledgeBaseModel> Articles { get; set; }
    }

    //public class KnowledgebaseCategoryViewModel
    //{
    //    public int CategoryId { get; set; }
    //    public string CategoryType { get; set; }
    //}

}

