namespace FoodApp.Models
{
    public class FoodRecord
    {
        public int id { get; set; }
        public required string name { get; set; }
        public int foodTypeId { get; set; }
    }
    
    public class Food : FoodRecord
    {
        public FoodType foodType { get; set; }

        public override string ToString()
        {
            return $"Food: {name} (ID: {id}, TypeID: {foodTypeId})";
        }
    }

    public class CreteFoodRequest
    {
        
    }

    public class UpdateFoodRequest
    {
        
    }
}
