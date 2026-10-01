using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    /// <summary>
    /// Service to manage kitchen users and check kitchen roles and permissions
    /// </summary>
    public class KitchenUserService : IKitchenUsersService
    {
        private readonly IKitchenUsersRepository _kitchenUsersRepo;
        private readonly IKitchenRoleRepository _kitchenRoleRepo;
        private readonly IKitchenRepository _kitchenRepo;
        private readonly ILogger<KitchenUserService> _logger;

        public KitchenUserService(
            IKitchenUsersRepository kitchenUsersRepo,
            IKitchenRoleRepository kitchenRoleRepo,
            IKitchenRepository kitchenRepo,
            ILogger<KitchenUserService> logger
            )
        {
            _kitchenUsersRepo = kitchenUsersRepo;
            _kitchenRoleRepo = kitchenRoleRepo;
            _kitchenRepo = kitchenRepo;
            _logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<KitchenUserDetailResponse> GetKitchenUsers(Guid kitchenId, Guid userId, Guid kitchenUserId)
        {
            var members = _kitchenUsersRepo.GetKitchenUsers(kitchenId, userId, kitchenUserId, "").ToList();
            return members;
        }

        /// <inheritdoc/>
        public KitchenUserDetailResponse JoinKitchen(Guid userId, string accessCode)
        {
            var kitchen = _kitchenRepo.GetKitchenByAccessCode(accessCode);
            if (kitchen == null)
            {
                throw new InvalidOperationException("there is no kitchen with such code");
            }
            var kitchenId = kitchen.Id;
            var existing = _kitchenUsersRepo.GetKitchenUsers(kitchenId, userId, Guid.Empty, "").FirstOrDefault();
            if (existing != null)
            {
                return existing;
            }

            var roleRecord = _kitchenRoleRepo.GetKitchenRoleByName(KitchenRoles.inspector.ToString());

            var kitchenUser =  _kitchenUsersRepo.CreateKitchenUser(userId, kitchenId, roleRecord.Id);
            return _kitchenUsersRepo.GetKitchenUsers(Guid.Empty, Guid.Empty, kitchenUser.Id, "").FirstOrDefault();
        }

        /// <inheritdoc/>
        public KitchenUserDetailResponse ChangeRole(Guid userId, Guid kitchenId, KitchenRoles newRole)
        {
            var role = _kitchenRoleRepo.GetKitchenRoleByName(newRole.ToString());
            if (role == null)
            {
                throw new Exception($"Role '{newRole}' not found in the database.");
            }

            var updated = _kitchenUsersRepo.UpdateKitchenUserRole(userId, kitchenId, role.Id);
            return _kitchenUsersRepo.GetKitchenUsers(Guid.Empty, Guid.Empty, updated.Id, "").FirstOrDefault();

        }

        /// <inheritdoc/>
        public bool CheckPermission(Guid userId, Guid kitchenId, KitchenRoles minimalRole)
        {
            // 1. Check if user is the direct owner of the kitchen
            var kitchen = _kitchenRepo.GetKitchenById(kitchenId);
            if (kitchen != null && kitchen.ownerId == userId)
            {
                return true;
            }

            // 2. Retrieve user's role assignment in the kitchen
            var kitchenUser = _kitchenUsersRepo.GetKitchenUsers(kitchenId, userId, Guid.Empty, "").FirstOrDefault();
            if (kitchenUser == null)
            {
                return false;
            }

            var role = _kitchenRoleRepo.GetKitchenRoleById(kitchenUser.RoleId);
            if (role == null)
            {
                return false;
            }

            if (!Enum.TryParse<KitchenRoles>(role.Name, true, out var userRole))
            {
                return false;
            }

            return GetRoleRank(userRole) >= GetRoleRank(minimalRole);
        }

        /// <inheritdoc/>
        public bool RemoveUserFromKitchen(Guid userId, Guid kitchenId)
        {
            var kitchen = _kitchenRepo.GetKitchenById(kitchenId);
            if (kitchen != null && kitchen.ownerId == userId)
            {
                throw new InvalidOperationException("The owner of the kitchen cannot be removed from it");
            }

            return _kitchenUsersRepo.DeleteKitchenUser(userId, kitchenId);
        }

        /// <summary>
        /// Maps KitchenRoles enum to numeric priority rank.
        /// Higher number indicates higher permissions.
        /// </summary>
        private static int GetRoleRank(KitchenRoles role)
        {
            return role switch
            {
                KitchenRoles.owner => 4,
                KitchenRoles.admin => 3,
                KitchenRoles.editor => 2,
                KitchenRoles.inspector => 1,
                _ => 0
            };
        }
    }
}

