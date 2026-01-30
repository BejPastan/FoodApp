namespace FoodApp.Models
{
    public class Food
    {
        public int id { get; set; }
        public required string name { get; set; }
        public int foodTypeId { get; set; }
        public FoodType foodType { get; set; }

        public override string ToString()
        {
            return $"Food: {name} (ID: {id}, TypeID: {foodTypeId})";
        }
    }
}
