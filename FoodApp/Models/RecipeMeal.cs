namespace FoodApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RecipeMeal
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid recipeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid mealId { get; set; }

        ///<inheritdoc/>
        public override string ToString()
        {
            return $"RecipeMeal {{ id = {id}, recipeId = {recipeId}, mealId = {mealId} }}";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CreateRecipeMealRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid recipeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid mealId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RecipeMealUpdateRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? recipeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? mealId { get; set; }
    }
}
