namespace FoodApp.Models
{
    public class Step
    {
        public int id { get; set; }
        public int recipeId { get; set; }
        public string instruction { get; set; } = string.Empty;
        public int stepNumber { get; set; } = 0;

        public override string ToString()
        {
            return $"id = {id}, recipeId = {recipeId}, instruction = {instruction}, stepNumber = {stepNumber}";
        }
    }


    /// <summary>
    /// recipe id is not required beacause we often create step when creting recipe
    /// </summary>
    public class StepCreateRequest
    {
        public int? recipeId { get; set; } = 0;
        public string instruction { get; set; } = string.Empty;
        public int stepNumber { get; set; } = 0;
    }

    public class StepUpdateRequest
    {
        public int? recipeId { get; set; }
        public string? instruction { get; set; }
        public int? stepNumber { get; set; }
    }
}
