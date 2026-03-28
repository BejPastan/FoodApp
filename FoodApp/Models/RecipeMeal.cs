namespace FoodApp.Models
{
    public class RecipeMeal
    {
        public int id { get; set; }
        public int recipeId { get; set; }
        public int mealId { get; set; }

        public override string ToString()
        {
            return $"RecipeMeal {{ id = {id}, recipeId = {recipeId}, mealId = {mealId} }}";
        }
    }

    public class CreateRecipeMeal
    {
        public int recipeId { get; set; }
        public int mealId { get; set; }
    }
}
