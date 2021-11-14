using LangApp.Shared.Models;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Controllers
{
    //[Authorize]
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
        public async Task<IEnumerable<CategoryName>> GetCategoriesAsync()
        {
            return await _categoriesRepository.GetCategoriesAsync();
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<CategoryName>> CreateCategoryAsync([FromBody] CategoryName category)
        {
            return await _categoriesRepository.CreateCategoryAsync(category);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCategoryAsync([FromBody] CategoryName category)
        {
            await _categoriesRepository.UpdateCategoryAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(uint id)
        {
            await _categoriesRepository.DeleteCategoryAsync(id);

            return NoContent();
        }
    }
}
