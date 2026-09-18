namespace FoodApp.Models
{
    public class RecipeMeal
    {
        public Guid id { get; set; }
        public Guid recipeId { get; set; }
        public Guid mealId { get; set; }

        public override string ToString()
        {
            return $"RecipeMeal {{ id = {id}, recipeId = {recipeId}, mealId = {mealId} }}";
        }
    }

    public class CreateRecipeMealRequest
    {
        public Guid recipeId { get; set; }
        public Guid mealId { get; set; }
    }

    public class RecipeMealUpdateRequest
    {
        public Guid? recipeId { get; set; }
        public Guid? mealId { get; set; }
    }
}
