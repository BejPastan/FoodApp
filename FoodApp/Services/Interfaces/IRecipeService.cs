using FoodApp.Models;

namespace FoodApp.Services.Interfaces
{
    /// <summary>
    /// Service for manging recipes
    /// </summary>
    public interface IRecipeService
    {
        /// <summary>
        /// return list of recipes with given filters
        /// </summary>
        /// <param name="mealsIds"></param>
        /// <param name="tags"></param>
        /// <param name="nameFilter"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<RecipeRecord> GetRecipes(string[]? mealsIds, string[]? tags, string nameFilter, int page = 1, int pageSize = 25);
        /// <summary>
        /// Return recipe by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Recipe? GetRecipeById(Guid id);
        Recipe? CreateRecipe(RecipeCreateRequest request);
        Recipe? UpdateRecipe(Guid recipeId, RecipeUpdateRequest request);
        bool DeleteRecipe(Guid id);
        RecipeRecord[] GetRecipeToChoose(Guid mealId, Guid userId, int excludedWeeks, int chooseSize);
    }
}
