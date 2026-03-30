using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public interface IIngredientService
    {
        IEnumerable<Ingredient> GetIngredients(int? recipeId, int? foodId, int page = 1, int perPage = 25);
        Ingredient? GetIngredientById(int id);
        Ingredient CreateIngredient(IngredientCreateRequest request);
        Ingredient? UpdateIngredient(int id, IngredientUpdateRequest request);
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

        public Ingredient CreateIngredient(IngredientCreateRequest request)
        {
            if(!request.recipeId.HasValue)
            {
                throw new ArgumentException("recipeId is required to create an ingredient");
            }

            // Ensure food exists
            int foodId = 0;
            if ((!request.foodId.HasValue || request.foodId.Value<=0) && request.food != null)
            {
                var createdFood = _foodServ.CreateFood(request.food);
                foodId = createdFood.id;
            }else if(request.foodId.HasValue && request.foodId.Value > 0)
            {
                foodId = request.foodId.Value;
            }
            else
            {
                throw new ArgumentException("Either foodId must be provided and greater than 0, or food details must be provided to create a new food.");
            }

            // Ensure unit exists
            int unitId = 0;
            if ((!request.unitId.HasValue || request.unitId <= 0) && request.unit != null)
            {
                var createdUnit = _unitServ.CreateUnit(request.unit);
                unitId = createdUnit.id;
            }
            else if (request.unitId.HasValue && request.unitId.Value > 0)
            {
                unitId = request.unitId.Value;
            }
            else
            {
                throw new ArgumentException("Either unitId must be provided and greater than 0, or unit details must be provided to create a new unit.");
            }

            var created = _repo.CreateIngredient(foodId, unitId, request.unitAmount, request.recipeId.Value);
            return created;
        }

        /// <summary>
        /// Update ingredient record, based on request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">happen when request contain both id and crete request for food or unit</exception>
        public Ingredient? UpdateIngredient(int id, IngredientUpdateRequest request)
        {
            int? foodId = request.foodId;
            int? unitId = request.unitId;
            if ((foodId.HasValue && foodId.Value >= 0) && request.food != null)
            {
                throw new ArgumentException("You could provide only food id or food create request");
            }

            if ((unitId.HasValue && unitId.Value <= 0) && request.unit != null)
            {
                throw new ArgumentException("You could provide only unit id or unit create request");
            }

            if ((!foodId.HasValue || foodId.Value <= 0) && request.food != null)
            {
                var createdFood = _foodServ.CreateFood(request.food);
                foodId = createdFood.id;
            }


            if ((!unitId.HasValue || unitId.Value <= 0) && request.unit != null)
            {
                var createdUnit = _unitServ.CreateUnit(request.unit);
                unitId = createdUnit.id;
            }


            var updated = _repo.UpdateIngredient(id, foodId, unitId, request.unitAmount, request.recipeId);
            return updated;
        }

        /// <summary>
        /// Delete ingredient record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteIngredient(int id) => _repo.DeleteIngredient(id);

        public Ingredient FormatIngridient(Ingredient toFormat, FormatMode mode = FormatMode.inspect)
        {
            toFormat.food = _foodServ.GetFoodById(toFormat.foodId);
            toFormat.unit = _unitServ.GetUnitById(toFormat.unitId);
            return toFormat;
        }
    }
}
