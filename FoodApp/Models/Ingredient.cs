namespace FoodApp.Models
{
    public class IngredientRecord
    {
        public Guid id { get; set; }
        public Guid foodId { get; set; }
        public Guid unitId { get; set; }
        public decimal unitAmount { get; set; }
        public Guid recipeId { get; set; }
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
        public Guid? foodId { get; set; } = Guid.Empty;
        public Guid? unitId { get; set; } = Guid.Empty;
        public required decimal unitAmount { get; set; }
        public Guid? recipeId { get; set; }//this is not requires due to creation of ingredient often happens when creating recipe, and recipeId is not generated until recipe is created
        public CreateFoodRequest? food { get; set; } = null;
        public UnitCreateRequest? unit { get; set; } = null;

    }

    /// <summary>
    /// 
    /// </summary>
    public class IngredientUpdateRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? foodId { get; set; } = Guid.Empty;
        /// <summary>
        /// 
        /// </summary>
        public Guid? unitId { get; set; } = Guid.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal? unitAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? recipeId { get; set; } = Guid.Empty;
        /// <summary>
        /// 
        /// </summary>
        public CreateFoodRequest? food { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public UnitCreateRequest? unit { get; set; } = null;

    }

    public class ExternalIngredientUpdateRequest:IngredientUpdateRequest
    {
        public required Guid id { get; set; }
    }
}
