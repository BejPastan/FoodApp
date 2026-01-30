using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IRecipeRepository
    {
        IEnumerable<Recipe> GetRecipes(string nameFilter);
        Recipe? GetRecipeById(int id);
        Recipe CreateRecipe(string name);
        Recipe? UpdateRecipe(int id, string? name);
        bool DeleteRecipe(int id);
        Recipe[] GetRecipesToChoose(int userId, int mealId, int[] excluded, int excludedWeeks);
    }

    public class RecipeRepository : IRecipeRepository
    {
        public IEnumerable<Recipe> GetRecipes(string nameFilter)
        {
            var sql = "SELECT * FROM recipe WHERE name LIKE @name ORDER BY id ASC;";
            return DBConnector.QueryDatabase<Recipe>(sql, new { name = $"%{(nameFilter ?? string.Empty)}%" });
        }

        public Recipe? GetRecipeById(int id)
        {
            var sql = "SELECT * FROM recipe WHERE id = @id;";
            var result = DBConnector.QueryDatabase<Recipe>(sql, new { id = id }).FirstOrDefault();
            Console.WriteLine(result.ToString());
            return result;
        }

        public Recipe CreateRecipe(string name)
        {
            string sql = "INSERT INTO recipe (name) OUTPUT INSERTED.* VALUES (@name);";
            return DBConnector.QueryDatabase<Recipe>(sql, new { name = name ?? string.Empty }).First();
        }

        public Recipe? UpdateRecipe(int id, string? name)
        {
            if (name == null) return null;
            string sql = "UPDATE recipe OUTPUT INSERTED.* SET name = @name WHERE id = @id; SELECT * FROM recipe WHERE id = @id;";
            return DBConnector.QueryDatabase<Recipe>(sql, new { id = id, name = name }).FirstOrDefault();
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
        public Recipe[] GetRecipesToChoose(int userId, int mealId, int[] excluded, int excludedWeeks)
        {
            DateTime cutOffDate = DateTime.Now.AddDays(-excludedWeeks * 7);
            string sql = "SELECT * FROM recipe WHERE recipe.id NOT IN @excluded AND recipe.id NOT IN (SELECT recipeId FROM user_meals WHERE mealDate>@cutDate) AND recipe.id IN (SELECT recipeId FROM recipe_meal WHERE id=@mealId);";
            if(excluded.Length == 0)
            {
                excluded = new int[] { -1 };
            }
            return DBConnector.QueryDatabase<Recipe>(sql, new { excluded = excluded, cutDate = cutOffDate, mealId = mealId }).ToArray();
        }
    }
}