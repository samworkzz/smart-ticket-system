using SmartTicketApi.Models.DTOs.TicketComments;

namespace SmartTicketApi.Services.TicketComments
{
    public interface ITicketCommentService
    {
        Task AddCommentAsync(int userId, AddTicketCommentDto dto);
    }
}
