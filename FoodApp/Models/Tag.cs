namespace FoodApp.Models
{
    public class Tag
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Tag {{ id: {id}, name: \"{name}\" }}";
        }
    }

    public class TagCreateRequest
    {
        public string name { get; set; } = string.Empty;
    }

    public class TagUpdateRequest
    {
        public string? name { get; set; }
    }
}
