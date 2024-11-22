using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class TicketViewModel
    {
        public int TicketId { get; set; }

        [Required(ErrorMessage = "Ticket subject is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }             // Title of the ticket

        [Required(ErrorMessage = "Ticket description is required.")]
        public string Description { get; set; }       // Detailed description of the issue

        public string Attatchment { get; set; }       //file path of attatchment db will receive

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }           // Selected Category ID

        public int PriorityId { get; set; } = 4;      // Priority ID, default to "General" (assuming General has an ID of 4)

        public int StatusId { get; set; } = 1;        // Status ID, default to "Open" (assuming Open has an ID of 1)

        public int? AssignedTo { get; set; }          // Assigned User ID (nullable)
        public string CreatedBy { get; set; }            // Creator User ID

        public DateTime DateCreated { get; set; } = DateTime.Now; // Creation date (default to current date)

        public int TotalTickets { get; set; }
        public int PendingTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int DeletedTickets { get; set; }

        public List<TicketServiceModel> Tickets { get; set; }

        public DateTime? DateClosed { get; set; }

        // These properties are for dropdown lists in the form view
        public IEnumerable<CategoryViewModel> Categories { get; set; } // List of available categories
        public IEnumerable<PriorityViewModel> Priorities { get; set; } // List of available priorities
        public IEnumerable<StatusViewModel> Statuses { get; set; }     // List of available statuses

        // Optional file upload for attachments (assuming multiple files)
        public List<FileUpload> Attachments { get; set; } = new List<FileUpload>(); // List of uploaded files
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryType { get; set; }
    }

    public class PriorityViewModel
    {
        public int PriorityId { get; set; }
        public string PriorityType { get; set; }
    }

    public class StatusViewModel
    {
        public int StatusId { get; set; }
        public string StatusType { get; set; }
    }

    // Class for handling file uploads (optional)
    public class FileUpload
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public byte[] Content { get; set; }
    }
}
