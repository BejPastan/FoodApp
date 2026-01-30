namespace FoodApp.Models
{
    public class Meal
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Meal: {name} (ID: {id})";
        }
    }
}
