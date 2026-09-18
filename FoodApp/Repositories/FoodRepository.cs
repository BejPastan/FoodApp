using FoodApp.Models;
using FoodApp.Utilities;
using FoodApp.Repositories.Interfaces;


namespace FoodApp.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class FoodRepository : IFoodRepository
    {
        /// <summary>
        /// Return list of food
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="nameFilter"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Food> GetFoods(Guid? typeId, string nameFilter, int page = 1, int pageSize = 25)
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
            return DBConnector.QueryDatabase<Food>(sql, new { typeId, name = $"%{nameFilter}%", offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        /// <summary>
        /// return single food object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Food? GetFoodById(Guid id)
        {
            var sql = "SELECT * FROM food WHERE id = @id;";
            return DBConnector.QueryDatabase<Food>(sql, new { id = id }).FirstOrDefault();
        }

        /// <summary>
        /// Create single food object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foodTypeId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Food CreateFood(string name, Guid foodTypeId)
        {
            var sql = @"INSERT INTO food 
                 (name, foodTypeId) 
                 OUTPUT INSERTED.* 
                 VALUES (@name, @foodType);";
            var list = DBConnector.QueryDatabase<Food>(sql, new { name = name ?? string.Empty, foodType = foodTypeId }).ToList();
            if (list.Count >0) return list[0];
            throw new Exception("Insert failed for food");
        }

        public Food? UpdateFood(Guid id, string? name, Guid? foodTypeId)
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
            return DBConnector.QueryDatabase<Food>(sql, new { id, name = $"{name}", foodType = foodTypeId }).FirstOrDefault();
        }

        public bool DeleteFood(Guid id)
        {
            var sql = "DELETE FROM food WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id }).ToList();
            return true;
        }
    }
}
