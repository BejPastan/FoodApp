namespace FoodApp.Models
{
    public class IngredientRecord
    {
        public int id { get; set; }
        public int foodId { get; set; }
        public int unitId { get; set; }
        public decimal unitAmount { get; set; }
        public int recipeId { get; set; }
        public override string ToString()
        {
            return $"Ingredient {{ id = {id}, foodId = {foodId}, unitId = {unitId}, unitAmount = {unitAmount}, recipeId = {recipeId}}}";
        }
    }

    public class Ingredient : IngredientRecord
    {
        public Food? food { get; set; } = null;
        public Unit? unit { get; set; } = null;

        public override string ToString()
        {
            return $"Ingredient {{ id = {id}, foodId = {foodId}, unitId = {unitId}, unitAmount = {unitAmount}, recipeId = {recipeId}, unit = {unit.ToString()}, food = {food.ToString()} }}";
        }
    }

    public class IngredientCreateRequest
    {
        public int? foodId { get; set; } = 0;
        public int? unitId { get; set; } = 0;
        public required decimal unitAmount { get; set; }
        public int? recipeId { get; set; }//this is not requires due to creation of ingredient often happens when creating recipe, and recipeId is not generated until recipe is created
        public CreteFoodRequest? food { get; set; } = null;
        public UnitCreateRequest? unit { get; set; } = null;

    }

    public class IngredientUpdateRequest
    {
        public int? foodId { get; set; } = 0;
        public int? unitId { get; set; } = 0;
        public decimal? unitAmount { get; set; }
        public int? recipeId { get; set; } = 0;
        public CreteFoodRequest? food { get; set; } = null;
        public UnitCreateRequest? unit { get; set; } = null;

    }

    public class ExternalIngredientUpdateRequest:IngredientUpdateRequest
    {
        public required int id { get; set; }
    }
}
