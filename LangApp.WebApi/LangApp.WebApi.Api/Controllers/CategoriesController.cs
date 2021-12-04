using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesController(ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<CategoryName>> GetCategoriesAsync()
        {
            return await _categoriesRepository.GetCategoriesAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoryName>> GetCategoryAsync(uint id)
        {
            var category = await _categoriesRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<CategoryName>> CreateCategoryAsync([FromBody] CategoryName category)
        {
            return await _categoriesRepository.CreateCategoryAsync(category);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateCategoryAsync([FromBody] CategoryName category)
        {
            await _categoriesRepository.UpdateCategoryAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteCategoryAsync(uint id)
        {
            await _categoriesRepository.DeleteCategoryAsync(id);

            return NoContent();
        }
    }
}
