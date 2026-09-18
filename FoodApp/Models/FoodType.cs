namespace FoodApp.Models
{
    public class FoodType
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"FoodType {{ id = {id}, name = {name} }}";
        }
    }

    public class FoodTypeCreateRequest
    {
        public string name { get; set; } = string.Empty;
    }

    public class FoodTypeUpdateRequest
    {
        public string? name { get; set; }
    }
}