using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// Default implementation of IKitchenUsersRepository
    /// </summary>
    public class KitchenUsersRepository : IKitchenUsersRepository
    {
        /// <inheritdoc/>
        public IEnumerable<KitchenUserDetailResponse> GetKitchenUsers(Guid kitchenId, Guid userId, Guid kitchenUserId, string userName)
        {
            var sql = "SELECT ku.id AS Id, ku.kitchenId as KitchenId, ku.roleId as RoleId, r.name as RoleName, u.name AS UserName, u.id AS userId FROM kitchens_users ku JOIN users u ON u.id = ku.userId JOIN Kitchen_role r ON r.id = ku.roleId WHERE u.name LIKE @userName";
            if(kitchenId != Guid.Empty)
            {
                sql += " AND ku.kitchenId = @kitchenId ";
            }
            if(userId != Guid.Empty)
            {
                sql += " AND ku.userId = @userId ";
            }
            if(kitchenUserId != Guid.Empty)
            {
                sql += " AND ku.id = @kitchenUserId ";
            }
            sql += ";";
            return DBConnector.QueryDatabase<KitchenUserDetailResponse>(sql, new { kitchenId, userId, kitchenUserId, userName = $"%{userName}%" });
        }

        /// <inheritdoc/>
        public KitchenUsers CreateKitchenUser(Guid userId, Guid kitchenId, Guid roleId)
        {
            var sql = """
                INSERT INTO kitchens_users (userId, kitchenId, roleId) 
                OUTPUT INSERTED.* 
                VALUES (@userId, @kitchenId, @roleId);
                """;
            var created = DBConnector.QueryDatabase<KitchenUsers>(sql, new { userId, kitchenId, roleId }).ToList();
            if (created.Count > 0)
            {
                return created[0];
            }
            throw new Exception("Insert failed for kitchens_users");
        }

        /// <inheritdoc/>
        public KitchenUsers? UpdateKitchenUserRole(Guid userId, Guid kitchenId, Guid roleId)
        {
            var sql = """
                UPDATE kitchens_users 
                SET role_id = @roleId
                OUTPUT INSERTED.* 
                WHERE user_id = @userId AND kitchen_id = @kitchenId;
                """;
            return DBConnector.QueryDatabase<KitchenUsers>(sql, new { userId, kitchenId, roleId }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool DeleteKitchenUser(Guid userId, Guid kitchenId)
        {
            var sql = "DELETE FROM kitchens_users WHERE userId = @userId AND kitchenId = @kitchenId;";
            DBConnector.QueryDatabase<Guid>(sql, new { userId, kitchenId }).ToList();
            return true;
        }

        /// <inheritdoc/>
        public bool DeleteKitchenUserById(Guid id)
        {
            var sql = "DELETE FROM kitchens_users WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id }).ToList();
            return true;
        }
    }
}
