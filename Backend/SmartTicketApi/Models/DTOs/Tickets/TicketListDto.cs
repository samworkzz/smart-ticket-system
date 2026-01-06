namespace SmartTicketApi.Models.DTOs.Tickets
{
    public class TicketListDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AssignedTo { get; set; }
        public bool IsEscalated { get; set; }
    }

}
