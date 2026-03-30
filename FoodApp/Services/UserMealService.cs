using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public interface IUserMealService
    {
        IEnumerable<UserMealWithData> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate);
        UserMeal? GetUserMealById(int id);
        UserMeal CreateUserMeal(UserMealCreateRequest toCreate, int userId);
        bool DeleteUserMeal(int id);
    }

    public class UserMealService : IUserMealService
    {
        private readonly IUserMealRepository _repo;
        public UserMealService(IUserMealRepository repo) { _repo = repo; }
        public IEnumerable<UserMealWithData> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate)
        {
            var response = _repo.GetUserMeals(userId, startDate, endDate);
            return response;
        }
        public UserMeal? GetUserMealById(int id) => _repo.GetUserMealById(id);
        public UserMeal CreateUserMeal(UserMealCreateRequest toCreate, int userId)
        {
            int id =_repo.FindUserMealId(userId, toCreate.mealId, toCreate.mealDate);

            if(id!=-1)
            {
                return _repo.UpdateUserMeal(id, userId, toCreate.mealId, toCreate.mealDate);
            }
            else
            {
                return _repo.CreateUserMeal(userId, toCreate.recipeId, toCreate.mealId, toCreate.mealDate);
            }
        }
        public bool DeleteUserMeal(int id) => _repo.DeleteUserMeal(id);
    }
}