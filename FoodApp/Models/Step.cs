namespace FoodApp.Models
{
    public class Step
    {
        public Guid id { get; set; }
        public Guid recipeId { get; set; }
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
        public Guid? recipeId { get; set; } = Guid.Empty;
        public string instruction { get; set; } = string.Empty;
        public int stepNumber { get; set; } = 0;
    }

    public class StepUpdateRequest
    {
        public Guid? recipeId { get; set; }
        public string? instruction { get; set; }
        public int? stepNumber { get; set; }
    }
}
