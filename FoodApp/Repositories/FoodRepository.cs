using FoodApp.Models;
using FoodApp.Utilities;


namespace FoodApp.Repositories
{
    public interface IFoodRepository
    {
        IEnumerable<Food> GetFoods(int? typeId, string nameFilter, int page = 1, int pageSize = 25);
        Food? GetFoodById(int id);
        Food CreateFood(string name, int foodTypeId);
        Food? UpdateFood(int id, string? name, int? foodTypeId);
        bool DeleteFood(int id);
    }

    public class FoodRepository : IFoodRepository
    {
        public IEnumerable<Food> GetFoods(int? typeId, string nameFilter, int page = 1, int pageSize = 25)
        {
            string sql = "SELECT * FROM food WHERE 1=1";
            if(typeId.HasValue)
            {
                sql += " AND foodTypeId = @typeId";
            }
            if(!string.IsNullOrEmpty(nameFilter))
            {
                sql += " AND name LIKE @name";
            }
            sql += " ORDER BY name ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            Console.WriteLine("get food repo test");
            return DBConnector.QueryDatabase<Food>(sql, new { typeId = typeId, name = $"%{nameFilter}%", offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        public Food? GetFoodById(int id)
        {
            var sql = "SELECT * FROM food WHERE id = @id;";
            return DBConnector.QueryDatabase<Food>(sql, new { id = id }).FirstOrDefault();
        }

        public Food CreateFood(string name, int foodTypeId)
        {
            var sql = @"INSERT INTO food 
                 (name, foodTypeId) 
                 OUTPUT INSERTED.* 
                 VALUES (@name, @foodType);";
            var list = DBConnector.QueryDatabase<Food>(sql, new { name = name ?? string.Empty, foodType = foodTypeId }).ToList();
            if (list.Count >0) return list[0];
            throw new Exception("Insert failed for food");
        }

        public Food? UpdateFood(int id, string? name, int? foodTypeId)
        {
            var sets = new List<string>();
            if (name != null) 
            { 
                sets.Add("name = @name"); 
            }
            if (foodTypeId.HasValue) 
            {
                sets.Add("foodTypeId = @foodType"); 
            }
            if (sets.Count ==0) return null;
            var sql = $"UPDATE food SET {string.Join(", ", sets)} WHERE id = @id; SELECT * FROM food WHERE id = @id;";
            return DBConnector.QueryDatabase<Food>(sql, new { id = id, name = $"%{name}%", foodType = foodTypeId }).FirstOrDefault();
        }

        public bool DeleteFood(int id)
        {
            var sql = "DELETE FROM food WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
