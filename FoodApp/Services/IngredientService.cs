using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public interface IIngredientService
    {
        IEnumerable<Ingredient> GetIngredients(int? recipeId, int? foodId, int page = 1, int perPage = 25);
        Ingredient? GetIngredientById(int id);
        Ingredient CreateIngredient(Ingredient request);
        Ingredient? UpdateIngredient(Ingredient request);
        bool DeleteIngredient(int id);
    }

    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repo;
        private readonly IFoodService _foodServ;
        private readonly IUnitService _unitServ;
        public IngredientService(IIngredientRepository repo, IFoodService foodServ, IUnitService unitServ)
        {
            _repo = repo;
            _foodServ = foodServ;
            _unitServ = unitServ;
        }

        public IEnumerable<Ingredient> GetIngredients(int? recipeId, int? foodId, int page = 1, int perPage = 25)
        {
            Ingredient[] ingredients = _repo.GetIngredients(recipeId, foodId, page, perPage).ToArray();
            foreach (var ingredient in ingredients)
            {
                FormatIngridient(ingredient);
            }
            return ingredients;
        }
        public Ingredient? GetIngredientById(int id)
        {
            var ingredient = _repo.GetIngredientById(id);
            if (ingredient == null)
            {
                return null;
            }
            return FormatIngridient(ingredient, FormatMode.full);
        }

        public Ingredient CreateIngredient(Ingredient request)
        {
            // Ensure food exists
            int foodId = request.foodId;
            if ((foodId == 0) && request.food != null)
            {
                var createdFood = _foodServ.CreateFood(request.food);
                foodId = createdFood.id;
            }

            // Ensure unit exists
            int unitId = request.unitId;
            if ((unitId == 0) && request.unit != null)
            {
                var createdUnit = _unitServ.CreateUnit(request.unit);
                unitId = createdUnit.id;
            }

            var created = _repo.CreateIngredient(foodId, unitId, request.unitAmount, request.recipeId);
            return created;
        }

        public Ingredient? UpdateIngredient(Ingredient request)
        {
            int? foodId = request.foodId;
            if ((foodId == 0) && request.food != null)
            {
                var createdFood = _foodServ.CreateFood(request.food);
                foodId = createdFood.id;
            }

            int? unitId = request.unitId;
            if ((unitId == 0) && request.unit != null)
            {
                var createdUnit = _unitServ.CreateUnit(request.unit);
                unitId = createdUnit.id;
            }

            var updated = _repo.UpdateIngredient(request.id, foodId, unitId, request.unitAmount, request.recipeId);
            return updated;
        }

        public bool DeleteIngredient(int id) => _repo.DeleteIngredient(id);

        public Ingredient FormatIngridient(Ingredient toFormat, FormatMode mode = FormatMode.inspect)
        {
            toFormat.food = _foodServ.GetFoodById(toFormat.foodId);
            toFormat.unit = _unitServ.GetUnitById(toFormat.unitId);
            return toFormat;
        }
    }
}
