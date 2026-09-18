using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IRecipeTagService
    {
        IEnumerable<RecipeTag> GetRecipeTags(Guid? recipeId, Guid? tagId);
        RecipeTag? CreateRecipeTag(RecipeTagCreateRequest request);
        bool DeleteRecipeTag(Guid recipeId, Guid tagId);
        bool DeleteRecipeTags(Guid recipeId);
    }

    public class RecipeTagService : IRecipeTagService
    {
        private readonly IRecipeTagRepository _repo;
        public RecipeTagService(IRecipeTagRepository repo) { _repo = repo; }

        public IEnumerable<RecipeTag> GetRecipeTags(Guid? recipeId, Guid? tagId) => _repo.GetRecipeTags(recipeId, tagId);
        public RecipeTag? CreateRecipeTag(RecipeTagCreateRequest request)
        {
            return _repo.CreateRecipeTag(request.recipeId, request.tagId);
        }
        public bool DeleteRecipeTag(Guid recipeId, Guid tagId) => _repo.DeleteRecipeTag(recipeId, tagId);

        /// <summary>
        /// Delete all tags associations with specific recipe
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        public bool DeleteRecipeTags(Guid recipeId)
        {
            Console.WriteLine("Deleting recipe Tags");
            return _repo.DeleteRecipeTags(recipeId);
        }
    }
}