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
        public DbSet<Login> Login { get; set; } = default!;
        public DbSet<Membership> Membership { get; set; } = default!;
        public DbSet<Transaction> Transaction { get; set; } = default!;
        public DbSet<Ticket> Ticket { get; set; } = default!;
        public DbSet<TicketType> TicketType { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<EventCategory>().ToTable("EventCategory");
            modelBuilder.Entity<Login>().ToTable("Login");
            modelBuilder.Entity<Membership>().ToTable("Membership");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Ticket>().ToTable("Ticket");
            modelBuilder.Entity<TicketType>().ToTable("TicketType");

            modelBuilder.Entity<TicketType>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>()
                .HasOne<Membership>()
                .WithMany(m => m.Users)
                .HasForeignKey(u => u.MembershipId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Transaction)
                .WithMany()
                .HasForeignKey(b => b.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
