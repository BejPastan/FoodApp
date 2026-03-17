using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    public  interface IRoleService
    {
        Role? GetRoleByUserId(int id);
    }

    public class RoleService : IRoleService
    {
        IRoleRepository _roleRepo { get; set; }

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepo = roleRepository;
        }

        public Role? GetRoleByUserId(int id)
        {
            Role? role = _roleRepo.GetRoleByUserId(id);
            return role;
        }

        public bool CheckRole(Roles[] permittedRoles, int userId)
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
    }
}
