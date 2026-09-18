using FoodApp.Models;
using FoodApp.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FoodApp.Repositories
{
    public interface IFoodTypeRepository
    {
        IEnumerable<FoodType> GetFoodTypes(string nameFilter, int page = 1, int pageSize = 25);
        FoodType? GetFoodTypeById(Guid id);
        FoodType CreateFoodType(string name);
        FoodType? UpdateFoodType(Guid id, string? name);
        bool DeleteFoodType(Guid id);
    }

    public class FoodTypeRepository : IFoodTypeRepository
    {
        public IEnumerable<FoodType> GetFoodTypes(string nameFilter, int page = 1, int pageSize = 25)
        {
            var sql = "SELECT * FROM food_type WHERE name LIKE @name ORDER BY name ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            return DBConnector.QueryDatabase<FoodType>(sql, new { name = $"%{(nameFilter ?? string.Empty)}%", offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        public FoodType? GetFoodTypeById(Guid id)
        {
            var sql = "SELECT * FROM food_type WHERE id = @id;";
            return DBConnector.QueryDatabase<FoodType>(sql, new { id = id }).FirstOrDefault();
        }

        public FoodType CreateFoodType(string name)
        {
            var sql = "INSERT INTO food_type (name) OUTPUT INSERTED.* VALUES (@name);";
            var list = DBConnector.QueryDatabase<FoodType>(sql, new { name = name ?? string.Empty }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed for food_type");
        }

        public FoodType? UpdateFoodType(Guid id, string? name)
        {
            if (name == null) return null;
            var sql = "UPDATE food_type SET name = @name WHERE id = @id; SELECT * FROM food_type WHERE id = @id;";
            return DBConnector.QueryDatabase<FoodType>(sql, new { name = $"{name}", id = id }).FirstOrDefault();
        }

        public bool DeleteFoodType(Guid id)
        {
            var sql = "DELETE FROM food_type WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id = id }).ToList();
            return true;
        }
    }
}