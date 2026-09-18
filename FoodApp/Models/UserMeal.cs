namespace FoodApp.Models
{
    /// <summary>
    /// DTO of user_meal table
    /// </summary>
    public class UserMeal
    {
        /// <summary>
        /// id of user_meal record
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// id of user
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// id of recipe
        /// </summary>
        public Guid recipeId { get; set; }
        /// <summary>
        /// id of meal
        /// </summary>
        public Guid mealId { get; set; }
        /// <summary>
        /// date of meal
        /// </summary>
        public DateOnly mealDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"UserMeal {{ id = {id}, userId = {userId}, recipeId = {recipeId}, mealId = {mealId}, mealDate = {mealDate} }}";
        }
    }

    /// <summary>
    /// Model with data for user meal list
    /// </summary>
    public class UserMealWithData
    {

        /// <summary>
        /// record id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// recipe Id
        /// </summary>
        public Guid recipeId { get; set; }//recipeId
        /// <summary>
        /// recipe name
        /// </summary>
        public string name { get; set; } = "";
        /// <summary>
        /// preparing time in minutes
        /// </summary>
        public int time { get; set; }
        /// <summary>
        /// number of portions
        /// </summary>
        public int portion { get; set; }
        /// <summary>
        /// name of meal
        /// </summary>
        public string meal { get; set; } = "";
        /// <summary>
        /// date of meal
        /// </summary>
        public DateOnly mealDate { get; set; }

        public override string ToString()
        {
            return $"User meal with data racipeId = {recipeId}, recipe name = {name}, time={time}, portions= {portion}, meal= {meal}, meal Day= {mealDate}";
        }
    }

    public class UserMealCreateRequest
    {
        public Guid recipeId { get; set; }
        public Guid mealId { get; set; }
        public DateOnly mealDate { get; set; }
    }

    public class UserMealUpdateRequest
    {
        public Guid userMealId { get; set; }
        public Guid recipeId { get; set; }
    }
}
