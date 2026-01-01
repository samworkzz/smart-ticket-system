
namespace SmartTicketApi.Models.Entities
{
    public class TicketPriority
    {
        public int TicketPriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;

        // Navigation
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        //FK
        public SLA SLA { get; set; }
    }
}
