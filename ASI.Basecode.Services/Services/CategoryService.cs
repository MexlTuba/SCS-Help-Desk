using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly SCSHelpDeskContext _context;
        private readonly IUserRepository userRepository;

        public CategoryService(SCSHelpDeskContext context)
        {
            _context = context;
        }

        public IEnumerable<CategoryViewModel> GetAllCategories()
        {
            return _context.Categories.Select(c => new CategoryViewModel
            {
                CategoryId = c.CategoryId,
                CategoryType = c.CategoryType
            }).ToList();
        }
    }
}
