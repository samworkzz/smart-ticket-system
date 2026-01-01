
namespace SmartTicketApi.Models.Entities
{
    public class TicketComment
    {
        public int TicketCommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Foreign Keys
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

       
    }
}
