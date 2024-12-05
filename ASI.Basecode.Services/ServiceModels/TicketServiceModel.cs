using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketServiceModel
    {
        public int TicketId { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public int? AssignedTo { get; set; }
        public string Description { get; set; }
        public string AssignedToName { get; set; }
        public string PriorityType { get; set; }
        public string CategoryType { get; set; }
        public string StatusType { get; set; }
        public DateTime DateCreated { get; set; }
        public string AttachmentPath { get; set; }

        public int CategoryId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
    }
}
