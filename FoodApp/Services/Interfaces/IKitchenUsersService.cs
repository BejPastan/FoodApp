using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Services.Interfaces
{
    /// <summary>
    /// Interface to manage kitchens users and permissions
    /// </summary>
    public interface IKitchenUsersService
    {
        /// <summary>
        /// Get all users belonging to a kitchen with their role names. Caller must have access.
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="requestingUserId">Current authenticated user ID</param>
        /// <returns>List of kitchen users with details</returns>
        public IEnumerable<KitchenUserDetailResponse> GetKitchenUsers(Guid kitchenId, Guid userId, Guid kitchenUserId);

        /// <summary>
        /// Add new user to kitchen. Caller must have admin or owner permission.
        /// </summary>
        /// <param name="userId">Target user ID to add</param>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="role">Role to assign (default inspector)</param>
        /// <param name="requestingUserId">Optional requesting user ID for permission check</param>
        /// <returns>Created membership record</returns>
        public KitchenUserDetailResponse JoinKitchen(Guid userId, string accessCode);

        /// <summary>
        /// Check if user has required or higher role in the kitchen
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="minimalRole">Minimum required role</param>
        /// <returns>True if permitted</returns>
        public bool CheckPermission(Guid userId, Guid kitchenId, KitchenRoles minimalRole);

        /// <summary>
        /// Remove user access to kitchen. Caller must be admin, owner, or self.
        /// </summary>
        /// <param name="userId">Target user ID</param>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="requestingUserId">Optional requesting user ID for permission check</param>
        /// <returns>True if removed</returns>
        public bool RemoveUserFromKitchen(Guid userId, Guid kitchenId);

        /// <summary>
        /// Change role of user in a kitchen. Caller must be admin or owner.
        /// </summary>
        /// <param name="userId">Target user ID</param>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="newRole">New role to assign</param>
        /// <param name="requestingUserId">Optional requesting user ID for permission check</param>
        /// <returns>Updated membership record</returns>
        public KitchenUserDetailResponse ChangeRole(Guid userId, Guid kitchenId, KitchenRoles newRole);
    }
}

