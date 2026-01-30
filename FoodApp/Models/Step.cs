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
}
