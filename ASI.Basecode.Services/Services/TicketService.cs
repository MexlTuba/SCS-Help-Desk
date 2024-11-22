using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly SCSHelpDeskContext _context;

        public TicketService(ITicketRepository repository, IUserService userService, IMapper mapper, SCSHelpDeskContext context)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
            _context = context;
        }

        // Repository-based AddTicket method
        public void AddTicket(TicketViewModel model, string userName)
        {
            // Retrieve user details based on userName
            var user = _userService.GetUserByName(userName);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Create and populate the Ticket entity
            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Attatchment = model.Attatchment,
                PriorityId = 4,      // Default to "General" priority
                StatusId = 1,        // Default to "Open" status
                CreatedBy = user.UserId, // User ID of the logged-in user
                DateCreated = DateTime.Now
            };

            // Save the ticket to the repository
            _repository.AddTicket(ticket);
        }

        // Direct-context GetAllTickets method
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

        // Repository-based GetTickets method
        public List<Ticket> GetTickets()
        {
            return _repository.GetTickets().ToList();
        }

        // Direct-context GetTicketById method

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


        public void UpdateTicket(Ticket ticket)
        {
            _context.Ticket.Update(ticket);
            _context.SaveChanges();
        }


        // Empty DeleteTicket method
        public void DeleteTicket(int ticketId)
        {
            var ticket = _repository.GetTickets().FirstOrDefault(t => t.TicketId == ticketId);

            
            if (ticket != null)
            {
                _repository.DeleteTicket(ticket); 
            }
        }

    }
}
