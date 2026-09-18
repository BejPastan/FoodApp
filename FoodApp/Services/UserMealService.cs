using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    /// <summary>
    /// Interface for managing user meals
    /// </summary>
    public class UserMealService : IUserMealService
    {
        private readonly IUserMealRepository _repo;
        public UserMealService(IUserMealRepository repo) { _repo = repo; }
        /// <summary>
        /// Return list of user meals
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<UserMealWithData> GetUserMeals(Guid userId, DateOnly? startDate, DateOnly? endDate)
        {
            var response = _repo.GetUserMeals(userId, startDate, endDate);
            return response;
        }
        public UserMeal? GetUserMealById(Guid id) => _repo.GetUserMealById(id);
        public UserMeal CreateUserMeal(UserMealCreateRequest toCreate, Guid userId)
        {
            return _repo.CreateUserMeal(userId, toCreate.recipeId, toCreate.mealId, toCreate.mealDate);
        }
        public bool DeleteUserMeal(Guid id) => _repo.DeleteUserMeal(id);

        public UserMeal UpdateUserMeal(UserMealUpdateRequest toUpdate, Guid userId)
        {
           return _repo.UpdateUserMeal(toUpdate.userMealId, toUpdate.recipeId);
        }
    }
}