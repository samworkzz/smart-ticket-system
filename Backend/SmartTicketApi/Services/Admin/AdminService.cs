using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Extensions;
using SmartTicketApi.Models.DTOs.Admin;
using SmartTicketApi.Models.Entities;

namespace SmartTicketApi.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        // Categories
        public async Task<List<TicketCategory>> GetCategoriesAsync() => await _context.TicketCategories.ToListAsync();

        public async Task<int> CreateCategoryAsync(string name)
        {
            var category = new TicketCategory { CategoryName = name };
            _context.TicketCategories.Add(category);
            await _context.SaveChangesAsync();
            return category.TicketCategoryId;
        }

        public async Task UpdateCategoryAsync(int id, string name)
        {
            var category = await _context.TicketCategories.FindAsync(id);
            if (category == null) throw new Exception("Category not found");
            category.CategoryName = name;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.TicketCategories.FindAsync(id);
            if (category == null) throw new Exception("Category not found");
            if (await _context.Tickets.AnyAsync(t => t.TicketCategoryId == id))
                throw new Exception("Cannot delete category with associated tickets");
            _context.TicketCategories.Remove(category);
            await _context.SaveChangesAsync();
        }

        // Priorities
        public async Task<List<TicketPriority>> GetPrioritiesAsync() => await _context.TicketPriorities.ToListAsync();

        public async Task<int> CreatePriorityAsync(string name)
        {
            var priority = new TicketPriority { PriorityName = name };
            _context.TicketPriorities.Add(priority);
            await _context.SaveChangesAsync();
            return priority.TicketPriorityId;
        }

        public async Task UpdatePriorityAsync(int id, string name)
        {
            var priority = await _context.TicketPriorities.FindAsync(id);
            if (priority == null) throw new Exception("Priority not found");
            priority.PriorityName = name;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePriorityAsync(int id)
        {
            var priority = await _context.TicketPriorities.FindAsync(id);
            if (priority == null) throw new Exception("Priority not found");
            if (await _context.Tickets.AnyAsync(t => t.TicketPriorityId == id))
                throw new Exception("Cannot delete priority with associated tickets");
            _context.TicketPriorities.Remove(priority);
            await _context.SaveChangesAsync();
        }

        // SLAs
        public async Task<List<SLA>> GetSLAsAsync() => await _context.SLAs.Include(s => s.TicketPriority).ToListAsync();

        public async Task UpdateSLAAsync(int id, int responseHours)
        {
            var sla = await _context.SLAs.FindAsync(id);
            if (sla == null) throw new Exception("SLA not found");
            sla.ResponseHours = responseHours;
            await _context.SaveChangesAsync();
        }

        // User Management
        public async Task<PagedResponseDto<UserListDto>> GetUsersWithRolesAsync(PagedRequestDto pagination)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.SortBy, pagination.SortDescending)
                .ApplyPaging(pagination.PageNumber, pagination.PageSize)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role.RoleName
                })
                .ToListAsync();

            return new PagedResponseDto<UserListDto>(items, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) throw new Exception("User not found");

            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null) throw new Exception("Role not found");

            // Role Restriction: Admin can only assign SupportManager or SupportAgent
            if (role.RoleName != "SupportManager" && role.RoleName != "SupportAgent" && role.RoleName != "EndUser")
            {
                 // Small adjustment: allowing revert to EndUser might be useful too, 
                 // but strictly following "make users as support manager or agent".
                 // Let's stick to the user's specific text: "support manager or agent"
                 if (role.RoleName == "Admin")
                 {
                     throw new Exception("Cannot assign Admin role through this interface.");
                 }
            }
            
            // Re-evaluating based on "admin can make users as support manager or agent"
            if (role.RoleName != "SupportManager" && role.RoleName != "SupportAgent")
            {
                 throw new Exception("Admins can only assign 'Support Manager' or 'Support Agent' roles.");
            }

            user.RoleId = dto.RoleId;
            await _context.SaveChangesAsync();
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToListAsync();
        }
    }
}
