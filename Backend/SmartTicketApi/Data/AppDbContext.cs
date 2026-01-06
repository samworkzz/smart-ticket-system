using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<TicketActivityLog> TicketActivityLogs { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<SLA> SLAs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            // User -> Role (1 to Many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> CreatedBy (User)
         
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.TicketsCreated)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

          
            // Ticket -> AssignedTo (User)
           
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.TicketsAssigned)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

        
            // Ticket -> TicketCategory
   
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.TicketCategory)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.TicketCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Ticket -> TicketPriority

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.TicketPriority)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.TicketPriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> TicketStatus

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.TicketStatus)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.TicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketPriority -> SLA (1 to 1)
          
            modelBuilder.Entity<TicketPriority>()
                .HasOne(p => p.SLA)
                .WithOne(s => s.TicketPriority)
                .HasForeignKey<SLA>(s => s.TicketPriorityId)
                .OnDelete(DeleteBehavior.Cascade);

        
            // Ticket -> TicketComment
          
            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // TicketComment -> User

            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(tc => tc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> TicketActivityLog

            modelBuilder.Entity<TicketActivityLog>()
                .HasOne(tl => tl.Ticket)
                .WithMany(t => t.ActivityLogs)
                .HasForeignKey(tl => tl.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
