using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Auth;
using SmartTicketApi.Models.Entities;
using SmartTicketApi.Services.Auth;
using Xunit;

namespace SmartTicketApiTests
{
    public class AuthServiceTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposesOnly12345");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _mockConfig.Setup(c => c["Jwt:DurationInMinutes"]).Returns("60");

            _authService = new AuthService(_context, _mockConfig.Object);

            SeedRoles();
        }

        private void SeedRoles()
        {
            _context.Roles.AddRange(
                new Role { RoleName = "EndUser" },
                new Role { RoleName = "SupportAgent" },
                new Role { RoleName = "SupportManager" },
                new Role { RoleName = "Admin" }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenEmailIsUnique()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.Equal("EndUser", result.Role);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Existing User",
                Email = "existing@example.com",
                Password = "Password123!"
            };
            await _authService.RegisterAsync(registerDto);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Login User",
                Email = "login@example.com",
                Password = "Password123!"
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new LoginDto
            {
                Email = "login@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "Login User 2",
                Email = "login2@example.com",
                Password = "Password123!"
            };
            await _authService.RegisterAsync(registerDto);

            var loginDto = new LoginDto
            {
                Email = "login2@example.com",
                Password = "WrongPassword"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
        }
    }
}
