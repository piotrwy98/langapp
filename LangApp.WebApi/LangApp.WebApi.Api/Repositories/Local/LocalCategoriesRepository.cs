using LangApp.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LangApp.WebApi.Api.Repositories.Local
{
    public class LocalCategoriesRepository : ICategoriesRepository
    {
        private static readonly List<Category> _categories = new List<Category>()
        {
            new Category { Id = 1, Level = Enums.Level.A, ImagePath = "https://cdn.pixabay.com/photo/2021/01/21/16/17/english-cocker-spaniel-5937757_960_720.jpg" },
            new Category { Id = 2, Level = Enums.Level.A, ImagePath = "https://cdn.pixabay.com/photo/2017/07/03/20/17/colorful-2468874_960_720.jpg" },
            new Category { Id = 3, Level = Enums.Level.B, ImagePath = "https://cdn.pixabay.com/photo/2016/01/19/15/05/doctor-1149149_960_720.jpg" },
            new Category { Id = 4, Level = Enums.Level.A, ImagePath = "https://cdn.pixabay.com/photo/2016/11/19/17/20/athlete-1840437_960_720.jpg" },
            new Category { Id = 5, Level = Enums.Level.B, ImagePath = "https://cdn.pixabay.com/photo/2017/04/05/01/12/traveler-2203666_960_720.jpg" }
        };

        private readonly List<CategoryName> _categoryNames = new List<CategoryName>()
        {
            new CategoryName { Id = 1, LanguageId = 0, Language = LocalTranslationsRepository.Languages[0], CategoryId = 1, Category = _categories[0], Value = "Zwierzęta"},
            new CategoryName { Id = 2, LanguageId = 0, Language = LocalTranslationsRepository.Languages[0], CategoryId = 2, Category = _categories[1], Value = "Kolory"},
            new CategoryName { Id = 3, LanguageId = 0, Language = LocalTranslationsRepository.Languages[0], CategoryId = 3, Category = _categories[2], Value = "Zawody"},
            new CategoryName { Id = 4, LanguageId = 0, Language = LocalTranslationsRepository.Languages[0], CategoryId = 4, Category = _categories[3], Value = "Sport"},
            new CategoryName { Id = 5, LanguageId = 0, Language = LocalTranslationsRepository.Languages[0], CategoryId = 5, Category = _categories[4], Value = "Ubrania"}
        };

        public async Task<IEnumerable<CategoryName>> GetCategoriesAsync()
        {
            return await Task.FromResult(_categoryNames);
        }

        public async Task<CategoryName> GetCategoryAsync(uint id)
        {
            return await Task.FromResult(_categoryNames.FirstOrDefault(x => x.Id == id));
        }

        public async Task<CategoryName> CreateCategoryAsync(CategoryName category)
        {
            category.Id = (uint) _categoryNames.Count + 1;
            _categoryNames.Add(category);

            return await Task.FromResult(category);
        }

        public async Task UpdateCategoryAsync(CategoryName category)
        {
            var index = _categoryNames.FindIndex(x => x.Id == category.Id);
            if (index >= 0)
            {
                _categoryNames[index] = category;
            }

            await Task.CompletedTask;
        }

        public async Task DeleteCategoryAsync(uint id)
        {
            var index = _categoryNames.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                _categoryNames.RemoveAt(index);
            }

            await Task.CompletedTask;
        }
    }
}
