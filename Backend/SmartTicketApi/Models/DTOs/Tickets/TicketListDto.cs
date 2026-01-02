namespace SmartTicketApi.Models.DTOs
{
    public class TicketListDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? AssignedTo { get; set; }
    }
}
