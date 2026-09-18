using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IRecipeMealService
    {
        IEnumerable<RecipeMeal> GetRecipeMeals(Guid? recipeId, Guid? mealId);
        RecipeMeal? GetRecipeMealById(Guid id);
        RecipeMeal? CreateRecipeMeal(CreateRecipeMealRequest request);
        bool DeleteRecipeMeal(Guid id);
        bool DeleteRecipeMeal(Guid recipeId, Guid mealId);
        bool DeleteRecipesMeal(Guid recipeId);
    }

    public class RecipeMealService : IRecipeMealService
    {
        private readonly IRecipeMealRepository _repo;
        public RecipeMealService(IRecipeMealRepository repo) { _repo = repo; }

        public IEnumerable<RecipeMeal> GetRecipeMeals(Guid? recipeId, Guid? mealId) => _repo.GetRecipeMeals(recipeId, mealId);
        public RecipeMeal? GetRecipeMealById(Guid id) => _repo.GetRecipeMealById(id);
        public RecipeMeal? CreateRecipeMeal(CreateRecipeMealRequest request)
        {
            return _repo.CreateRecipeMeal(request.recipeId, request.mealId);
        }
        public bool DeleteRecipeMeal(Guid id) => _repo.DeleteRecipeMeal(id);
        public bool DeleteRecipeMeal(Guid recipeId, Guid mealId) => _repo.DeleteRecipeMeal(recipeId, mealId);


        /// <summary>
        /// Delete all meals for single recipe
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        public bool DeleteRecipesMeal(Guid recipeId)
        {
            return _repo.DeleteRecipesMeal(recipeId);
        }
    }
}