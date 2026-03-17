using FoodApp.Models;
using FoodApp.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApp.Repositories
{
    public interface IRoleRepository
    {
        Role? GetRoleByUserId(int id);
        bool AddRoleToUser(int  userId, Roles role);
    }

    public class RoleRepository : IRoleRepository
    {
        public bool AddRoleToUser(int userId, Roles role)
        {
            try
            {
                var sql = "INSERT INTO user_roles(user_id, role_id) OUTPUT INSERTED.* SELECT @userId, role.id as roleId FROM role WHERE name = @roleName;";
                DBConnector.QueryDatabase<Role>(sql, new {roleName = role.ToString(), userId = userId});
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("There was error when adding user");
            }

        }

        public Role? GetRoleByUserId(int id)
        {
            var sql = "SELECT role.* FROM role JOIN user_roles ON user_roles.user_id = @userId WHERE role.id = user_roles.role_id;";
            var result = DBConnector.QueryDatabase<Role>(sql, new {userId = id});
            if (result != null) { 
                return result.FirstOrDefault();
            }
            return null;
        }
    }
}
