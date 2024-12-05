using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketService
    {
        List<TicketServiceModel> GetAllTickets();
        List<Ticket> GetTickets();

        List<TicketServiceModel> GetFilteredTickets(int? categoryId, int? priorityId, int? statusId);
        Ticket GetTicketById(int id);

        void UpdateTicket(Ticket ticket);

        int GetTicketCount();
        int GetTicketCountByStatus(string statusType);
        int GetTicketCountByCategory(string categoryType);
        int GetTicketCountByPriority(string priorityType);


        void AddTicket(TicketViewModel model, string userName);
        void DeleteTicket(int ticketId);

        void EditDetailsTicket(Ticket ticket);

    }
}
