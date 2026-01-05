namespace SmartTicketApi.Models.DTOs.Tickets
{
    public class TicketDetailsDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? AssignedTo { get; set; }

        public List<TicketCommentDto> Comments { get; set; } = new();
        public List<TicketLogDto> ActivityLogs { get; set; } = new();
    }

    public class TicketCommentDto
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // Person who commented
        public DateTime CreatedAt { get; set; }
    }

    public class TicketLogDto
    {
        public int LogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
