using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IRecipeRepository
    {
        IEnumerable<Recipe> GetRecipes(string nameFilter, int page = 1, int pageSize = 25);
        Recipe? GetRecipeById(int id);
        Recipe CreateRecipe(string name, int portion, int prepTime);
        Recipe? UpdateRecipe(int id, string? name, int? portion, int? prepTime);
        bool DeleteRecipe(int id);
        Recipe[] GetRecipesToChoose(int userId, int mealId, int excludedWeeks);
    }

    public class RecipeRepository : IRecipeRepository
    {
        public IEnumerable<Recipe> GetRecipes(string nameFilter, int page = 1, int pageSize = 25)
        {
            var sql = "SELECT * FROM recipe WHERE name LIKE @name ORDER BY name ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            return DBConnector.QueryDatabase<Recipe>(sql, new { name = $"%{nameFilter}%", offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        public Recipe? GetRecipeById(int id)
        {
            var sql = "SELECT * FROM recipe WHERE id = @id;";
            var result = DBConnector.QueryDatabase<Recipe>(sql, new { id = id }).FirstOrDefault();
            return result;
        }

        public Recipe CreateRecipe(string name, int portion, int prepTime)
        {
            string sql = "INSERT INTO recipe (name, time, portion) OUTPUT INSERTED.* VALUES (@name, @prepTime, @portion);";
            return DBConnector.QueryDatabase<Recipe>(sql, new { name = name, time = prepTime, portion = portion }).First();
        }

        public Recipe? UpdateRecipe(int id, string? name, int? portion, int? prepTime)
        {
            string sql = "UPDATE recipe OUTPUT INSERTED.* SET ";
            if (name != null)
            {
                sql.Concat(" name = @name,");
            }
            if(portion != null)
            {
                sql.Concat(" portion = @portion,");
            }
            if(prepTime != null)
            {
                sql.Concat(" time = @prepTime,");
            }
            sql = sql.Substring(0, sql.Length - 2);
            sql.Concat(" WHERE id = @id; SELECT * FROM recipe WHERE id = @id;");
            return DBConnector.QueryDatabase<Recipe>(sql, new { id = id, name = name, time = prepTime, portion = portion }).FirstOrDefault();
        }

        public bool DeleteRecipe(int id)
        {
            string sql = "DELETE FROM recipe OUTPUT DELETED.* WHERE id = @id;";
            return DBConnector.QueryDatabase<Recipe>(sql, new { id = id }).Any();
        }

        /// <summary>
        /// Return list of possible recipes to choose from based on criteria
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mealId"></param>
        /// <param name="excluded"></param>
        /// <param name="excludedWeeks"></param>
        /// <returns></returns>
        public Recipe[] GetRecipesToChoose(int userId, int mealId, int excludedWeeks)
        {
            DateTime cutOffDate = DateTime.Now.AddDays(-excludedWeeks * 7);
            string sql = "With recipe_usage AS (Select rm.recipeId, COUNT(um.recipeId) eaten from recipe_meal rm JOIN meal ON meal.id = rm.mealId LEFT JOIN user_meals um ON um.recipeId = rm.recipeId AND um.mealDate >= @cutDate AND um.userId = @userId WHERE meal.id = @mealId GROUP BY rm.recipeId),ranked_recipes AS (SELECT TOP(3) ROW_NUMBER() OVER(ORDER BY ru.eaten asc, NEWID()) as rank_order, ru.recipeId as recipeId FROM recipe_usage ru ORDER BY rank_order) SELECT * FROM recipe WHERE recipe.id IN(SELECT ranked_recipes.recipeId FROM ranked_recipes);";
            Recipe[] recipes = DBConnector.QueryDatabase<Recipe>(sql, new {cutDate = cutOffDate, mealId = mealId, userId = userId}).ToArray();
            Console.WriteLine($"recipeCount in repo: {recipes.Length}");
            return recipes;
        }
    }
}