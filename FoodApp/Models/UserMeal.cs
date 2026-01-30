namespace FoodApp.Models
{
    public class UserMeal
    {
        public int id { get; set; }
        public int userId { get; set; }//userId
        public int recipeId { get; set; }//recipeId
        public int mealId { get; set; }//mealId
        public DateTime mealDate { get; set; }

        public override string ToString()
        {
            return $"UserMeal {{ id = {id}, userId = {userId}, recipeId = {recipeId}, mealId = {mealId}, mealDate = {mealDate} }}";
        }
    }
}
