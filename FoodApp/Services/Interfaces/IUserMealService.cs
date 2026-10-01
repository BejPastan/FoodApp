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
        IEnumerable<KitchenMealWithData> GetUserMeals(Guid userId, DateOnly? startDate, DateOnly? endDate);
        /// <summary>
        /// return user meal record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KitchenMeal? GetUserMealById(Guid id);
        KitchenMeal CreateUserMeal(KitchenMealCreateRequest toCreate, Guid userId);
        KitchenMeal UpdateUserMeal(KitchenMealUpdateRequest toUpdate, Guid userId);
        bool DeleteUserMeal(Guid id);
    }

}
