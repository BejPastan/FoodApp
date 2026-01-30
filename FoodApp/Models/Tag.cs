namespace FoodApp.Models
{
    public class Tag
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Tag {{ id: {id}, name: \"{name}\" }}";
        }
    }
}
