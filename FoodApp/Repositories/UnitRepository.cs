using Dapper;
using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IUnitRepository
    {
        IEnumerable<Unit> GetUnits(string nameFilter, int page = 1, int pageSize = 25);
        Unit? GetUnitById(int id);
        Unit CreateUnit(string name, decimal volumeEquivalent);
        Unit? UpdateUnit(int id, string? name, decimal? volumeEquivalent);
        bool DeleteUnit(int id);
    }

    public class UnitRepository : IUnitRepository
    {
        public IEnumerable<Unit> GetUnits(string nameFilter, int page = 1, int pageSize = 25)
        {
            var sql = "SELECT * FROM units WHERE name LIKE @name ORDER BY name ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
            return DBConnector.QueryDatabase<Unit>(sql, new { name = $"%{nameFilter}%", offset = (page - 1) * pageSize, pageSize = pageSize });
        }

        public Unit? GetUnitById(int id)
        {
            var sql = "SELECT * FROM units WHERE id = @id;";
            return DBConnector.QueryDatabase<Unit>(sql, new { id = id }).FirstOrDefault();
        }

        public Unit CreateUnit(string name, decimal volumeEquivalent)
        {
            var sql = "INSERT INTO units (name, volumeEquivalent) OUTPUT INSERTED.* VALUES (@name, @vol);";
            var list = DBConnector.QueryDatabase<Unit>(sql, new { name = name ?? string.Empty, vol = volumeEquivalent }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed");
        }

        public Unit? UpdateUnit(int id, string? name, decimal? volumeEquivalent)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();
            if (name != null) { sets.Add("name = @name"); parameters.Add("name", name); }
            if (volumeEquivalent.HasValue) { sets.Add("volumeEquivalent = @vol"); parameters.Add("vol", volumeEquivalent.Value); }
            if (sets.Count == 0) return null;
            var sql = $"UPDATE units SET {string.Join(", ", sets)} WHERE id = @id; SELECT * FROM units WHERE id = @id;";
            parameters.Add("id", id);
            return DBConnector.QueryDatabase<Unit>(sql, parameters).FirstOrDefault();
        }

        public bool DeleteUnit(int id)
        {
            var sql = "DELETE FROM units WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
