using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public interface IUserMealService
    {
        IEnumerable<UserMealWithData> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate);
        UserMeal? GetUserMealById(int id);
        UserMeal CreateUserMeal(UserMeal toCreate);
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
        public UserMeal CreateUserMeal(UserMeal toCreate)
        {
            int id =_repo.FindUserMealId(toCreate.userId, toCreate.mealId, toCreate.mealDate);

            if(id!=-1)
            {
                return _repo.UpdateUserMeal(id, toCreate.userId, toCreate.mealId, toCreate.mealDate);
            }
            else
            {
                return _repo.CreateUserMeal(toCreate.userId, toCreate.recipeId, toCreate.mealId, toCreate.mealDate);
            }
        }
        public bool DeleteUserMeal(int id) => _repo.DeleteUserMeal(id);
    }
}