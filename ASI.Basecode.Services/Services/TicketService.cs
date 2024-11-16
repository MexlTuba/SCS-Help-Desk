using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services
{
    public class TicketService : ITicketService
    {
        private readonly SCSHelpDeskContext _context;

        public TicketService(SCSHelpDeskContext context)
        {
            _context = context;
        }

        public List<TicketServiceModel> GetAllTickets()
        {
            var tickets = (from ticket in _context.Ticket
                           join priority in _context.Priorities on ticket.PriorityId equals priority.PriorityId
                           join category in _context.Categories on ticket.CategoryId equals category.CategoryId
                           join status in _context.Statuses on ticket.StatusId equals status.StatusId
                           select new TicketServiceModel
                           {
                               TicketId = ticket.TicketId,
                               CreatedBy = ticket.CreatedBy,
                               Title = ticket.Title,
                               AssignedTo = ticket.AssignedTo,
                               PriorityType = priority.PriorityType,
                               CategoryType = category.CategoryType,
                               StatusType = status.StatusType,
                               DateCreated = ticket.DateCreated
                           }).ToList();

            return tickets;
        }

        public Ticket GetTicketById(int id)
        {
            return _context.Ticket.FirstOrDefault(t => t.TicketId == id);
        }
    }
}