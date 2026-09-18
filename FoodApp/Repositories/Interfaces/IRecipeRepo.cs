using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Interface for manging recipes
    /// </summary>
    public interface IRecipeRepo
    {
        /// <summary>
        /// Return recipes basesd on filters
        /// </summary>
        /// <param name="nameFilter"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="mealNames"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        IEnumerable<RecipeRecord> GetRecipes(string[]? mealNames, string[]? tags, string nameFilter, int page = 1, int pageSize = 25);
        /// <summary>
        /// Return recipe by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Recipe? GetRecipeById(Guid id);
        /// <summary>
        /// Add new recipe to database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="prepTime"></param>
        /// <returns></returns>
        Recipe CreateRecipe(string name, int portion, int prepTime);

        /// <summary>
        /// updatet values in selected object, and return it 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="portion"></param>
        /// <param name="prepTime"></param>
        /// <returns></returns>
        RecipeRecord? UpdateRecipe(Guid id, string? name, int? portion, int? prepTime);
        /// <summary>
        /// Delete recipe record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteRecipe(Guid id);
        /// <summary>
        /// Return list of possible recipes to choose from based on criteria
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mealId"></param>
        /// <param name="excluded"></param>
        /// <param name="excludedWeeks"></param>
        /// <returns></returns>
        RecipeRecord[] GetRecipesToChoose(Guid userId, Guid mealId, int excludedWeeks);
    }
}
