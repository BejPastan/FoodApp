namespace FoodApp.Models
{
    /// <summary>
    /// DTO for kitchen_role table
    /// </summary>
    public class KitchenRole
    {
        /// <summary>
        /// Unique identifier for the kitchen role
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Name of role
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime updated_at { get; set; }
    }

    /// <summary>
    /// Request model for creating a kitchen role
    /// </summary>
    public class KitchenRoleCreateRequest
    {
        /// <summary>
        /// Name of the role
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for updating a kitchen role
    /// </summary>
    public class KitchenRoleUpdateRequest
    {
        /// <summary>
        /// Updated name of the role
        /// </summary>
        public string? Name { get; set; }
    }
}


