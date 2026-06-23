using Microsoft.EntityFrameworkCore;
using Domain;

namespace MVC.Data2
{
    public class DemoMVC2Context : DbContext
    {
        public DemoMVC2Context(DbContextOptions<DemoMVC2Context> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventCategory> EventCategorie { get; set; }
        public DbSet<Membership> Membership { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
    }
}