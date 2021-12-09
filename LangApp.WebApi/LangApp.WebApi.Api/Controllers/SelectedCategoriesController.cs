using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("selected-categories")]
    public class SelectedCategoriesController : ControllerBase
    {
        private readonly ISelectedCategoriesRepository _selectedCategoriesRepository;
        private readonly ISessionsRepository _sessionsRepository;

        public SelectedCategoriesController(ISelectedCategoriesRepository selectedCategoriesRepository, ISessionsRepository sessionsRepository)
        {
            _selectedCategoriesRepository = selectedCategoriesRepository;
            _sessionsRepository = sessionsRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<SelectedCategory>> GetSelectedCategoriesAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _selectedCategoriesRepository.GetSelectedCategoriesAsync(uint.Parse(userId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SelectedCategory>> GetSelectedCategoryAsync(uint id)
        {
            var selectedCategory = await _selectedCategoriesRepository.GetSelectedCategoryAsync(id);

            if (selectedCategory != null)
            {
                var session = await _sessionsRepository.GetSessionAsync(selectedCategory.SessionId);

                if (session != null)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (session.UserId != uint.Parse(userId))
                    {
                        return Unauthorized();
                    }
                }

                return selectedCategory;
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SelectedCategory>> CreateSelectedCategoryAsync([FromBody] SelectedCategory selectedCategory)
        {
            var session = await _sessionsRepository.GetSessionAsync(selectedCategory.SessionId);

            if (session == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (session.UserId != uint.Parse(userId))
            {
                return Unauthorized();
            }

            return await _selectedCategoriesRepository.CreateSelectedCategoryAsync(selectedCategory);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateSelectedCategoryAsync([FromBody] SelectedCategory selectedCategory)
        {
            await _selectedCategoriesRepository.UpdateSelectedCategoryAsync(selectedCategory);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteSelectedCategoryAsync(uint id)
        {
            await _selectedCategoriesRepository.DeleteSelectedCategoryAsync(id);

            return NoContent();
        }
    }
}
