namespace FoodApp.Models
{
    public class Unit
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public decimal volumeEquivalent { get; set; }

        public override string ToString()
        {
            return $"{name} (id: {id}, volumeEquivalent: {volumeEquivalent})";
        }
    }
}
