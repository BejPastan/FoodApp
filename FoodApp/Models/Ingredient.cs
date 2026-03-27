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
}
