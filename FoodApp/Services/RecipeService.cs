using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public interface IRecipeService
    {
        IEnumerable<RecipeRecord> GetRecipes(string nameFilter, int page = 1, int pageSize = 25);
        Recipe? GetRecipeById(int id);
        Recipe? CreateRecipe(RecipeCreateRequest request);
        Recipe? UpdateRecipe(int recipeId, RecipeUpdateRequest request);
        bool DeleteRecipe(int id);
        RecipeRecord[] GetRecipeToChoose(int mealId, int userId, int excludedWeeks, int chooseSize);
    }

    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IMealService _mealService;
        private readonly IStepService _stepService;
        private readonly IIngredientService _ingredientServ;
        private readonly IRecipeMealService _recipeMealService;

        public RecipeService(IRecipeRepository recipeRepo, IMealService mealService, IStepService stepService, IIngredientService ingredientServ, IRecipeMealService recipeMealService)
        {
            _recipeRepo = recipeRepo;
            _mealService = mealService;
            _stepService = stepService;
            _ingredientServ = ingredientServ;
            _recipeMealService = recipeMealService;
        }

        public IEnumerable<RecipeRecord> GetRecipes(string nameFilter, int page = 1, int pageSize = 25)
        {
            RecipeRecord[] recipes = _recipeRepo.GetRecipes(nameFilter, page, pageSize).ToArray();
            return recipes;
        }

        public Recipe? GetRecipeById(int id)
        {
            var recipe = _recipeRepo.GetRecipeById(id);
            if (recipe == null) return null;
            return FormatRecipe(recipe);
        }

        public Recipe? CreateRecipe(RecipeCreateRequest request)
        {
            Recipe created = _recipeRepo.CreateRecipe(request.name, request.portion, request.time);
            int recipeId = created.id;
            foreach(var step in request.steps)
            {
                step.recipeId = recipeId;
                _stepService.CreateStep(step);
            }

            foreach(var ingredient in request.ingredients)
            {
                ingredient.recipeId = recipeId;
                _ingredientServ.CreateIngredient(ingredient);
            }

            foreach (var mealIds in request.mealIds)
            {
                CreateRecipeMealRequest recipeMeal = new CreateRecipeMealRequest
                {
                    mealId = mealIds,
                    recipeId = recipeId
                };

                _recipeMealService.CreateRecipeMeal(recipeMeal);
            }
            
            return FormatRecipe(_recipeRepo.GetRecipeById(recipeId), FormatMode.full);
        }

        public Recipe? UpdateRecipe(int recipeId, RecipeUpdateRequest request)
        {
            foreach (var step in request.steps)
            {
                step.recipeId = recipeId;
                _stepService.CreateStep(step);
            }

            foreach(var ingredient in request.ingredients)
            {
                ingredient.recipeId = recipeId;
                _ingredientServ.CreateIngredient(ingredient);
            }


            foreach(var meal in request.mealIds)
            {
                CreateRecipeMealRequest recipeMeal = new();
                recipeMeal.mealId = meal;
                recipeMeal.recipeId = recipeId;
                _recipeMealService.CreateRecipeMeal(recipeMeal);
            }

            var updated = _recipeRepo.UpdateRecipe(recipeId, request.name, request.portion, request.time);

            return FormatRecipe(updated);
        }

        public bool DeleteRecipe(int id)
        {
            return _recipeRepo.DeleteRecipe(id);
        }

        public RecipeRecord[] GetRecipeToChoose(int mealId, int userId, int excludedWeeks, int choosSize)
        {
            int currentExcludedWeeks = excludedWeeks;
            List<RecipeRecord> recipes = _recipeRepo.GetRecipesToChoose(userId, mealId, currentExcludedWeeks).ToList();
            Console.WriteLine($"recipe count: {recipes.Count}");
            return recipes.ToArray();
        }

        private Recipe? FormatRecipe(Recipe? toFormat, FormatMode mode=FormatMode.inspect)
        {
            if (toFormat == null) return toFormat;

            switch (mode)
            {
                case FormatMode.inspect:
                    {
                        toFormat.meals = _mealService.GetMeals(null, toFormat.id).ToArray();
                        toFormat.steps = _stepService.GetSteps(toFormat.id).ToArray();
                        toFormat.ingredients = _ingredientServ.GetIngredients(toFormat.id, null).ToArray();
                        break;
                    }
                case FormatMode.full:
                    {
                        toFormat.meals = _mealService.GetMeals(null, toFormat.id).ToArray();
                        toFormat.steps = _stepService.GetSteps(toFormat.id).ToArray();
                        toFormat.ingredients = _ingredientServ.GetIngredients(toFormat.id, null).ToArray();
                        break;
                    }
            }
            return toFormat;
        }

        private Recipe? FormatRecipe(RecipeRecord? toFormat, FormatMode mode = FormatMode.inspect)
        {
            if (toFormat == null) return null;
            Recipe toReturn = new Recipe
            {
                id = toFormat.id,
                name = toFormat.name,
                portion = toFormat.portion,
                time = toFormat.time
            };
            return FormatRecipe(toReturn, mode);
        }
    }
}
