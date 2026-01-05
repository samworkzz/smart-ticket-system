
namespace SmartTicketApi.Models.DTOs.Manager
{
    public class UnassignedTicketDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
