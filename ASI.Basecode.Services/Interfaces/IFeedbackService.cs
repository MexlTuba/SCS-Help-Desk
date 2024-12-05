using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IFeedbackService
    {
        
        List<Feedback> GetFeedbacks();

        Feedback GetFeedbackById(int id);

        public Feedback GetFeedbackByTicketId(int ticketId);

        void AddFeedback(FeedbackViewModel model, string userName);

    }
}
