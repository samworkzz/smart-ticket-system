using System.ComponentModel.DataAnnotations;

namespace SmartTicketApi.Models.DTOs.TicketComments
{
    public class AddTicketCommentDto
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
