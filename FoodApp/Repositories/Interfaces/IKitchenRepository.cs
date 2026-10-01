using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for managing Kitchens
    /// </summary>
    public interface IKitchenRepository
    {
        /// <summary>
        /// Get kitchen by its unique identifier
        /// </summary>
        Kitchen? GetKitchenById(Guid id);

        /// <summary>
        /// find kitchen with given access code
        /// </summary>
        /// <param name="accessCode"></param>
        /// <returns></returns>
        Kitchen? GetKitchenByAccessCode(string accessCode);

        /// <summary>
        /// Get all kitchens where the user is a member (either owner or assigned via kitchens_users)
        /// </summary>
        IEnumerable<Kitchen> GetKitchensByUserId(Guid userId, string name="");

        /// <summary>
        /// Create a new kitchen record
        /// </summary>
        Kitchen CreateKitchen(string name, Guid ownerId, string accessCode = "");

        /// <summary>
        /// Update an existing kitchen record
        /// </summary>
        Kitchen? UpdateKitchen(Guid id, string? name, string? accessCode);

        /// <summary>
        /// Delete a kitchen record by id
        /// </summary>
        bool DeleteKitchen(Guid id);
    }
}
