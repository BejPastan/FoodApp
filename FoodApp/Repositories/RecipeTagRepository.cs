using FoodApp.Models;
using FoodApp.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FoodApp.Repositories
{
    public interface IRecipeTagRepository
    {
        IEnumerable<RecipeTag> GetRecipeTags(int? recipeId, int? tagId);
        RecipeTag? CreateRecipeTag(int recipeId, int tagId);
        bool DeleteRecipeTag(int recipeId, int tagId);
    }

    public class RecipeTagRepository : IRecipeTagRepository
    {
        public IEnumerable<RecipeTag> GetRecipeTags(int? recipeId, int? tagId)
        {
            var sql = "SELECT * FROM recipe_tags WHERE 1=1";
            if (recipeId.HasValue) sql += " AND recipeId = @recipeId";
            if (tagId.HasValue) sql += " AND tagId = @tagId";
            sql += " ORDER BY recipeId ASC, tagId ASC;";
            return DBConnector.QueryDatabase<RecipeTag>(sql, new { recipeId = recipeId, tagId = tagId });
        }

        public RecipeTag? CreateRecipeTag(int recipeId, int tagId)
        {
            var sql = "INSERT INTO recipe_tags (recipeId, tagId) OUTPUT INSERTED.* VALUES (@recipeId, @tagId);";
            return DBConnector.QueryDatabase<RecipeTag>(sql, new { recipeId = recipeId, tagId = tagId }).FirstOrDefault();
        }

        public bool DeleteRecipeTag(int recipeId, int tagId)
        {
            var sql = "DELETE FROM recipe_tags WHERE recipeId = @recipeId AND tagId = @tagId;";
            DBConnector.QueryDatabase<int>(sql, new { recipeId = recipeId, tagId = tagId }).ToList();
            return true;
        }
    }
}
