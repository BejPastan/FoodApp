using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IRecipeMealService
    {
        IEnumerable<RecipeMeal> GetRecipeMeals(int? recipeId, int? mealId);
        RecipeMeal? GetRecipeMealById(int id);
        RecipeMeal? CreateRecipeMeal(RecipeMeal request);
        bool DeleteRecipeMeal(int id);
        bool DeleteRecipeMeal(int recipeId, int mealId);
    }

    public class RecipeMealService : IRecipeMealService
    {
        private readonly IRecipeMealRepository _repo;
        public RecipeMealService(IRecipeMealRepository repo) { _repo = repo; }

        public IEnumerable<RecipeMeal> GetRecipeMeals(int? recipeId, int? mealId) => _repo.GetRecipeMeals(recipeId, mealId);
        public RecipeMeal? GetRecipeMealById(int id) => _repo.GetRecipeMealById(id);
        public RecipeMeal? CreateRecipeMeal(RecipeMeal request)
        {
            return _repo.CreateRecipeMeal(request.recipeId, request.mealId);
        }
        public bool DeleteRecipeMeal(int id) => _repo.DeleteRecipeMeal(id);
        public bool DeleteRecipeMeal(int recipeId, int mealId) => _repo.DeleteRecipeMeal(recipeId, mealId);
    }
}