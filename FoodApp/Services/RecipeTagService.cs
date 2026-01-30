using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IRecipeTagService
    {
        IEnumerable<RecipeTag> GetRecipeTags(int? recipeId, int? tagId);
        RecipeTag? CreateRecipeTag(RecipeTag request);
        bool DeleteRecipeTag(int recipeId, int tagId);
    }

    public class RecipeTagService : IRecipeTagService
    {
        private readonly IRecipeTagRepository _repo;
        public RecipeTagService(IRecipeTagRepository repo) { _repo = repo; }

        public IEnumerable<RecipeTag> GetRecipeTags(int? recipeId, int? tagId) => _repo.GetRecipeTags(recipeId, tagId);
        public RecipeTag? CreateRecipeTag(RecipeTag request)
        {
            return _repo.CreateRecipeTag(request.recipeId, request.tagId);
        }
        public bool DeleteRecipeTag(int recipeId, int tagId) => _repo.DeleteRecipeTag(recipeId, tagId);
    }
}