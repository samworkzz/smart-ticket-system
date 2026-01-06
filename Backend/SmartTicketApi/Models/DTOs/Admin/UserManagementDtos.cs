
namespace SmartTicketApi.Models.DTOs.Admin
{
    public class UserListDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class UpdateUserRoleDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
