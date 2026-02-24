using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IMealRepository
    {
        IEnumerable<Meal> GetMeals(string? nameFilter, int? recipeId, int page = 1, int pageSize = 25);
        Meal? GetMealById(int id);
        Meal CreateMeal(Meal name);
        Meal? UpdateMeal(int id, string? name);
        bool DeleteMeal(int id);
    }

    public class MealRepository : IMealRepository
    {
        public IEnumerable<Meal> GetMeals(string? nameFilter, int? recipeId, int page = 1, int pageSize = 25)
        {
            string sql = "SELECT * FROM meal WHERE 1=1";
            if (!string.IsNullOrEmpty(nameFilter))
            {
                sql += " AND name LIKE @name";
            }
            if(recipeId > 0)
            {
                sql += " AND id IN (SELECT mealId FROM recipe_meal WHERE recipeId = @recipeId)";
            }
            sql += " ORDER BY id ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            return DBConnector.QueryDatabase<Meal>(sql, new { name = $"%{nameFilter}%", recipeId = recipeId, offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        public Meal? GetMealById(int id)
        {
            var sql = "SELECT * FROM meal WHERE id = @id;";
            return DBConnector.QueryDatabase<Meal>(sql, new { id = id }).FirstOrDefault();
        }

        public Meal CreateMeal(Meal mealRequest)
        {
            var sql = "INSERT INTO meal (name) OUTPUT INSERTED.* VALUES (@name);";
            var list = DBConnector.QueryDatabase<Meal>(sql, new { name = mealRequest.name ?? string.Empty }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed for meal");
        }

        public Meal? UpdateMeal(int id, string? name)
        {
            if (name == null) return null;
            var sql = "UPDATE meal SET name = @name WHERE id = @id; SELECT * FROM meal WHERE id = @id;";
            return DBConnector.QueryDatabase<Meal>(sql, new { name = name, id = id }).FirstOrDefault();
        }

        public bool DeleteMeal(int id)
        {
            var sql = "DELETE FROM meal WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
