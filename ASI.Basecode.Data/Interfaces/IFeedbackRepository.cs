using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IFeedbackRepository
    {
        IQueryable<Feedback> GetFeedbacks();
        bool FeedbackExists(int FeedbackId);
        void AddFeedback(Feedback feedback);
        Feedback GetFeedbackByTicketId(int ticketId);
    }
}
