using FoodApp.Models;

namespace FoodApp.Services.Interfaces
{
    public interface IUserMealService
    {
        /// <summary>
        /// return list of user meal between given dates
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<UserMealWithData> GetUserMeals(Guid userId, DateOnly? startDate, DateOnly? endDate);
        /// <summary>
        /// return user meal record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserMeal? GetUserMealById(Guid id);
        UserMeal CreateUserMeal(UserMealCreateRequest toCreate, Guid userId);
        UserMeal UpdateUserMeal(UserMealUpdateRequest toUpdate, Guid userId);
        bool DeleteUserMeal(Guid id);
    }

}
