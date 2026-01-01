
namespace SmartTicketApi.Models.Entities
{
    public class TicketStatus
    {
        public int TicketStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        // Navigation
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
