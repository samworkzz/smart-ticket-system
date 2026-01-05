using System.ComponentModel.DataAnnotations;

namespace SmartTicketApi.Models.DTOs.Manager
{
    public class AssignTicketDto
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int AssignedToUserId{ get; set; }
    }
}
