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
                AttachmentPath = model.AttachmentPath,
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
                           join userAssigned in _context.Users on ticket.AssignedTo.ToString() equals userAssigned.UserId into userJoin
                           from assignedUser in userJoin.DefaultIfEmpty()
                           join createdByUser in _context.Users on ticket.CreatedBy equals createdByUser.UserId into createdByJoin
                           from creator in createdByJoin.DefaultIfEmpty()
                           where ticket.StatusId != 5 // Exclude Deleted tickets
                           select new TicketServiceModel
                           {
                               TicketId = ticket.TicketId,
                               CreatedBy = creator != null ? creator.Name : "Unknown", // Get name of the user who created the ticket
                               Title = ticket.Title,
                               AssignedTo = ticket.AssignedTo,
                               AssignedToName = assignedUser != null ? assignedUser.Name : "Unassigned",
                               PriorityType = priority.PriorityType,
                               CategoryType = category.CategoryType,
                               StatusType = status.StatusType,
                               DateCreated = ticket.DateCreated,

                               CategoryId = ticket.CategoryId,
                               StatusId = ticket.StatusId,
                               PriorityId = ticket.PriorityId,
                               AttachmentPath = ticket.AttachmentPath
                           }).ToList();

            return tickets;
        }


        public List<TicketServiceModel> GetFilteredTickets(int? categoryId, int? statusId, int? priorityId)
        {
            var tickets = (from ticket in _context.Ticket
                           join priority in _context.Priorities on ticket.PriorityId equals priority.PriorityId
                           join category in _context.Categories on ticket.CategoryId equals category.CategoryId
                           join status in _context.Statuses on ticket.StatusId equals status.StatusId
                           where ticket.CategoryId == categoryId &&
                                 ticket.StatusId == statusId &&
                                 ticket.PriorityId == priorityId
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
            // Get the StatusId for "Deleted"
            var DeletedStatus = _context.Statuses
                                          .FirstOrDefault(s => s.StatusId == 5);

            if (DeletedStatus == null)
            {
                return _context.Ticket.Count();  // If no "Deleted" status found, count all tickets
            }

            var DeletedStatusId = DeletedStatus.StatusId;

            // Count tickets excluding the "Deleted" status
            return _context.Ticket
                           .Where(t => t.StatusId != DeletedStatusId)
                           .Count();
        }

        public int GetTicketCountByStatus(string statusType)
        {
            return _context.Ticket
                           .Include(t => t.Status)
                           .Where(t => t.Status.StatusType == statusType && t.Status.StatusType != "Deleted")
                           .Count(t => t.Status.StatusType == statusType);
        }

        public int GetTicketCountByCategory(string categoryType)
        {
            var DeletedStatus = _context.Statuses
                                          .FirstOrDefault(s => s.StatusId == 5);
            if (DeletedStatus == null)
            {
                return _context.Ticket
                           .Include(t => t.Category)
                           .Count(t => t.Category.CategoryType == categoryType);
            }
            var DeletedStatusId = DeletedStatus.StatusId;

            // Count tickets by priority excluding "Deleted" status
            return _context.Ticket
                           .Include(t => t.Category)
                           .Where(t => t.Category.CategoryType == categoryType && t.StatusId != DeletedStatusId)
                           .Count();

        }

        public int GetTicketCountByPriority(string priorityType)
        {
            // Fetch the StatusId for "Deleted"
            var DeletedStatus = _context.Statuses
                                          .FirstOrDefault(s => s.StatusId == 5);

            if (DeletedStatus == null)
            {
                // If no "Deleted" status is found, count tickets by priority
                return _context.Ticket
                               .Include(t => t.Priority)
                               .Count(t => t.Priority.PriorityType == priorityType);
            }

            var DeletedStatusId = DeletedStatus.StatusId;

            // Count tickets by priority excluding "Deleted" status
            return _context.Ticket
                           .Include(t => t.Priority)
                           .Where(t => t.Priority.PriorityType == priorityType && t.StatusId != DeletedStatusId)
                           .Count();
        }
        public int GetDeletedTicketCount()
        {
            return _context.Ticket.Count(t => t.StatusId == 5);
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
                ticket.StatusId = 5; 
                _context.Ticket.Update(ticket);
                _context.SaveChanges();

                // Optionally, hard delete after marking Deleted
                //_repository.DeleteTicket(ticket);
            }
        }


        public void EditDetailsTicket(Ticket ticket)
        {
            var existingTicket = _repository.GetTickets().FirstOrDefault(t => t.TicketId == ticket.TicketId);

            if (existingTicket != null)
            {
                existingTicket.Title = ticket.Title;
                existingTicket.Description = ticket.Description;

                // Save changes
                _repository.UpdateTicket(existingTicket);
            }
            else
            {
                throw new Exception("Ticket not found");
            }
        }



    }
}
