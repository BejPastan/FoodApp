using FoodApp.Models;

namespace FoodApp.Services.Interfaces
{
    /// <summary>
    /// Interface to manage Kitchen meals
    /// </summary>
    public interface IKitchenMealService
    {
        /// <summary>
        /// get meals for given kitchen
        /// </summary>
        /// <param name="kitchenId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<KitchenMealWithData> GetKitchenMeals(Guid kitchenId, DateOnly? startDate, DateOnly? endDate);
        KitchenMeal? GetKitchenMealById(Guid id);
        /// <summary>
        /// Add new user meal record to database
        /// </summary>
        /// <param name="kitchenId"></param>
        /// <param name="recipeId"></param>
        /// <param name="mealId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        KitchenMeal CreateKitchenMeal(Guid kitchenId, Guid recipeId, Guid mealId, DateOnly date, int portions = 1);
        bool DeleteKitchenMeal(Guid id);
        /// <summary>
        /// Finds a user meal by user ID, meal ID, and meal date
        /// </summary>
        /// <param name="kitchenId">The ID of the user</param>
        /// <param name="mealId">The ID of the meal</param>
        /// <param name="mealDate">The date of the meal</param>
        /// <returns>return id or -1 if not found</returns>
        int[] FindKitchenMealId(Guid kitchenId, Guid mealId, DateOnly mealDate);
        /// <summary>
        /// Updates a user meal record
        /// </summary>
        /// <param name="id">The ID of the user meal to update</param>
        /// <param name="userId">Optional new user ID</param>
        /// <param name="mealId">Optional new meal ID</param>
        /// <param name="mealDate">Optional new meal date</param>
        /// <returns>Updated UserMeal record or null if no fields to update</returns>
        KitchenMeal UpdateKitchenMeal(Guid kitchenMealId, Guid recipeId);

        /// <summary>
        /// return kitchen into which meal belongs
        /// </summary>
        /// <param name="mealId"></param>
        /// <returns></returns>
        Kitchen GetKitchenByMealId(Guid mealId);

        /// <summary>
        /// return list of possible options to choose from when selecting random meal
        /// </summary>
        /// <param name="mealId"></param>
        /// <param name="kitchenId"></param>
        /// <param name="excludedWeeks"></param>
        /// <param name="chooseSize"></param>
        /// <returns></returns>
        RecipeRecord[] GetRecipeToChoose(Guid mealId, Guid kitchenId, int excludedWeeks, int chooseSize);
    }
}
