using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Interface for managing Kitchen meals
    /// </summary>
    public interface IKitchenMealRepository
    {
        /// <summary>
        /// return meals for given part of time
        /// </summary>
        /// <param name="kitchenId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<KitchenMealWithData> GetKitchenMeals(Guid kitchenId, DateOnly? startDate, DateOnly? endDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KitchenMeal? GetKitchenMealById(Guid id);
        /// <summary>
        /// Add new user meal record to database
        /// </summary>
        /// <param name="kitchenId"></param>
        /// <param name="recipeId"></param>
        /// <param name="mealId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        KitchenMeal CreateKitchenMeal(Guid kitchenId, Guid recipeId, Guid mealId, DateOnly date, int portions =1);
        /// <summary>
        /// Delete Kitchen meal record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <param name="userMealId"></param>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        KitchenMeal UpdateKitchenMeal(Guid userMealId, Guid recipeId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kitchenId"></param>
        /// <param name="mealId"></param>
        /// <param name="excludedWeeks"></param>
        /// <returns></returns>
        RecipeRecord[] GetRecipesToChoose(Guid kitchenId, Guid mealId, int excludedWeeks);

        /// <summary>
        /// Return kitchen to which belong given kitchen meal record
        /// </summary>
        /// <param name="mealId"></param>
        /// <returns></returns>
        Kitchen? GetKitchenByMealId(Guid mealId);
    }
}
