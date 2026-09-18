using FoodApp.Models;
using FoodApp.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FoodApp.Repositories
{
    public interface IRecipeTagRepository
    {
        IEnumerable<RecipeTag> GetRecipeTags(Guid? recipeId, Guid? tagId);
        RecipeTag? CreateRecipeTag(Guid recipeId, Guid tagId);
        bool DeleteRecipeTag(Guid recipeId, Guid tagId);
        /// <summary>
        /// Delete all tags associated with specific recipe record
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        bool DeleteRecipeTags(Guid recipeId);
    }

    public class RecipeTagRepository : IRecipeTagRepository
    {
        public IEnumerable<RecipeTag> GetRecipeTags(Guid? recipeId, Guid? tagId)
        {
            var sql = "SELECT * FROM recipe_tags WHERE 1=1";
            if (recipeId.HasValue) sql += " AND recipeId = @recipeId";
            if (tagId.HasValue) sql += " AND tagId = @tagId";
            sql += " ORDER BY recipeId ASC, tagId ASC;";
            return DBConnector.QueryDatabase<RecipeTag>(sql, new { recipeId = recipeId, tagId = tagId });
        }

        public RecipeTag? CreateRecipeTag(Guid recipeId, Guid tagId)
        {
            var sql = "INSERT INTO recipe_tags (recipeId, tagId) OUTPUT INSERTED.* VALUES (@recipeId, @tagId);";
            return DBConnector.QueryDatabase<RecipeTag>(sql, new { recipeId = recipeId, tagId = tagId }).FirstOrDefault();
        }

        public bool DeleteRecipeTag(Guid recipeId, Guid tagId)
        {
            var sql = "DELETE FROM recipe_tags WHERE recipeId = @recipeId AND tagId = @tagId;";
            DBConnector.QueryDatabase<Guid>(sql, new { recipeId = recipeId, tagId = tagId }).ToList();
            return true;
        }

        /// <summary>
        /// Delete all tags associated with specific recipe record
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        public bool DeleteRecipeTags(Guid recipeId)
        {
            Console.WriteLine("Deleting recipe tags");
            var sql = "DELETE FROM recipe_tags WHERE recipeId = @recipeId;";
            DBConnector.QueryDatabase<Guid>(sql, new { recipeId }).ToList();
            return true;
        }
    }
}
