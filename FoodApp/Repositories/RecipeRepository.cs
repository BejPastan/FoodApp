using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;
using System.Text.Json;

namespace FoodApp.Repositories
{
    /// <summary>
    /// Default implementation of IRecipeRepo
    /// </summary>
    public class RecipeRepository : IRecipeRepo
    {
        /// <inheritdoc/>
        public IEnumerable<RecipeRecord> GetRecipes(string[]? mealNames, string[]? tags, string nameFilter, int page = 1, int pageSize = 25)
        {
            var param = new Dictionary<string, object>
            {
                { "name", $"%{nameFilter}%" },
                { "offset", (page - 1) * pageSize },
                { "pageSize", pageSize }
            };

            var sql = """
                SELECT r.id, r.name, r.portion, r.time FROM recipe r 

                """;

            if (mealNames != null && mealNames.Length > 0)
            {
                sql += """
                    JOIN recipe_meal rm ON rm.recipeId = r.id 
                    JOIN meal m ON rm.mealId = m.id 
                    JOIN OPENJSON(@mealArray) AS ma ON m.name LIKE ma.value

                    """;
                param.Add("mealArray", JsonSerializer.Serialize(mealNames));
            }
            if(tags!= null && tags.Length > 0)
            {
                sql += """
                    JOIN recipe_tags rt ON rt.recipeId = r.id
                    JOIN tags t ON rt.tagId = t.id
                    JOIN OPENJSON(@tagArray) AS ta ON t.name = ta.value

                    """;
                param.Add("tagArray", JsonSerializer.Serialize(tags));
            }

            sql += """
                WHERE r.name LIKE @name 
                GROUP BY r.id, r.name, r.portion, r.time
                ORDER BY r.name ASC 
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                """;

            var result = DBConnector.QueryDatabase<RecipeRecord>(sql,param);
            return result;
        }

        /// <inheritdoc/>
        public Recipe? GetRecipeById(Guid id)
        {
            var sql = "SELECT * FROM recipe WHERE id = @id;";
            var result = DBConnector.QueryDatabase<Recipe>(sql, new { id }).FirstOrDefault();
            return result;
        }

        /// <inheritdoc/>

        public Recipe CreateRecipe(string name, int portion, int prepTime)
        {
            string sql = "INSERT INTO recipe (name, time, portion) OUTPUT INSERTED.* VALUES (@name, @prepTime, @portion);";
            return DBConnector.QueryDatabase<Recipe>(sql, new { name, prepTime, portion }).First();
        }

        /// <inheritdoc/>
        public RecipeRecord? UpdateRecipe(Guid id, string? name, int? portion, int? prepTime)
        {
            string sql = "UPDATE recipe SET ";
            if (name != null)
            {
                sql+=(" name = @name,");
            }
            if(portion != null)
            {
                sql+=(" portion = @portion,");
            }
            if(prepTime != null)
            {
                sql+=(" time = @prepTime,");
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += (" OUTPUT INSERTED.*  WHERE id = @id;");
            return DBConnector.QueryDatabase<RecipeRecord>(sql, new { id = id, name = name, prepTime = prepTime, portion = portion }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool DeleteRecipe(Guid id)
        {
            string sql = "DELETE FROM recipe OUTPUT DELETED.* WHERE id = @id;";
            return DBConnector.QueryDatabase<Recipe>(sql, new { id = id }).Any();
        }

        /// <inheritdoc/>
        public RecipeRecord[] GetRecipesToChoose(Guid userId, Guid mealId, int excludedWeeks)
        {
            DateTime cutOffDate = DateTime.Now.AddDays(-excludedWeeks * 7);
            string sql = "With recipe_usage AS (Select rm.recipeId, COUNT(um.recipeId) eaten from recipe_meal rm JOIN meal ON meal.id = rm.mealId LEFT JOIN user_meals um ON um.recipeId = rm.recipeId AND um.mealDate >= @cutDate AND um.userId = @userId WHERE meal.id = @mealId GROUP BY rm.recipeId),ranked_recipes AS (SELECT TOP(3) ROW_NUMBER() OVER(ORDER BY ru.eaten asc, NEWID()) as rank_order, ru.recipeId as recipeId FROM recipe_usage ru ORDER BY rank_order) SELECT * FROM recipe WHERE recipe.id IN(SELECT ranked_recipes.recipeId FROM ranked_recipes);";
            Recipe[] recipes = DBConnector.QueryDatabase<Recipe>(sql, new {cutDate = cutOffDate, mealId = mealId, userId = userId}).ToArray();
            Console.WriteLine($"recipeCount in repo: {recipes.Length}");
            return recipes;
        }
    }
}