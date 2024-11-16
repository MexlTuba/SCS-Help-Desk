using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services
{
    public class StatusService : IStatusService
    {
        private readonly SCSHelpDeskContext _context;
        private readonly IUserRepository userRepository;

        public StatusService(SCSHelpDeskContext context)
        {
            _context = context;
        }

        public IEnumerable<StatusViewModel> GetAllStatuses()
        {
            return _context.Statuses.Select(c => new StatusViewModel
            {
                StatusId = c.StatusId,
                StatusType = c.StatusType
            }).ToList();
        }
    }
}
