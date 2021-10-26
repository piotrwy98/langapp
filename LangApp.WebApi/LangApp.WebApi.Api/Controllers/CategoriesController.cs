using LangApp.Shared.Models;
using LangApp.Shared.Models.Controllers;
using LangApp.WebApi.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _categoriesRepository.GetCategoriesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryAsync(Guid id)
        {
            var category = await _categoriesRepository.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategoryAsync([FromBody] CategoryData data)
        {
            Category category = new Category()
            {
                Id = Guid.NewGuid(),
                Level = data.Level,
                Name = data.Name,
                ImagePath = data.ImagePath
            };

            await _categoriesRepository.CreateCategoryAsync(category);

            return CreatedAtAction(nameof(GetCategoryAsync), new { id = category.Id }, category);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCategoryAsync([FromBody] Category category)
        {
            await _categoriesRepository.UpdateCategoryAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(Guid id)
        {
            await _categoriesRepository.DeleteCategoryAsync(id);

            return NoContent();
        }
    }
}
