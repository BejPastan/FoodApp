using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    /// <summary>
    /// Service implementation for managing Kitchens
    /// </summary>
    public class KitchenService : IKitchenService
    {
        private readonly IKitchenRepository _kitchenRepo;
        private readonly IKitchenRoleRepository _kitchenRoleRepo;
        private readonly IKitchenUsersRepository _kitchenUsersRepo;

        public KitchenService(
            IKitchenRepository kitchenRepo,
            IKitchenRoleRepository kitchenRoleRepo,
            IKitchenUsersRepository kitchenUsersRepo)
        {
            _kitchenRepo = kitchenRepo;
            _kitchenRoleRepo = kitchenRoleRepo;
            _kitchenUsersRepo = kitchenUsersRepo;
        }

        /// <inheritdoc/>
        public IEnumerable<Kitchen> GetKitchensForUser(Guid userId, string name)
        {
            return _kitchenRepo.GetKitchensByUserId(userId, name);
        }

        /// <inheritdoc/>
        public Kitchen? GetKitchenById(Guid kitchenId)
        {
            return _kitchenRepo.GetKitchenById(kitchenId);
        }

        /// <inheritdoc/>
        public Kitchen CreateKitchen(KitchenCreateRequest request, Guid ownerId)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Kitchen name cannot be empty", nameof(request.Name));
            }

            var kitchen = _kitchenRepo.CreateKitchen(request.Name, ownerId);

            // Automatically assign owner role in kitchens_users
            var ownerRole = _kitchenRoleRepo.GetKitchenRoleByName("owner");
            if (ownerRole != null)
            {
                _kitchenUsersRepo.CreateKitchenUser(ownerId, kitchen.Id, ownerRole.Id);
            }

            return kitchen;
        }

        /// <inheritdoc/>
        public Kitchen? UpdateKitchen(Guid kitchenId, KitchenUpdateRequest request)
        {
            if(request.AccessCode != null && request.AccessCode != "")
            {
                var exists = _kitchenRepo.GetKitchenByAccessCode(request.AccessCode);
                if(exists != null)
                {
                    throw new ArgumentException("Kitchen with this access code already exist, you need to choose another");
                }
            }

            return _kitchenRepo.UpdateKitchen(kitchenId, request.Name, request.AccessCode);
        }

        /// <inheritdoc/>
        public bool DeleteKitchen(Guid kitchenId)
        {
            return _kitchenRepo.DeleteKitchen(kitchenId);
        }
    }
}
