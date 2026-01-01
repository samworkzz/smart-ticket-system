
namespace SmartTicketApi.Models.Entities
{
    public class TicketActivityLog
    {
        public int TicketActivityLogId { get; set; }

        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //foreign keys

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        
    }
}
