using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for managing kitchens_users relations
    /// </summary>
    public interface IKitchenUsersRepository
    {
        /// <summary>
        /// Get all membership records for a specific kitchen
        /// </summary>
        IEnumerable<KitchenUserDetailResponse> GetKitchenUsers(Guid kitchenId, Guid userId, Guid kitchenUserId, string userName);

        /// <summary>
        /// Add a user to a kitchen with a specified role
        /// </summary>
        KitchenUsers CreateKitchenUser(Guid userId, Guid kitchenId, Guid roleId);

        /// <summary>
        /// Update user's role in a kitchen
        /// </summary>
        KitchenUsers? UpdateKitchenUserRole(Guid userId, Guid kitchenId, Guid roleId);

        /// <summary>
        /// Remove user membership from a kitchen
        /// </summary>
        bool DeleteKitchenUser(Guid userId, Guid kitchenId);

        /// <summary>
        /// Remove user membership by id
        /// </summary>
        bool DeleteKitchenUserById(Guid id);
    }
}
