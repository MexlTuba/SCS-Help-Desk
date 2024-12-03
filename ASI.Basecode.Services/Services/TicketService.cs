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
                           join user in _context.Users on ticket.AssignedTo.ToString() equals user.UserId into userJoin
                           from assignedUser in userJoin.DefaultIfEmpty() 
                           select new TicketServiceModel
                           {
                               TicketId = ticket.TicketId,
                               CreatedBy = ticket.CreatedBy,
                               Title = ticket.Title,
                               AssignedTo = ticket.AssignedTo, 
                               AssignedToName = assignedUser != null ? assignedUser.Name : "Unassigned", 
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

        public int GetTicketCount()
        {
            return _context.Ticket.Count();
        }

        public int GetTicketCountByStatus(string statusType)
        {
            return _context.Ticket
                           .Include(t => t.Status)
                           .Count(t => t.Status.StatusType == statusType);
        }


        public void UpdateStatus(Ticket ticket)
        {
            _context.Ticket.Update(ticket);
            _context.SaveChanges();
        }

    }
}