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
    public class FeedbackService: IFeedbackService
    {
        private readonly IFeedbackRepository _repository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly SCSHelpDeskContext _context;

        public FeedbackService(IFeedbackRepository repository, IUserService userService, IMapper mapper, SCSHelpDeskContext context)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
            _context = context;
        }

        public List<Feedback> GetFeedbacks()
        {
            return _repository.GetFeedbacks().ToList();
        }

        public Feedback GetFeedbackById(int id)
        {
            return _context.Feedback.FirstOrDefault(f => f.FeedbackId == id);
        }

        public Feedback GetFeedbackByTicketId(int ticketId)
        {
            return _context.Feedback.FirstOrDefault(f => f.TicketId == ticketId);
        }


        public void AddFeedback(FeedbackViewModel model, string userName)
        {
            var user = _userService.GetUserByName(userName);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Ensure TicketId is valid and maps correctly
            var ticket = _context.Ticket.FirstOrDefault(t => t.TicketId == model.TicketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found for the provided TicketId");
            }

            var feedback = new Feedback
            {
                TicketRating = model.TicketRating,
                TicketComment = model.TicketComment,
                AgentRating = model.AgentRating,
                AgentComment = model.AgentComment,
                UserId = user.UserId,
                TicketId = ticket.TicketId, // Map valid TicketId
                DateCreated = DateTime.Now
            };

            _repository.AddFeedback(feedback);
        }
    }
}
