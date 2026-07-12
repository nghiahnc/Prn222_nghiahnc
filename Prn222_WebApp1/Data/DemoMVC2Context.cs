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
        public DbSet<RefundCancelPolicy> RefundCancelPolicy { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<PaymentGatewayConfig> PaymentGatewayConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table mappings
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Booking>().ToTable("Booking");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<EventCategory>().ToTable("EventCategory");
            modelBuilder.Entity<Membership>().ToTable("Membership");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<Ticket>().ToTable("Ticket");
            modelBuilder.Entity<TicketType>().ToTable("TicketType");
            modelBuilder.Entity<RefundCancelPolicy>().ToTable("RefundCancelPolicy");
            modelBuilder.Entity<SystemSetting>().ToTable("SystemSetting");
            modelBuilder.Entity<PaymentGatewayConfig>().ToTable("PaymentGateway");

            // Event -> EventCategory (one-to-many)
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventCategory)
                .WithMany(ec => ec.Events)
                .HasForeignKey(e => e.EventCategoryId);

            // Event -> User (CreatedBy)
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .IsRequired(false);

            // Event -> TicketType (one-to-many)
            modelBuilder.Entity<TicketType>()
                .HasOne(tt => tt.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(tt => tt.EventId);

            // Event -> RefundCancelPolicy (one-to-one)
            modelBuilder.Entity<RefundCancelPolicy>()
                .HasKey(r => r.EventId);

            modelBuilder.Entity<RefundCancelPolicy>()
                .HasOne(r => r.Event)
                .WithOne(e => e.RefundCancelPolicy)
                .HasForeignKey<RefundCancelPolicy>(r => r.EventId);

            // TicketType.Price precision
            modelBuilder.Entity<TicketType>()
                .Property(tt => tt.Price)
                .HasPrecision(18, 2);

            // User -> Membership (optional one-to-many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Membership)
                .WithMany(m => m.Users)
                .HasForeignKey(u => u.MembershipId)
                .IsRequired(false);
        }
    }
}