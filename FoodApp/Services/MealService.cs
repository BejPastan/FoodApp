using FoodApp.Repositories;
using FoodApp.Models;

namespace FoodApp.Services
{
    public interface IMealService
    {
        Meal[] GetMeals(string? nameFilter, Guid? recipeId, int page = 1, int pageSize = 25);
        Meal? GetMealById(Guid id);
        Meal CreateMeal(MealCreateRequest request);
        Meal? UpdateMeal(Guid id, MealUpdateRequest request);
        bool DeleteMeal(Guid id);
    }

    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepo;
        public MealService(IMealRepository mealRepo)
        {
            _mealRepo = mealRepo;
        }

        public Meal[] GetMeals(string? nameFilter, Guid? recipeId, int page = 1, int pageSize = 25)
        {
            return _mealRepo.GetMeals(nameFilter, recipeId, page, pageSize).ToArray();
        }

        public Meal? GetMealById(Guid id) => _mealRepo.GetMealById(id);

        public Meal CreateMeal(MealCreateRequest request) => _mealRepo.CreateMeal(request.name, request.order);

        public Meal? UpdateMeal(Guid id, MealUpdateRequest request) => _mealRepo.UpdateMeal(id, request.name, request.order);
        public bool DeleteMeal(Guid id) => _mealRepo.DeleteMeal(id);
    }
}
