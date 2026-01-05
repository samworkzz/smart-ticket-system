using Microsoft.EntityFrameworkCore;
using SmartTicketApi.Data;
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
    }
}
