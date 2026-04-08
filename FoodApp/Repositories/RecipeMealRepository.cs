using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IRecipeMealRepository
    {
        IEnumerable<RecipeMeal> GetRecipeMeals(int? recipeId, int? mealId);
        RecipeMeal? GetRecipeMealById(int id);
        RecipeMeal? CreateRecipeMeal(int recipeId, int mealId);
        bool DeleteRecipeMeal(int id);
        bool DeleteRecipeMeal(int recipeId, int mealId);
        bool DeleteRecipesMeal(int recipeId);
    }

    public class RecipeMealRepository : IRecipeMealRepository
    {
        public IEnumerable<RecipeMeal> GetRecipeMeals(int? recipeId, int? mealId)
        {
            var sql = "SELECT * FROM recipe_meal WHERE 1=1";
            if (recipeId.HasValue) sql += " AND recipeId = @recipeId";
            if (mealId.HasValue) sql += " AND mealId = @mealId";
            sql += " ORDER BY id ASC;";
            return DBConnector.QueryDatabase<RecipeMeal>(sql, new { recipeId = recipeId, mealId = mealId });
        }

        public RecipeMeal? GetRecipeMealById(int id)
        {
            var sql = "SELECT * FROM recipe_meal WHERE id = @id;";
            return DBConnector.QueryDatabase<RecipeMeal>(sql, new { id = id }).FirstOrDefault();
        }

        public RecipeMeal? CreateRecipeMeal(int recipeId, int mealId)
        {
            var sql = "INSERT INTO recipe_meal (recipeId, mealId) OUTPUT INSERTED.* VALUES (@recipeId, @mealId);";
            return DBConnector.QueryDatabase<RecipeMeal>(sql, new { recipeId = recipeId, mealId = mealId }).FirstOrDefault();
        }

        public bool DeleteRecipeMeal(int id)
        {
            var sql = "DELETE FROM recipe_meal WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
        public bool DeleteRecipeMeal(int recipeId, int mealId)
        {
            var sql = "DELETE FROM recipe_meal WHERE recipeId = @recipeId AND mealId = @mealId;";
            DBConnector.QueryDatabase<int>(sql, new { recipeId = recipeId, mealId = mealId }).ToList();
            return true;
        }

        public bool DeleteRecipesMeal(int recipeId)
        {
            var sql = "DELETE FROM recipe_meal WHERE recipeId = @recipeId;";
            DBConnector.QueryDatabase<int>(sql, new { recipeId = recipeId}).ToList();
            return true;
        }
    }
}
