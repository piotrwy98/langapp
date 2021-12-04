using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("selected-categories")]
    public class SelectedCategoriesController : ControllerBase
    {
        private readonly ISelectedCategoriesRepository _selectedCategoriesRepository;

        public SelectedCategoriesController(ISelectedCategoriesRepository selectedCategoriesRepository)
        {
            _selectedCategoriesRepository = selectedCategoriesRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<SelectedCategory>> GetSelectedCategoriesAsync()
        {
            return await _selectedCategoriesRepository.GetSelectedCategoriesAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SelectedCategory>> GetSelectedCategoryAsync(uint id)
        {
            var selectedCategory = await _selectedCategoriesRepository.GetSelectedCategoryAsync(id);
            if (selectedCategory == null)
            {
                return NotFound();
            }

            return selectedCategory;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SelectedCategory>> CreateSelectedCategoryAsync([FromBody] SelectedCategory selectedCategory)
        {
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
