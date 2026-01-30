using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IUserMealService
    {
        IEnumerable<UserMeal> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate);
        UserMeal? GetUserMealById(int id);
        UserMeal CreateUserMeal(UserMeal toCreate);
        bool DeleteUserMeal(int id);
    }

    public class UserMealService : IUserMealService
    {
        private readonly IUserMealRepository _repo;
        public UserMealService(IUserMealRepository repo) { _repo = repo; }
        public IEnumerable<UserMeal> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate) => _repo.GetUserMeals(userId, startDate, endDate);
        public UserMeal? GetUserMealById(int id) => _repo.GetUserMealById(id);
        public UserMeal CreateUserMeal(UserMeal toCreate)
        {
            return _repo.CreateUserMeal(toCreate.userId, toCreate.recipeId, toCreate.mealId, toCreate.mealDate);
        }
        public bool DeleteUserMeal(int id) => _repo.DeleteUserMeal(id);
    }
}