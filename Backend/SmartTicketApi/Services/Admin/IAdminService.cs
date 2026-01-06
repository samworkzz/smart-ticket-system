using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Models.DTOs.Admin;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Admin
{
    public interface IAdminService
    {
        // Categories
        Task<List<TicketCategory>> GetCategoriesAsync();
        Task<int> CreateCategoryAsync(string name);
        Task UpdateCategoryAsync(int id, string name);
        Task DeleteCategoryAsync(int id);

        // Priorities
        Task<List<TicketPriority>> GetPrioritiesAsync();
        Task<int> CreatePriorityAsync(string name);
        Task UpdatePriorityAsync(int id, string name);
        Task DeletePriorityAsync(int id);

        // SLAs
        Task<List<SLA>> GetSLAsAsync();
        Task UpdateSLAAsync(int id, int responseHours);

        // User Management
        Task<PagedResponseDto<UserListDto>> GetUsersWithRolesAsync(PagedRequestDto pagination);
        Task UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task<List<RoleDto>> GetRolesAsync();
    }
}
