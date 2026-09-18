namespace FoodApp.Models
{
    public class Meal
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public int order { get; set; }

        public override string ToString()
        {
            return $"Meal: {name} (ID: {id}), order {order}";
        }
    }

    public class MealCreateRequest
    {
        public string name { get; set; } = string.Empty;
        public int order { get; set; }
    }

    public class MealUpdateRequest
    {
        public string? name { get; set; }
        public int? order { get; set; }
    }
}
