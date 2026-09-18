namespace FoodApp.Models
{
    public class FoodRecord
    {
        public Guid id { get; set; }
        public required string name { get; set; }
        public Guid foodTypeId { get; set; }
    }
    
    public class Food : FoodRecord
    {
        public FoodType foodType { get; set; }

        public override string ToString()
        {
            return $"Food: {name} (ID: {id}, TypeID: {foodTypeId})";
        }
    }

    /// <summary>
    /// Model for creating a new food item. All properties are required, ensuring that a complete food item is created.
    /// </summary>
    public class CreateFoodRequest
    {
        /// <summary>
        /// The name of the food item. This property is required and must be provided when creating a new food item.
        /// </summary>
        public required string name { get; set; }
        /// <summary>
        /// The ID of the food type. This property is required and must be provided when creating a new food item. It links the food item to its corresponding food type.
        /// </summary>
        public Guid? foodTypeId { get; set; } = Guid.Empty;
        /// <summary>
        /// The food type details. This property is required and must be provided when creating a new food item. It allows for the creation of a new food type along with the food item, if necessary.
        /// </summary>
        public FoodTypeCreateRequest? foodType { get; set; }
    }

    /// <summary>
    /// Model for updating a food item. All properties are optional, allowing for partial updates. If a property is not provided, it will not be updated.
    /// </summary>
    public class UpdateFoodRequest
    {
        /// <summary>
        /// The name of the food item. If provided, it will update the existing name of the food item.
        /// </summary>
        public string? name { get; set; }
        /// <summary>
        /// The ID of the food type. If provided, it will update the existing food type of the food item. If not provided, the food type will remain unchanged.
        /// </summary>
        public Guid? foodTypeId { get; set; } = Guid.Empty;
        /// <summary>
        /// The food type details. If provided, it will update the existing food type of the food item. If not provided, the food type will remain unchanged.
        /// </summary>
        public FoodTypeCreateRequest? foodType { get; set; }
    }
}
