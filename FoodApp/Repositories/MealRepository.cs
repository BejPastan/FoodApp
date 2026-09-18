using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IMealRepository
    {
        IEnumerable<Meal> GetMeals(string? nameFilter, Guid? recipeId, int page = 1, int pageSize = 25);
        Meal? GetMealById(Guid id);
        Meal CreateMeal(string name, int order);
        Meal? UpdateMeal(Guid id, string? name, int? order);
        bool DeleteMeal(Guid id);
    }

    public class MealRepository : IMealRepository
    {
        public IEnumerable<Meal> GetMeals(string? nameFilter, Guid? recipeId, int page = 1, int pageSize = 25)
        {
            string sql = "SELECT * FROM meal WHERE 1=1";
            if (!string.IsNullOrEmpty(nameFilter))
            {
                sql += " AND name LIKE @name";
            }
            if(recipeId != Guid.Empty && recipeId != null)
            {
                sql += " AND id IN (SELECT mealId FROM recipe_meal WHERE recipeId = @recipeId)";
            }
            sql += " ORDER BY id ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            return DBConnector.QueryDatabase<Meal>(sql, new { name = $"%{nameFilter}%", recipeId = recipeId, offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        public Meal? GetMealById(Guid id)
        {
            var sql = "SELECT * FROM meal WHERE id = @id;";
            return DBConnector.QueryDatabase<Meal>(sql, new { id = id }).FirstOrDefault();
        }

        public Meal CreateMeal(string name, int order = int.MaxValue)
        {
            var sql = $"INSERT INTO meal (name, [order]) OUTPUT INSERTED.* VALUES (@name, @order);";
            var list = DBConnector.QueryDatabase<Meal>(sql, new { name, order}).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed for meal");
        }

        /// <inheritdoc/>
        public Meal? UpdateMeal(Guid id, string? name, int? order = -1)
        {
            var sql = "UPDATE meal SET ";
            List<string> elements = [];
            if(order.HasValue && order.Value != -1)
            {
                elements.Add(" [order] = @order");
            }
            if(name != null)
            {
                elements.Add(" name = @name");
            }
            sql += String.Join(",", elements);
            sql+=" WHERE id = @id; SELECT * FROM meal WHERE id = @id;";
            return DBConnector.QueryDatabase<Meal>(sql, new { id = id, name = name?? string.Empty,order = order??-1}).FirstOrDefault();
        }

        public bool DeleteMeal(Guid id)
        {
            var sql = "DELETE FROM meal WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
