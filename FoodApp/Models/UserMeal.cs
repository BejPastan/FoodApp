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

    public class UserMealWithData
    {
        public int recipeId { get; set; }//recipeId
        public string name { get; set; }
        public int time { get; set; }
        public int portion { get; set; }
        public string meal { get; set; }
        public DateTime mealDate { get; set; }

        public override string ToString()
        {
            return $"User meal with data racipeId = {recipeId}, recipe name = {name}, time={time}, portions= {portion}, meal= {meal}, meal Day= {mealDate}";
        }
    }

    public class UserMealCreateRequest
    {
        public int recipeId { get; set; }
        public int mealId { get; set; }
        public DateTime mealDate { get; set; }
    }

    public class UserMealUpdateRequest
    {
        public int? recipeId { get; set; }
        public int? mealId { get; set; }
        public DateTime? mealDate { get; set; }
    }
}
