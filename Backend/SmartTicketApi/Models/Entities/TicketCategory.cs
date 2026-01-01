
namespace SmartTicketApi.Models.Entities
{
    public class TicketCategory
    {
        public int TicketCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Navigation
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
