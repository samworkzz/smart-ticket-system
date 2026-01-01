using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.TicketComments;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.TicketComments
{
    public class TicketCommentService : ITicketCommentService
    {
        private readonly AppDbContext _context;

        public TicketCommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCommentAsync(int userId, AddTicketCommentDto dto)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.TicketId == dto.TicketId);

            if (ticket == null)
                throw new Exception("Ticket not found");

            var comment = new TicketComment
            {
                TicketId = dto.TicketId,
                CommentText = dto.Comment,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();

            // Activity log 
            var log = new TicketActivityLog
            {
                TicketId = dto.TicketId,
                Action = "Comment Added",
                OldValue = null,
                NewValue = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
