using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IRecipeMealRepository
    {
        IEnumerable<RecipeMeal> GetRecipeMeals(Guid? recipeId, Guid? mealId);
        RecipeMeal? GetRecipeMealById(Guid id);
        RecipeMeal? CreateRecipeMeal(Guid recipeId, Guid mealId);
        bool DeleteRecipeMeal(Guid id);
        bool DeleteRecipeMeal(Guid recipeId, Guid mealId);
        bool DeleteRecipesMeal(Guid recipeId);
    }

    public class RecipeMealRepository : IRecipeMealRepository
    {
        public IEnumerable<RecipeMeal> GetRecipeMeals(Guid? recipeId, Guid? mealId)
        {
            var sql = "SELECT * FROM recipe_meal WHERE 1=1";
            if (recipeId.HasValue) sql += " AND recipeId = @recipeId";
            if (mealId.HasValue) sql += " AND mealId = @mealId";
            sql += " ORDER BY id ASC;";
            return DBConnector.QueryDatabase<RecipeMeal>(sql, new { recipeId = recipeId, mealId = mealId });
        }

        public RecipeMeal? GetRecipeMealById(Guid id)
        {
            var sql = "SELECT * FROM recipe_meal WHERE id = @id;";
            return DBConnector.QueryDatabase<RecipeMeal>(sql, new { id }).FirstOrDefault();
        }

        public RecipeMeal? CreateRecipeMeal(Guid recipeId, Guid mealId)
        {
            var sql = "INSERT INTO recipe_meal (recipeId, mealId) OUTPUT INSERTED.* VALUES (@recipeId, @mealId);";
            return DBConnector.QueryDatabase<RecipeMeal>(sql, new { recipeId = recipeId, mealId = mealId }).FirstOrDefault();
        }

        public bool DeleteRecipeMeal(Guid id)
        {
            var sql = "DELETE FROM recipe_meal WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id = id }).ToList();
            return true;
        }
        public bool DeleteRecipeMeal(Guid recipeId, Guid mealId)
        {
            var sql = "DELETE FROM recipe_meal WHERE recipeId = @recipeId AND mealId = @mealId;";
            DBConnector.QueryDatabase<Guid>(sql, new { recipeId = recipeId, mealId = mealId }).ToList();
            return true;
        }

        public bool DeleteRecipesMeal(Guid recipeId)
        {
            var sql = "DELETE FROM recipe_meal WHERE recipeId = @recipeId;";
            DBConnector.QueryDatabase<Guid>(sql, new { recipeId}).ToList();
            return true;
        }
    }
}
