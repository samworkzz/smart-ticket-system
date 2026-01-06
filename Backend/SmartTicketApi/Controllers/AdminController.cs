using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicketApi.Models.DTOs.Admin;
using SmartTicketApi.Models.DTOs.Shared;
using SmartTicketApi.Services.Admin;

namespace SmartTicketApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // --- Categories ---
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() => Ok(await _adminService.GetCategoriesAsync());

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] string name) => Ok(await _adminService.CreateCategoryAsync(name));

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] string name)
        {
            await _adminService.UpdateCategoryAsync(id, name);
            return Ok(new { Message = "Category updated" });
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _adminService.DeleteCategoryAsync(id);
            return Ok(new { Message = "Category deleted" });
        }

        // --- Priorities ---
        [HttpGet("priorities")]
        public async Task<IActionResult> GetPriorities() => Ok(await _adminService.GetPrioritiesAsync());

        [HttpPost("priorities")]
        public async Task<IActionResult> CreatePriority([FromBody] string name) => Ok(await _adminService.CreatePriorityAsync(name));

        [HttpPut("priorities/{id}")]
        public async Task<IActionResult> UpdatePriority(int id, [FromBody] string name)
        {
            await _adminService.UpdatePriorityAsync(id, name);
            return Ok(new { Message = "Priority updated" });
        }

        [HttpDelete("priorities/{id}")]
        public async Task<IActionResult> DeletePriority(int id)
        {
            await _adminService.DeletePriorityAsync(id);
            return Ok(new { Message = "Priority deleted" });
        }

        // --- SLAs ---
        [HttpGet("slas")]
        public async Task<IActionResult> GetSLAs() => Ok(await _adminService.GetSLAsAsync());

        [HttpPut("slas/{id}")]
        public async Task<IActionResult> UpdateSLA(int id, [FromBody] int responseHours)
        {
            await _adminService.UpdateSLAAsync(id, responseHours);
            return Ok(new { Message = "SLA updated" });
        }

        // --- User Management ---
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] PagedRequestDto pagination) => Ok(await _adminService.GetUsersWithRolesAsync(pagination));

        [HttpPut("users/role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto dto)
        {
            await _adminService.UpdateUserRoleAsync(dto);
            return Ok(new { Message = "User role updated successfully" });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles() => Ok(await _adminService.GetRolesAsync());
    }
}
