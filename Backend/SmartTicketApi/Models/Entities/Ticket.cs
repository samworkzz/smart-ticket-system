
namespace SmartTicketApi.Models.Entities
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ResolutionDetails { get; set; }

        public bool IsEscalated { get; set; } = false;
        public bool IsReopened { get; set; } = false;

        // Navigation Properties
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public int TicketCategoryId { get; set; }
        public TicketCategory TicketCategory { get; set; }

        public int TicketPriorityId { get; set; }
        public TicketPriority TicketPriority { get; set; }

        public int TicketStatusId { get; set; }
        public TicketStatus TicketStatus { get; set; }

        // Navigation
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
        public ICollection<TicketActivityLog> ActivityLogs { get; set; } = new List<TicketActivityLog>();
    }
}
