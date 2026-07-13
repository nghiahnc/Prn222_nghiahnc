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

        public DbSet<User> User { get; set; } = default!;
        public DbSet<Booking> Booking { get; set; } = default!;
        public DbSet<Event> Event { get; set; } = default!;
        public DbSet<EventCategory> EventCategory { get; set; } = default!;
        public DbSet<Membership> Membership { get; set; } = default!;
        public DbSet<Transaction> Transaction { get; set; } = default!;
        public DbSet<Ticket> Ticket { get; set; } = default!;
        public DbSet<TicketType> TicketType { get; set; } = default!;
        public DbSet<Login> Login { get; set; } = default!;

        public DbSet<OrganizerAuditLog> OrganizerAuditLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Booking>().ToTable("Bookings");
            modelBuilder.Entity<Event>().ToTable("Events");
            modelBuilder.Entity<EventCategory>().ToTable("EventCategories");

            modelBuilder.Entity<Ticket>().ToTable("Ticket");
            modelBuilder.Entity<TicketType>().ToTable("TicketType");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Membership>().ToTable("Membership");
            modelBuilder.Entity<Login>().ToTable("Login");

            modelBuilder.Entity<OrganizerAuditLog>().ToTable("OrganizerAuditLogs");

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizerAuditLog>()
                .HasOne(l => l.Organizer)
                .WithMany()
                .HasForeignKey(l => l.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketType>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);
        }
    }
}