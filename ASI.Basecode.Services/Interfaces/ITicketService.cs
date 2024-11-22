using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITicketService
    {
        List<TicketServiceModel> GetAllTickets();

        Ticket GetTicketById(int id);

        void UpdateTicket(Ticket ticket);

        int GetTicketCount();
        int GetTicketCountByStatus(string statusType);

        void AddTicket(TicketViewModel model, string userName);
        List<Ticket> GetTickets();
        void DeleteTicket(int ticketId);
        void UpdateTicket(Ticket ticket);

    }
}
