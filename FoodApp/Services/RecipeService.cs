using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public interface IRecipeService
    {
        IEnumerable<Recipe> GetRecipes(string nameFilter, int page = 1, int pageSize = 25);
        Recipe? GetRecipeById(int id);
        Recipe? CreateRecipe(Recipe request);
        Recipe? UpdateRecipe(Recipe request);
        bool DeleteRecipe(int id);
        Recipe[] GetRecipeToChoose(int mealId, int userId, int excludedWeeks, int chooseSize);
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

        public IEnumerable<Recipe> GetRecipes(string nameFilter, int page = 1, int pageSize = 25)
        {
            Recipe[] recipes = _recipeRepo.GetRecipes(nameFilter, page, pageSize).ToArray();
            for(int i = 0; i< recipes.Length; i++)
            {
                FormatRecipe(recipes[i]);
            }
            return recipes;
        }

        public Recipe? GetRecipeById(int id)
        {
            var recipe = _recipeRepo.GetRecipeById(id);
            Console.WriteLine();
            if (recipe == null) return null;
            return FormatRecipe(recipe);
        }

        public Recipe? CreateRecipe(Recipe request)
        {
            Recipe created = _recipeRepo.CreateRecipe(request.name);
            int recipeId = created.id;
            foreach(var step in request.steps)
            {
                step.recipeId = recipeId;
                _stepService.CreateStep(step);
            }

            foreach(var ingredient in request.ingredients)
            {
                if(ingredient.id != 0)
                {
                    if(ingredient.recipeId == recipeId)
                    {
                        continue;
                    }
                    else
                    {
                        ingredient.recipeId = recipeId;
                        _ingredientServ.UpdateIngredient(ingredient);
                        continue;
                    }
                }
                ingredient.recipeId = recipeId;
                _ingredientServ.CreateIngredient(ingredient);
            }

            foreach (var meal in request.meals)
            {
                RecipeMeal recipeMeal = new();
                recipeMeal.recipeId = recipeId;
                recipeMeal.mealId = meal.id;

                _recipeMealService.CreateRecipeMeal(recipeMeal);
            }
            
            return FormatRecipe(_recipeRepo.GetRecipeById(recipeId), FormatMode.full);
        }

        public Recipe? UpdateRecipe(Recipe request)
        {
            var recipeToUpdate = _recipeRepo.GetRecipeById(request.id);
            //steps
            int[] stepsToRemove = recipeToUpdate.steps.Where(s => !request.steps.Any(rs => rs.id == s.id)).Select(s => s.id).ToArray();
            Step[] stepsToAdd = recipeToUpdate.steps.Where(s=> s.id == 0).ToArray();
            Step[] stepsToChange = request.steps.Where(s => s.id != 0 && recipeToUpdate.steps.Any(rs => rs.id == s.id)).ToArray();

            foreach(int stepId in stepsToRemove)
            {
                _stepService.DeleteStep(stepId);
            }
            foreach (var step in stepsToAdd)
            {
                step.recipeId = request.id;
                _stepService.CreateStep(step);
            }
            foreach (var step in stepsToChange)
            {
                step.recipeId = request.id;
                _stepService.UpdateStep(step);
            }

            //ingredients
            int[] ingredientsToRemove = recipeToUpdate.ingredients.Where(i => !request.ingredients.Any(ri => ri.id == i.id)).Select(i => i.id).ToArray();
            Ingredient[] ingredientsToAdd = request.ingredients.Where(i => i.id == 0).ToArray();
            Ingredient[] ingredientsToChange = request.ingredients.Where(i => i.id != 0 && recipeToUpdate.ingredients.Any(ri => ri.id == i.id)).ToArray();

            foreach (int ingredientId in ingredientsToRemove)
            {
                _ingredientServ.DeleteIngredient(ingredientId);
            }
            foreach(var ingredient in ingredientsToAdd)
            {
                ingredient.recipeId = request.id;
                _ingredientServ.CreateIngredient(ingredient);
            }
            foreach(var ingredient in ingredientsToChange)
            {
                _ingredientServ.UpdateIngredient(ingredient);
            }

            //meals
            int[] mealToRemove = recipeToUpdate.meals.Where(m=>recipeToUpdate.meals.Any(mId=>mId.id==m.id)).Select(i=> i.id).ToArray();
            Meal[] mealToAdd = request.meals.Where(m => !recipeToUpdate.meals.Any(mId => mId.id == m.id)).ToArray();
            foreach(int mealId in mealToRemove)
            {
                _recipeMealService.DeleteRecipeMeal(request.id, mealId);
            }
            foreach(var meal in mealToAdd)
            {
                RecipeMeal recipeMeal = new();
                recipeMeal.mealId = meal.id;
                recipeMeal.recipeId = request.id;
                _recipeMealService.CreateRecipeMeal(recipeMeal);
            }

            var updated = _recipeRepo.UpdateRecipe(request.id, request.name);

            return FormatRecipe(updated);
        }

        public bool DeleteRecipe(int id)
        {
            return _recipeRepo.DeleteRecipe(id);
        }

        public Recipe[] GetRecipeToChoose(int mealId, int userId, int excludedWeeks, int choosSize)
        {
            int currentExcludedWeeks = excludedWeeks;
            List<Recipe> recipes = new();
            while (recipes.Count <= choosSize)
            {
                recipes = _recipeRepo.GetRecipesToChoose(mealId, userId, new int[0], currentExcludedWeeks).ToList();
                currentExcludedWeeks--;
                if(currentExcludedWeeks < 0) break;
            }

            Recipe[] selected = new Recipe[choosSize];
            Random random = new Random();
            if (recipes.Count > choosSize)
            {
                for(int i = 0; i < choosSize; i++)
                {
                    int index = random.Next(0, recipes.Count);
                    selected[i] = recipes[index];
                    recipes.RemoveAt(index);
                }
            }
            else
            {
                selected = recipes.ToArray();
            }

            foreach(var recipe in selected)
            {
                FormatRecipe(recipe);
            }

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
    }
}
