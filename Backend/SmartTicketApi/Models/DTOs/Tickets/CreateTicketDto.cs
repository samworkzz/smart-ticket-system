using System.ComponentModel.DataAnnotations;

namespace SmartTicketApi.Models.DTOs.Tickets
{
    public class CreateTicketDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int TicketCategoryId { get; set; }

        // User sets priority at creation
        [Required]
        public int TicketPriorityId { get; set; }
    }
}
