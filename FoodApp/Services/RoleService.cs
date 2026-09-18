using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public  interface IRoleService
    {
        Role? GetRoleByUserId(Guid id);
        bool CheckRole(Roles[] permittedRoles, Guid userId);
        public bool AddRoleToUser(Guid userId, Roles role);
    }

    public class RoleService : IRoleService
    {
        IRoleRepository _roleRepo { get; set; }

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepo = roleRepository;
        }

        public Role? GetRoleByUserId(Guid id)
        {
            Role? role = _roleRepo.GetRoleByUserId(id);
            return role;
        }

        public bool CheckRole(Roles[] permittedRoles, Guid userId)
        {
            Role? role = GetRoleByUserId(userId);
            if (role == null)
            {
                return false;
            }
            if (permittedRoles.Contains(role.name))
            {
                return true;
            }
            return false;
        }

        public bool AddRoleToUser(Guid userId, Roles role)
        {
            return _roleRepo.AddRoleToUser(userId, role);
        }
    }
}
