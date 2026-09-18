using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Interface for managing user meals
    /// </summary>
    public interface IUserMealRepository
    {
        IEnumerable<UserMealWithData> GetUserMeals(Guid userId, DateOnly? startDate, DateOnly? endDate);
        UserMeal? GetUserMealById(Guid id);
        /// <summary>
        /// Add new user meal record to database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recipeId"></param>
        /// <param name="mealId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        UserMeal CreateUserMeal(Guid userId, Guid recipeId, Guid mealId, DateOnly date, int portions =1);
        bool DeleteUserMeal(Guid id);
        /// <summary>
        /// Finds a user meal by user ID, meal ID, and meal date
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="mealId">The ID of the meal</param>
        /// <param name="mealDate">The date of the meal</param>
        /// <returns>return id or -1 if not found</returns>
        int[] FindUserMealId(Guid userId, Guid mealId, DateOnly mealDate);
        /// <summary>
        /// Updates a user meal record
        /// </summary>
        /// <param name="id">The ID of the user meal to update</param>
        /// <param name="userId">Optional new user ID</param>
        /// <param name="mealId">Optional new meal ID</param>
        /// <param name="mealDate">Optional new meal date</param>
        /// <returns>Updated UserMeal record or null if no fields to update</returns>
        UserMeal UpdateUserMeal(Guid userMealId, Guid recipeId);
    }
}
