using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class TicketRepository : BaseRepository, ITicketRepository
    {
        public TicketRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<Ticket> GetTickets()
        {
            return this.GetDbSet<Ticket>();
        }

        public bool TicketExist(int TicketId)
        {
            return this.GetDbSet<Ticket>().Any(x => x.TicketId == TicketId);
        }

        public void AddTicket(Ticket ticket)
        {
            this.GetDbSet<Ticket>().Add(ticket);
            UnitOfWork.SaveChanges();
        }

        public void UpdateTicket(Ticket ticket)
        {
            this.GetDbSet<Ticket>().Update(ticket);
            UnitOfWork.SaveChanges();
        }

        public void DeleteTicket(Ticket ticket)
        {
            Console.WriteLine($"Deleting ticket ID: {ticket.TicketId}"); // Debugging step
            this.GetDbSet<Ticket>().Remove(ticket);
            UnitOfWork.SaveChanges();
        }

    }
}
