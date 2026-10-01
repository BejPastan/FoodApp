using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// Default implementation of IKitchenRoleRepository
    /// </summary>
    public class KitchenRoleRepository : IKitchenRoleRepository
    {
        /// <inheritdoc/>
        public IEnumerable<KitchenRole> GetAllKitchenRoles()
        {
            var sql = "SELECT * FROM kitchen_role ORDER BY name ASC;";
            return DBConnector.QueryDatabase<KitchenRole>(sql);
        }

        /// <inheritdoc/>
        public KitchenRole? GetKitchenRoleById(Guid id)
        {
            var sql = "SELECT * FROM kitchen_role WHERE id = @id;";
            return DBConnector.QueryDatabase<KitchenRole>(sql, new { id }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public KitchenRole? GetKitchenRoleByName(string name)
        {
            var sql = "SELECT * FROM kitchen_role WHERE LOWER(name) = LOWER(@name);";
            return DBConnector.QueryDatabase<KitchenRole>(sql, new { name }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public KitchenRole CreateKitchenRole(string name)
        {
            var sql = """
                INSERT INTO kitchen_role (name) 
                OUTPUT INSERTED.* 
                VALUES (@name);
                """;
            var created = DBConnector.QueryDatabase<KitchenRole>(sql, new { name = name ?? string.Empty }).ToList();
            if (created.Count > 0)
            {
                return created[0];
            }
            throw new Exception("Insert failed for kitchen_role");
        }

        /// <inheritdoc/>
        public KitchenRole? UpdateKitchenRole(Guid id, string name)
        {
            var sql = """
                UPDATE kitchen_role 
                SET name = @name 
                OUTPUT INSERTED.* 
                WHERE id = @id;
                """;
            return DBConnector.QueryDatabase<KitchenRole>(sql, new { id, name = name ?? string.Empty }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool DeleteKitchenRole(Guid id)
        {
            var sql = "DELETE FROM kitchen_role WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id }).ToList();
            return true;
        }
    }
}
