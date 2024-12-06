using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class DetailsTicketViewModel
    {
        // Ticket properties
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByName { get; set; }
        public string AttachmentPath { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public string AssignedToName { get; set; }
        public bool HasFeedback { get; set; }

        // Feedback properties
        public int? TicketRating { get; set; }
        public string TicketComment { get; set; }
        public int? AgentRating { get; set; }
        public string AgentComment { get; set; }
        public string UserId { get; set; }

        // Dropdown properties
        public List<CategoryViewModel> Categories { get; set; }
        public List<PriorityViewModel> Priorities { get; set; }
        public List<StatusViewModel> Statuses { get; set; }
    }
}
