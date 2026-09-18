using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetTags(string nameFilter, Guid? recipeId, int page, int pageSize);
        Tag? GetTagById(Guid id);
        Tag CreateTag(string name);
        Tag? UpdateTag(Guid id, string? name);
        bool DeleteTag(Guid id);
    }

    public class TagRepository : ITagRepository
    {
        public IEnumerable<Tag> GetTags(string nameFilter, Guid? recipeId, int page, int pageSize)
        {
            var sql = "SELECT * FROM tags WHERE name LIKE @name";
            Console.WriteLine(recipeId);
            if (recipeId.HasValue)
            {
                sql += " AND id IN (SELECT tagId FROM recipe_tags WHERE recipeId = @recipeId)";
            }
            sql += " ORDER BY id ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            return DBConnector.QueryDatabase<Tag>(sql, new { name = $"%{nameFilter}%", recipeId, offset = (page - 1) * pageSize, pageSize = pageSize }).ToList();
        }

        public Tag? GetTagById(Guid id)
        {
            var sql = "SELECT * FROM tags WHERE id = @id;";
            return DBConnector.QueryDatabase<Tag>(sql, new { id = id }).FirstOrDefault();
        }

        public Tag CreateTag(string name)
        {
            var sql = "INSERT INTO tags (name) OUTPUT INSERTED.* VALUES (@name);";
            var list = DBConnector.QueryDatabase<Tag>(sql, new { name = name ?? string.Empty }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed");
        }

        public Tag? UpdateTag(Guid id, string? name)
        {
            if (name == null) return null;
            var sql = "UPDATE tags SET name = @name WHERE id = @id; SELECT * FROM tags WHERE id = @id;";
            return DBConnector.QueryDatabase<Tag>(sql, new { name = name, id = id }).FirstOrDefault();
        }

        public bool DeleteTag(Guid id)
        {
            var sql = "DELETE FROM tags WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
