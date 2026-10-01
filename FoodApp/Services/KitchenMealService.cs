using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;

namespace FoodApp.Services
{
    /// <summary>
    /// Interface for managing user meals
    /// </summary>
    public class KitchenMealService : IKitchenMealService
    {
        private readonly IKitchenMealRepository _repo;
        public KitchenMealService(IKitchenMealRepository repo) { _repo = repo; }

        ///<inheritdoc/>
        public KitchenMeal? GetKitchenMealById(Guid id)
        {
            return _repo.GetKitchenMealById(id);
        }

        ///<inheritdoc/>
        public KitchenMeal CreateKitchenMeal(Guid kitchenId, Guid recipeId, Guid mealId, DateOnly date, int portions = 1)
        {
            return _repo.CreateKitchenMeal(kitchenId, recipeId, mealId, date);
        }

        ///<inheritdoc/>
        public bool DeleteKitchenMeal(Guid id)
        {
            return _repo.DeleteKitchenMeal(id);
        }

        public int[] FindKitchenMealId(Guid kitchenId, Guid mealId, DateOnly mealDate)
        {
            return _repo.FindKitchenMealId(kitchenId, mealId, mealDate);
        }

        ///<inheritdoc/>
        public KitchenMeal UpdateKitchenMeal(Guid kitchenMealId, Guid recipeId)
        {
            return _repo.UpdateKitchenMeal(kitchenMealId, recipeId);
        }

        ///<inheritdoc/>
        public Kitchen GetKitchenByMealId(Guid mealId)
        {
            return _repo.GetKitchenByMealId(mealId);
        }

        ///<inheritdoc/>
        public IEnumerable<KitchenMealWithData> GetKitchenMeals(Guid kitchenId, DateOnly? startDate, DateOnly? endDate)
        {
            return _repo.GetKitchenMeals(kitchenId, startDate, endDate);
        }

        public RecipeRecord[] GetRecipeToChoose(Guid mealId, Guid kitchenId, int excludedWeeks, int chooseSize)
        {
            return _repo.GetRecipesToChoose(kitchenId, mealId, excludedWeeks);
        }
    }
}