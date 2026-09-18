namespace FoodApp.Models
{
    public class RecipeTag
    {
        public Guid recipeId { get; set; }
        public Guid tagId { get; set; }

        public override string ToString()
        {
            return $"RecipeTag {{ recipeId = {recipeId}, tagId = {tagId} }}";
        }
    }

    public class RecipeTagCreateRequest
    {
        public Guid recipeId { get; set; }
        public Guid tagId { get; set; }
    }

    public class RecipeTagUpdateRequest
    {
        public Guid? recipeId { get; set; }
        public Guid? tagId { get; set; }
    }
}
