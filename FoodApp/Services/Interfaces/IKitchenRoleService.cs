using FoodApp.Models;

namespace FoodApp.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing kitchen roles
    /// </summary>
    public interface IKitchenRoleService
    {
        /// <summary>
        /// Get all available kitchen roles
        /// </summary>
        /// <returns>List of kitchen roles</returns>
        IEnumerable<KitchenRole> GetAllRoles();

        /// <summary>
        /// Get kitchen role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Kitchen role or null</returns>
        KitchenRole? GetRoleById(Guid id);

        /// <summary>
        /// Get kitchen role by name
        /// </summary>
        /// <param name="name">Role name</param>
        /// <returns>Kitchen role or null</returns>
        KitchenRole? GetRoleByName(string name);

        /// <summary>
        /// Create a new kitchen role
        /// </summary>
        /// <param name="request">Creation request</param>
        /// <returns>Created kitchen role</returns>
        KitchenRole CreateRole(KitchenRoleCreateRequest request);

        /// <summary>
        /// Update an existing kitchen role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated kitchen role or null</returns>
        KitchenRole? UpdateRole(Guid id, KitchenRoleUpdateRequest request);

        /// <summary>
        /// Delete a kitchen role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>True if deleted</returns>
        bool DeleteRole(Guid id);
    }
}
