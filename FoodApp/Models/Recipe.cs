using System;

namespace FoodApp.Models
{
    public class Recipe
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public Ingredient[] ingredients { get; set; } = Array.Empty<Ingredient>();
        public Step[] steps { get; set; } = Array.Empty<Step>();
        public Meal[] meals { get; set; } = Array.Empty<Meal>();

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
            return $"id = {id}, name = {name}, ingredients = [\n{ingredientsStr}\n], steps = [\n{stepsStr}\n], meals = [\n{mealsStr}\n]";
        }
    }
}
