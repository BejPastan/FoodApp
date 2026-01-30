namespace FoodApp.Models
{
    public class RecipeTag
    {
        public int recipeId { get; set; }
        public int tagId { get; set; }

        public override string ToString()
        {
            return $"RecipeTag {{ recipeId = {recipeId}, tagId = {tagId} }}";
        }
    }
}
