using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services
{
    public class PriorityService : IPriorityService
    {
        private readonly SCSHelpDeskContext _context;
        private readonly IUserRepository userRepository;

        public PriorityService(SCSHelpDeskContext context)
        {
            _context = context;
        }

        public IEnumerable<PriorityViewModel> GetAllPriorities()
        {
            return _context.Priorities.Select(c => new PriorityViewModel
            {
                PriorityId = c.PriorityId,
                PriorityType = c.PriorityType
            }).ToList();
        }
    }
}
