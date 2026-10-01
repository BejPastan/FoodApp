using FoodApp.Utilities;

namespace FoodApp.Models
{
    /// <summary>
    /// DTO for kitchens_users table
    /// </summary>
    public class KitchenUsers
    {
        /// <summary>
        /// Unique identifier for membership
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// ID of the kitchen
        /// </summary>
        public Guid KitchenId { get; set; }
        /// <summary>
        /// ID of the user
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// ID of the assigned role
        /// </summary>
        public Guid RoleId { get; set; }
        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime updated_at { get; set; }
    }

    /// <summary>
    /// Request model for adding a user to a kitchen
    /// </summary>
    public class KitchenUserAddRequest
    {
        /// <summary>
        /// User ID to add to kitchen
        /// </summary>
        public Guid UserId { get; set; }
        public string AccessCode { get; set; }
    }

    /// <summary>
    /// Request model for updating user role in a kitchen
    /// </summary>
    public class KitchenUserRoleUpdateRequest
    {
        /// <summary>
        /// New role to assign to the user
        /// </summary>
        public KitchenRoles Role { get; set; }
    }

    /// <summary>
    /// Detailed response model for kitchen membership with role name
    /// </summary>
    public class KitchenUserDetailResponse : KitchenUsers
    {
        /// <summary>
        /// Name of the role
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
    }
}


