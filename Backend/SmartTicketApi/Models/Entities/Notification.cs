using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTicketApi.Models.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? TicketId { get; set; } // Optional reference for context
        // Not creating a FK to Ticket to avoid cyclical cascading delete issues or complex seeding dependencies, 
        // but could add it if strictly needed. For now, ID is enough for linking in UI.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
