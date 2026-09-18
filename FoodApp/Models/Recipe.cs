using System;

namespace FoodApp.Models
{
    public class Recipe : RecipeRecord
    {
        public Ingredient[] ingredients { get; set; } = Array.Empty<Ingredient>();
        public Step[] steps { get; set; } = Array.Empty<Step>();
        public Meal[] meals { get; set; } = Array.Empty<Meal>();
        public Tag[] tags { get; set; } = Array.Empty<Tag>();

        public override string ToString()
        {
            string ingredientsStr = "";
            foreach(var ingredient in ingredients)
            {
                ingredientsStr += $"    {ingredient.ToString()}\n";
            }
            string stepsStr = "";
            foreach(var steps in steps)
            {
                stepsStr += $"    {steps.ToString()}\n";
            }
            string mealsStr = "";
            foreach(var meal in meals)
            {
                mealsStr += $"    {meal.ToString()}\n";
            }
            return $"id = {id}, name = {name}, portions = {portion}, preparation time = {time}, ingredients = [\n{ingredientsStr}\n], steps = [\n{stepsStr}\n], meals = [\n{mealsStr}\n]";
        }
    }

    public class RecipeRecord
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public int time { get; set; }
        public int portion { get; set; }
    }

    public class RecipeCreateRequest
    {
        public string name { get; set; } = string.Empty;
        public int time { get; set; }
        public int portion { get; set; }
        public Guid[] ingredientIds { get; set; } = Array.Empty<Guid>();
        public Guid[] mealIds { get; set; } = Array.Empty<Guid>();
        public Guid[] tagIds { get; set; } = Array.Empty<Guid>();
        public IngredientCreateRequest[] ingredients { get; set; } = Array.Empty<IngredientCreateRequest>();
        public StepCreateRequest[] steps { get; set; } = Array.Empty<StepCreateRequest>();
    }

    /// <summary>
    /// mealIds and tagIds are treated as PUT, they are updated to be in state as in request
    /// Ingredients are treated as inputs, they are added, nothing is removed
    /// Steps are treated treated as current state, if they have id, they are updated, if not, they are added to recipe
    /// </summary>
    public class RecipeUpdateRequest
    {
        public string? name { get; set; }
        public int? time { get; set; }
        public int? portion { get; set; }
        public Guid[]? mealIds { get; set; } = [];
        public Guid[] tagIds { get; set; } = Array.Empty<Guid>();
        public IngredientCreateRequest[] ingredients { get; set; } = [];
        public Step[] steps { get; set; } = [];
    }
}
