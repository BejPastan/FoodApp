using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// Default implementation of IKitchenRepository
    /// </summary>
    public class KitchenRepository : IKitchenRepository
    {
        /// <inheritdoc/>
        public Kitchen? GetKitchenById(Guid id)
        {
            var sql = "SELECT * FROM Kitchens WHERE id = @id;";
            return DBConnector.QueryDatabase<Kitchen>(sql, new { id }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public IEnumerable<Kitchen> GetKitchensByUserId(Guid userId, string name="")
        {
            var sql = """
                SELECT DISTINCT k.* 
                FROM Kitchens k 
                LEFT JOIN kitchens_users ku ON k.id = ku.kitchenId 
                WHERE ku.userId = @userId AND k.name LIKE @name
                ORDER BY k.name ASC;
                """;
            return DBConnector.QueryDatabase<Kitchen>(sql, new { userId, name=$"%{name}%" });
        }

        /// <inheritdoc/>
        public Kitchen CreateKitchen(string name, Guid ownerId, string accessCode = "")
        {
            var sql = """
                INSERT INTO Kitchens (name, ownerId, accessCode) 
                OUTPUT INSERTED.* 
                VALUES (@name, @ownerId, @accessCode);
                """;
            var created = DBConnector.QueryDatabase<Kitchen>(sql, new { name = name ?? string.Empty, ownerId, accessCode }).ToList();
            if (created.Count > 0)
            {
                return created[0];
            }
            throw new Exception("Insert failed for kitchen");
        }

        /// <inheritdoc/>
        public Kitchen? UpdateKitchen(Guid id, string? name, string? accessCode)
        {
            var sql = " UPDATE Kitchens SET ";
            if(name!= null)
            {
                sql += " name = @name ";
            }
            if(accessCode != null)
            {
                sql += " accessCode = @accessCode ";
            }
            sql += " OUTPUT INSERTED.* WHERE id = @id;";
                return DBConnector.QueryDatabase<Kitchen>(sql, new { id, name = name ?? string.Empty, accessCode = accessCode?? string.Empty }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool DeleteKitchen(Guid id)
        {
            var sql = "DELETE FROM Kitchens WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id }).ToList();
            return true;
        }

        public Kitchen? GetKitchenByAccessCode(string accessCode)
        {
            string sql = "SELECT * FROM Kitchen k WHERE k.accessCode = @accessCode";
            var resp = DBConnector.QueryDatabase<Kitchen>(sql, new { accessCode }).FirstOrDefault();
            if(resp != null)
            {
                return resp;
            }
            else
            {
                return null;
            }
        }
    }
}
