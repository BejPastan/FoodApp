using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;

namespace FoodApp.Services
{
    /// <summary>
    /// Service implementation for managing kitchen roles
    /// </summary>
    public class KitchenRoleService : IKitchenRoleService
    {
        private readonly IKitchenRoleRepository _repo;

        public KitchenRoleService(IKitchenRoleRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc/>
        public IEnumerable<KitchenRole> GetAllRoles()
        {
            return _repo.GetAllKitchenRoles();
        }

        /// <inheritdoc/>
        public KitchenRole? GetRoleById(Guid id)
        {
            return _repo.GetKitchenRoleById(id);
        }

        /// <inheritdoc/>
        public KitchenRole? GetRoleByName(string name)
        {
            return _repo.GetKitchenRoleByName(name);
        }

        /// <inheritdoc/>
        public KitchenRole CreateRole(KitchenRoleCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(request.Name));
            }

            var existing = _repo.GetKitchenRoleByName(request.Name);
            if (existing != null)
            {
                throw new ArgumentException($"Role '{request.Name}' already exists");
            }

            return _repo.CreateKitchenRole(request.Name.ToLowerInvariant());
        }

        /// <inheritdoc/>
        public KitchenRole? UpdateRole(Guid id, KitchenRoleUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Role name cannot be empty", nameof(request.Name));
            }

            return _repo.UpdateKitchenRole(id, request.Name.ToLowerInvariant());
        }

        /// <inheritdoc/>
        public bool DeleteRole(Guid id)
        {
            return _repo.DeleteKitchenRole(id);
        }
    }
}
