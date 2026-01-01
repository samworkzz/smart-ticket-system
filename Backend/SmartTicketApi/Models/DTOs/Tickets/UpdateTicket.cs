using System.ComponentModel.DataAnnotations;

namespace SmartTicketApi.Models.DTOs.Tickets
{
    public class UpdateTicketStatusDto
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int TicketStatusId { get; set; }
    }
}
