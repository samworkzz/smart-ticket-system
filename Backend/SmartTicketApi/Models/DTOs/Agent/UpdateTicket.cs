using System.ComponentModel.DataAnnotations;

namespace SmartTicketApi.Models.DTOs.Agent
{
    public class UpdateTicketStatusDto
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int TicketStatusId { get; set; }

        public string? ResolutionDetails { get; set; }
    }
}
