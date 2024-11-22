using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketService
    {
        List<TicketServiceModel> GetAllTickets();
        List<TicketServiceModel> GetFilteredTickets(int? categoryId, int? priorityId, int? statusId);
        Ticket GetTicketById(int id);
    }
}
