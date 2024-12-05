using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class FeedbackRepository : BaseRepository, IFeedbackRepository
    {
        public FeedbackRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Feedback GetFeedbackByTicketId(int ticketId)
        {
            return this.GetDbSet<Feedback>().FirstOrDefault(f => f.TicketId == ticketId);
        }


        public IQueryable<Feedback> GetFeedbacks()
        {
            return this.GetDbSet<Feedback>();
        }

        public bool FeedbackExists(int FeedbackId)
        {
            return this.GetDbSet<Feedback>().Any(x => x.FeedbackId == FeedbackId);
        }

        public void AddFeedback(Feedback feedback)
        {
            this.GetDbSet<Feedback>().Add(feedback);
            UnitOfWork.SaveChanges();
        }
    }
}
