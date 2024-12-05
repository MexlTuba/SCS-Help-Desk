using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Data.Models
{
    public class Feedback
    {
        public Feedback()
        {
        }

        [Key]
        public int FeedbackId { get; set; }          // Primary Key of the Feedback table

        [Required]
        public string UserId { get; set; }           // Student ID number (foreign key to User)

        [Required]
        public int TicketId { get; set; }            // Foreign key to Ticket table

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now; // Date the feedback was created/submitted

        [Range(1, 5)]                                // Rating should be between 1 and 5
        public int TicketRating { get; set; }        // Rating of how the ticket was handled

        [Range(1, 5)]                                // Rating should be between 1 and 5
        public int AgentRating { get; set; }         // Rating of the support agent

        [MaxLength(500)]                             // Optional max length for comments
        public string TicketComment { get; set; }    // Comment on how the ticket was handled

        [MaxLength(500)]                             // Optional max length for comments
        public string AgentComment { get; set; }     // Comment about the support agent

        // Navigation properties for relationships
        public Ticket Ticket { get; set; }           // Navigation property for Ticket
        public User User { get; set; }               // Navigation property for User
    }
}
