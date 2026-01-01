
namespace SmartTicketApi.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; } //FK
        public Role Role { get; set; } 

        // Navigation
        public ICollection<Ticket> TicketsCreated { get; set; } = new List<Ticket>();
        public ICollection<Ticket> TicketsAssigned { get; set; } = new List<Ticket>();
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}
