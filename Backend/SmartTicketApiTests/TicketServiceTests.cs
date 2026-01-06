using Microsoft.EntityFrameworkCore;
using Moq;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Models.DTOs.Tickets;
using SmartTicketApi.Models.Entities;
using SmartTicketApi.Services.Notifications;
using SmartTicketApi.Services.Tickets;
using Xunit;

namespace SmartTicketApiTests
{
    public class TicketServiceTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mockNotificationService = new Mock<INotificationService>();

            _ticketService = new TicketService(_context, _mockNotificationService.Object);

            SeedData();
        }

        private void SeedData()
        {
            _context.TicketStatuses.Add(new TicketStatus { StatusName = "Created" });
            _context.TicketPriorities.Add(new TicketPriority { PriorityName = "Normal" });
            _context.TicketCategories.Add(new TicketCategory { CategoryName = "IT Support" });
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateTicketAsync_ShouldCreateTicket_WithCorrectStatus()
        {
            // Arrange
            var createDto = new CreateTicketDto
            {
                Title = "Test Ticket",
                Description = "Test Description",
                TicketCategoryId = _context.TicketCategories.First().TicketCategoryId,
                TicketPriorityId = _context.TicketPriorities.First().TicketPriorityId
            };
            int userId = 1; 

            // Act
            var ticketId = await _ticketService.CreateTicketAsync(userId, createDto);

            // Assert
            var ticket = await _context.Tickets.FindAsync(ticketId);
            Assert.NotNull(ticket);
            Assert.Equal("Test Ticket", ticket.Title);
            Assert.Equal("Created", ticket.TicketStatus?.StatusName ?? _context.TicketStatuses.Find(ticket.TicketStatusId).StatusName);
        }

        [Fact]
        public async Task GetTicketsForEndUserAsync_ShouldReturnOnlyUserTickets()
        {
            // Arrange
            var createdStatus = _context.TicketStatuses.First(s => s.StatusName == "Created");
            var priority = _context.TicketPriorities.First();
            var category = _context.TicketCategories.First();

            _context.Tickets.AddRange(
                new Ticket { Title = "User 1 Ticket", CreatedById = 1, TicketStatusId = createdStatus.TicketStatusId, TicketPriorityId = priority.TicketPriorityId, TicketCategoryId = category.TicketCategoryId, CreatedAt = DateTime.UtcNow },
                new Ticket { Title = "User 2 Ticket", CreatedById = 2, TicketStatusId = createdStatus.TicketStatusId, TicketPriorityId = priority.TicketPriorityId, TicketCategoryId = category.TicketCategoryId, CreatedAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            var pagination = new PagedRequestDto { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _ticketService.GetTicketsForEndUserAsync(1, pagination);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("User 1 Ticket", result.Items.First().Title);
        }
    }
}
