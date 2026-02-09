using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetTags(string nameFilter);
        Tag? GetTagById(int id);
        Tag CreateTag(string name);
        Tag? UpdateTag(int id, string? name);
        bool DeleteTag(int id);
    }

    public class TagRepository : ITagRepository
    {
        public IEnumerable<Tag> GetTags(string nameFilter)
        {
            var sql = "SELECT * FROM tags WHERE name LIKE @name ORDER BY id ASC;";
            return DBConnector.QueryDatabase<Tag>(sql, new { name = $"%{nameFilter}%" });
        }

        public Tag? GetTagById(int id)
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

        public Tag? UpdateTag(int id, string? name)
        {
            if (name == null) return null;
            var sql = "UPDATE tags SET name = @name WHERE id = @id; SELECT * FROM tags WHERE id = @id;";
            return DBConnector.QueryDatabase<Tag>(sql, new { name = name, id = id }).FirstOrDefault();
        }

        public bool DeleteTag(int id)
        {
            var sql = "DELETE FROM tags WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
