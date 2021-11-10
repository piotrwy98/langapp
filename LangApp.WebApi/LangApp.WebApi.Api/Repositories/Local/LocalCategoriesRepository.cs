using LangApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class LocalCategoriesRepository : ICategoriesRepository
    {
        private static readonly List<Level> _levels = new List<Level>()
        {
            new Level { Id = 1, Name = "A1/A2" },
            new Level { Id = 2, Name = "B1/B2" },
            new Level { Id = 3, Name = "C1/C2" }
        };

        private readonly List<Category> _categories = new List<Category>()
        {
            new Category { Id = 1, Level = _levels[0], Name = "Zwierzęta", ImagePath = "https://cdn.pixabay.com/photo/2021/01/21/16/17/english-cocker-spaniel-5937757_960_720.jpg" },
            new Category { Id = 2, Level = _levels[0], Name = "Kolory", ImagePath = "https://cdn.pixabay.com/photo/2017/07/03/20/17/colorful-2468874_960_720.jpg" },
            new Category { Id = 3, Level = _levels[1], Name = "Zawody", ImagePath = "https://cdn.pixabay.com/photo/2016/01/19/15/05/doctor-1149149_960_720.jpg" },
            new Category { Id = 4, Level = _levels[0], Name = "Dyscypliny sportowe", ImagePath = "https://cdn.pixabay.com/photo/2016/11/19/17/20/athlete-1840437_960_720.jpg" },
            new Category { Id = 5, Level = _levels[1], Name = "Ubrania", ImagePath = "https://cdn.pixabay.com/photo/2017/04/05/01/12/traveler-2203666_960_720.jpg" }
        };

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await Task.FromResult(_categories);
        }

        public async Task<Category> GetCategoryAsync(uint id)
        {
            return await Task.FromResult(_categories.FirstOrDefault(x => x.Id == id));
        }

        public async Task CreateCategoryAsync(Category category)
        {
            _categories.Add(category);
            await Task.CompletedTask;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var index = _categories.FindIndex(x => x.Id == category.Id);
            if (index >= 0)
            {
                _categories[index] = category;
            }

            await Task.CompletedTask;
        }

        public async Task DeleteCategoryAsync(uint id)
        {
            var index = _categories.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                _categories.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
