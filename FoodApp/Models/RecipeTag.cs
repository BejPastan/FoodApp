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

    public class RecipeTagCreateRequest
    {
        public int recipeId { get; set; }
        public int tagId { get; set; }
    }

    public class RecipeTagUpdateRequest
    {
        public int? recipeId { get; set; }
        public int? tagId { get; set; }
    }
}
