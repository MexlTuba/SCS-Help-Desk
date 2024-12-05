using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class FeedbackViewModel
    {
        public int FeedbackId { get; set; }


        public string UserId { get; set; }


        public int TicketId { get; set; }


        public DateTime DateCreated { get; set; } = DateTime.Now;


        public int TicketRating { get; set; }


        public int AgentRating { get; set; }


        public string TicketComment { get; set; }


        public string AgentComment { get; set; }
    }
}
