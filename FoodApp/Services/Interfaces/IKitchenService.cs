using FoodApp.Models;

namespace FoodApp.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing Kitchens
    /// </summary>
    public interface IKitchenService
    {
        /// <summary>
        /// Retrieves all kitchens accessible by the user (owned or assigned)
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <returns>List of accessible kitchens</returns>
        IEnumerable<Kitchen> GetKitchensForUser(Guid userId, string name="");

        /// <summary>
        /// Retrieves a kitchen by its ID if the user has access
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="userId">Current user ID</param>
        /// <returns>Kitchen details or null if not found</returns>
        Kitchen? GetKitchenById(Guid kitchenId);

        /// <summary>
        /// Creates a new kitchen and assigns the creator as owner in kitchens_users
        /// </summary>
        /// <param name="request">Kitchen creation data</param>
        /// <param name="ownerId">Creator user ID</param>
        /// <returns>Newly created kitchen</returns>
        Kitchen CreateKitchen(KitchenCreateRequest request, Guid ownerId);

        /// <summary>
        /// Updates kitchen details. Requires at least admin permission on the kitchen.
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="request">Update data</param>
        /// <param name="userId">Current user ID</param>
        /// <returns>Updated kitchen or null</returns>
        Kitchen? UpdateKitchen(Guid kitchenId, KitchenUpdateRequest request);

        /// <summary>
        /// Deletes a kitchen. Only the kitchen owner can delete it.
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="userId">Current user ID</param>
        /// <returns>True if successfully deleted</returns>
        bool DeleteKitchen(Guid kitchenId);
    }
}
