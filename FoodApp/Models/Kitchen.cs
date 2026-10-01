namespace FoodApp.Models
{
    /// <summary>
    /// DTO for Kitchen table
    /// </summary>
    public class Kitchen
    {
        /// <summary>
        /// Unique identifier for the kitchen
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the kitchen
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Owner user ID of the kitchen
        /// </summary>
        public Guid ownerId { get; set; }
        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime updated_at { get; set; }

        /// <summary>
        /// code which is required to give when one want to join kitchen, default to empty string, when string is empty, no one cannot join kitchen
        /// </summary>
        public string accessCode { get; set; } = "";
    }

    /// <summary>
    /// Request model for creating a kitchen
    /// </summary>
    public class KitchenCreateRequest
    {
        /// <summary>
        /// Name of the kitchen
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string AccessCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for updating a kitchen
    /// </summary>
    public class KitchenUpdateRequest
    {
        /// <summary>
        /// New name of the kitchen
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? AccessCode { get; set; } = null;
    }
}


