using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Ticket
    {
        public Ticket()
        {

        }
        [Key]
        public int TicketId { get; set; }             // Primary Key

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }             // Title of the ticket

        [Required]
        public string Description { get; set; }       // Detailed description of the issue

        public string Attatchment { get; set; }       // file path of attatchment

        public int CategoryId { get; set; }           // Foreign Key to Category
        public int PriorityId { get; set; }           // Foreign Key to Priority
        public int StatusId { get; set; }             // Foreign Key to Status

        public int? AssignedTo { get; set; }          // Nullable Foreign Key to User (assigned support agent)
        public string CreatedBy { get; set; }        // Student ID number (userId) of the logged in user

        public DateTime DateCreated { get; set; } = DateTime.Now; // Creation date (default to current date)
        public DateTime? DateClosed { get; set; }     // Nullable close date (set only when ticket is closed)

        // Navigation properties for relationships
        public Category Category { get; set; }        // Navigation property for Category
        public Priority Priority { get; set; }        // Navigation property for Priority
        public Status Status { get; set; }            // Navigation property for Status
        public User AssignedUser { get; set; }        // Navigation property for assigned User (support agent)
    }
}
