using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public class RecipeService(IRecipeRepo recipeRepo, IMealService mealService, IStepService stepService, IIngredientService ingredientServ, IRecipeMealService recipeMealService, IRecipeTagService recipeTagService, ITagService tagService) : IRecipeService
    {
        private readonly IRecipeRepo _recipeRepo = recipeRepo;
        private readonly IMealService _mealService = mealService;
        private readonly IStepService _stepService = stepService;
        private readonly IIngredientService _ingredientServ = ingredientServ;
        private readonly IRecipeMealService _recipeMealService = recipeMealService;
        private readonly IRecipeTagService _recipeTagService = recipeTagService;
        private readonly ITagService _tagService = tagService;

        public IEnumerable<RecipeRecord> GetRecipes(string[]? mealNames, string[]? tags, string nameFilter, int page = 1, int pageSize = 25)
        {
            RecipeRecord[] recipes = _recipeRepo.GetRecipes(mealNames, tags, nameFilter, page, pageSize).ToArray();
            return recipes;
        }

        public Recipe? GetRecipeById(Guid id)
        {
            var recipe = _recipeRepo.GetRecipeById(id);
            if (recipe == null) return null;
            return FormatRecipe(recipe, FormatMode.full);
        }

        public Recipe? CreateRecipe(RecipeCreateRequest request)
        {
            Recipe created = _recipeRepo.CreateRecipe(request.name, request.portion, request.time);
            Guid recipeId = created.id;
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

        public Recipe? UpdateRecipe(Guid recipeId, RecipeUpdateRequest request)
        {
            Console.WriteLine($"Updating recipe {recipeId} with name: {request.name}, portion: {request.portion}, time: {request.time}");
            // Handle steps - update existing ones or create new ones
            foreach (var step in request.steps)
            {
                step.recipeId = recipeId;
                if (step.id != Guid.Empty)
                {
                    // Update existing step
                    StepUpdateRequest stepUpdate = new StepUpdateRequest
                    {
                        recipeId = recipeId,
                        instruction = step.instruction,
                        stepNumber = step.stepNumber
                    };
                    _stepService.UpdateStep(step.id, stepUpdate);
                }
                else
                {
                    // Create new step
                    StepCreateRequest stepCreate = new StepCreateRequest
                    {
                        recipeId = recipeId,
                        instruction = step.instruction,
                        stepNumber = step.stepNumber
                    };
                    _stepService.CreateStep(stepCreate);
                }
            }

            // Handle ingredients - add new ones (nothing removed)
            foreach(var ingredient in request.ingredients)
            {
                ingredient.recipeId = recipeId;
                _ingredientServ.CreateIngredient(ingredient);
            }

            // Handle mealIds - PUT behavior: delete existing, create new
            if (request.mealIds != null)
            {
                _recipeMealService.DeleteRecipesMeal(recipeId);
                //Add all meals
                foreach (var mealId in request.mealIds)
                {
                    CreateRecipeMealRequest recipeMeal = new CreateRecipeMealRequest
                    {
                        mealId = mealId,
                        recipeId = recipeId
                    };
                    _recipeMealService.CreateRecipeMeal(recipeMeal);
                }

            }

            // Handle tagIds - PUT behavior: delete existing, create new
            Console.WriteLine(request.tagIds==null);
            if (request.tagIds != null)
            {
                _recipeTagService.DeleteRecipeTags(recipeId);
                foreach (var tagId in request.tagIds)
                {
                    RecipeTagCreateRequest recipeTag = new RecipeTagCreateRequest
                    {
                        recipeId = recipeId,
                        tagId = tagId
                    };
                    _recipeTagService.CreateRecipeTag(recipeTag);
                }
            }

            var updated = _recipeRepo.UpdateRecipe(recipeId, request.name, request.portion, request.time);

            Console.WriteLine($"Updated recipe: {updated}");

            return FormatRecipe(updated);
        }

        public bool DeleteRecipe(Guid id)
        {
            return _recipeRepo.DeleteRecipe(id);
        }

        public RecipeRecord[] GetRecipeToChoose(Guid mealId, Guid userId, int excludedWeeks, int choosSize)
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
                        toFormat.ingredients = _ingredientServ.GetIngredients(toFormat.id, null).ToArray();
                        toFormat.tags = _tagService.GetTags(null, toFormat.id).ToArray();
                        break;
                    }
                case FormatMode.full:
                    {
                        Console.WriteLine("formatting full");
                        toFormat.meals = _mealService.GetMeals(null, toFormat.id).ToArray();
                        toFormat.steps = _stepService.GetSteps(toFormat.id).ToArray();
                        toFormat.ingredients = _ingredientServ.GetIngredients(toFormat.id, null).ToArray();
                        toFormat.tags = _tagService.GetTags(null, toFormat.id).ToArray();
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
                time = toFormat.time,
            };
            return toReturn;
        }
    }
}
