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

            // Role Restriction: Admin can assign SupportManager, SupportAgent OR EndUser (as per request)
            if (role.RoleName == "Admin")
            {
                 throw new Exception("Cannot assign Admin role through this interface.");
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

        public async Task<AdminReportDto> GetAdminReportAsync()
        {
            var report = new AdminReportDto();

            // 1. Global Ticket Stats
            var allTickets = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.AssignedTo)
                .ToListAsync();

            report.TotalTickets = allTickets.Count;
            report.OpenTickets = allTickets.Count(t => t.TicketStatus.StatusName == "Created" || t.TicketStatus.StatusName == "Assigned" || t.TicketStatus.StatusName == "In Progress");
            report.ResolvedTickets = allTickets.Count(t => t.TicketStatus.StatusName == "Resolved" || t.TicketStatus.StatusName == "Closed");
            report.EscalatedTickets = allTickets.Count(t => t.IsEscalated);

            // 2. Manager Summaries (Just list for now)
            var managers = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "SupportManager")
                .ToListAsync();

            report.ManagerSummaries = managers.Select(m => new ManagerSummaryDto
            {
                ManagerId = m.UserId,
                Name = m.Name,
                Email = m.Email
            }).ToList();

            // 3. Agent Summaries (Performance)
            // Reusing logic from TicketService/ManagerReport
            // Filter: Role == SupportAgent
            var agents = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "SupportAgent")
                .ToListAsync();

            // Pre-fetch resolved tickets for calc
            var resolvedTickets = allTickets.Where(t => t.ResolvedAt.HasValue && t.AssignedToId.HasValue).ToList();

            report.AgentSummaries = agents.Select(agent => 
            {
                var agentResolved = resolvedTickets.Where(t => t.AssignedToId == agent.UserId).ToList();
                double avgTime = 0;
                if (agentResolved.Any())
                {
                    avgTime = agentResolved.Average(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours);
                }

                return new SmartTicketApi.Models.DTOs.Manager.AgentPerformanceDto
                {
                    AgentId = agent.UserId,
                    AgentName = agent.Name,
                    ResolvedCount = agentResolved.Count,
                    AvgResolutionHours = Math.Round(avgTime, 1)
                };
            }).ToList();

            return report;
        }
    }
}
