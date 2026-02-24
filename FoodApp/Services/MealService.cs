using FoodApp.Repositories;
using FoodApp.Models;

namespace FoodApp.Services
{
    public interface IMealService
    {
        Meal[] GetMeals(string? nameFilter, int? recipeId, int page = 1, int pageSize = 25);
        Meal? GetMealById(int id);
        Meal CreateMeal(Meal name);
        Meal? UpdateMeal(int id, string? name);
        bool DeleteMeal(int id);
    }

    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepo;
        public MealService(IMealRepository mealRepo)
        {
            _mealRepo = mealRepo;
        }

        public Meal[] GetMeals(string? nameFilter, int? recipeId, int page = 1, int pageSize = 25)
        {
            return _mealRepo.GetMeals(nameFilter, recipeId, page, pageSize).ToArray();
        }

        public Meal? GetMealById(int id) => _mealRepo.GetMealById(id);

        public Meal CreateMeal(Meal name) => _mealRepo.CreateMeal(name);

        public Meal? UpdateMeal(int id, string? name) => _mealRepo.UpdateMeal(id, name);

        public bool DeleteMeal(int id) => _mealRepo.DeleteMeal(id);
    }
}
