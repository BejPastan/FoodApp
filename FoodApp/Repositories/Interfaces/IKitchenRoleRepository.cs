using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for managing kitchen roles
    /// </summary>
    public interface IKitchenRoleRepository
    {
        /// <summary>
        /// Get all available kitchen roles
        /// </summary>
        IEnumerable<KitchenRole> GetAllKitchenRoles();

        /// <summary>
        /// Get kitchen role by id
        /// </summary>
        KitchenRole? GetKitchenRoleById(Guid id);

        /// <summary>
        /// Get kitchen role by name (e.g. 'owner', 'admin', 'editor', 'inspector')
        /// </summary>
        KitchenRole? GetKitchenRoleByName(string name);

        /// <summary>
        /// Create a new kitchen role
        /// </summary>
        KitchenRole CreateKitchenRole(string name);

        /// <summary>
        /// Update an existing kitchen role
        /// </summary>
        KitchenRole? UpdateKitchenRole(Guid id, string name);

        /// <summary>
        /// Delete a kitchen role by id
        /// </summary>
        bool DeleteKitchenRole(Guid id);
    }
}
