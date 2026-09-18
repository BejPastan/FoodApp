using Dapper;
using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IUnitRepository
    {
        /// <summary>
        /// return units
        /// </summary>
        /// <param name="searchFilter">search filter, for name and desc</param>
        /// <param name="unitIds">ids of units in which we could search, if empty, skip this parameter</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<Unit> GetUnits(string? searchFilter, Guid[]? unitIds, int page = 1, int pageSize = 25);
        Unit? GetUnitById(Guid id);
        Unit CreateUnit(string name, decimal volumeEquivalent, string? desc);
        Unit? UpdateUnit(Guid id, string? name, decimal? volumeEquivalent, string? desc);
        bool DeleteUnit(Guid id);
        UnitConvertResp ConvertUnit(Guid oldUnitId, Guid newUnitId, float originalAmount);
    }

    public class UnitRepository : IUnitRepository
    {
        /// <summary>
        /// return units
        /// </summary>
        /// <param name="searchFilter">search filter, for name and desc</param>
        /// <param name="unitIds">ids of units in which we could search, if empty, skip this parameter</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Unit> GetUnits(string? searchFilter, Guid[]? unitIds, int page = 1, int pageSize = 25)
        {
            Console.WriteLine(unitIds.Length);
            DynamicParameters param = new();
            param.Add("@offset", (page - 1) * pageSize);
            param.Add("@pageSize", pageSize);
            var sql = "SELECT * FROM units WHERE  1=1";

            if (searchFilter != null)
            {
                sql+=(" AND (name LIKE @name OR [desc] LIKE @name)");
                param.Add("@name", $"%{searchFilter}%");

            }
            if (unitIds != null && unitIds.Length > 0)
            {
                sql+=(" AND id IN @ids");
                param.Add("@ids", unitIds);
            }
            sql += (" ORDER BY name ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;");
            return DBConnector.QueryDatabase<Unit>(sql, param);
        }

        public Unit? GetUnitById(Guid id)
        {
            var sql = "SELECT * FROM units WHERE id = @id;";
            return DBConnector.QueryDatabase<Unit>(sql, new { id }).FirstOrDefault();
        }

        public Unit CreateUnit(string name, decimal volumeEquivalent, string desc)
        {
            var sql = "INSERT INTO units (name, volumeEquivalent, [desc]) OUTPUT INSERTED.* VALUES (@name, @vol, @desc);";
            var list = DBConnector.QueryDatabase<Unit>(sql, new { name, vol = volumeEquivalent, desc}).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed");
        }

        /// <summary>
        /// update unit records with given data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="volumeEquivalent"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public Unit? UpdateUnit(Guid id, string? name, decimal? volumeEquivalent, string? desc)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();
            if (name != null) 
            { 
                sets.Add("name = @name"); parameters.Add("name", name); 
            }
            if (volumeEquivalent.HasValue) 
            { 
                sets.Add("volumeEquivalent = @vol"); parameters.Add("vol", volumeEquivalent.Value);
            }
            if(desc != null)
            {
                sets.Add("[desc] = @desc"); parameters.Add("desc", desc);
            }
            if (sets.Count == 0) return null;
            var sql = $"UPDATE units SET {string.Join(", ", sets)} WHERE id = @id; SELECT * FROM units WHERE id = @id;";
            parameters.Add("id", id);
            return DBConnector.QueryDatabase<Unit>(sql, parameters).FirstOrDefault();
        }

        public bool DeleteUnit(Guid id)
        {
            var sql = "DELETE FROM units WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id });
            return true;
        }

        /// <summary>
        /// Convert amount between units
        /// </summary>
        /// <param name="oldUnitId">id of reviouse unit</param>
        /// <param name="newUnitId">id of new unit</param>
        /// <param name="originalAmount">amount in old unit</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public UnitConvertResp ConvertUnit(Guid oldUnitId, Guid newUnitId, float originalAmount)
        {
            var sql  = "SELECT org.volumeEquivalent / toConv.volumeEquivalent newUnitAmount, toConv.* FROM units org JOIN units toConv ON toConv.id = @newId WHERE org.id = @oldId";
            var resp = DBConnector.QueryDatabase<UnitConvertResp>(sql, new { newId = newUnitId, oldId = oldUnitId });
            if (resp.FirstOrDefault() == null)
            {
                throw new InvalidOperationException("Conversion failed, check unit ids");
            }
            var newUnit = resp.FirstOrDefault();
            newUnit.newUnitAmount *= originalAmount;

            return newUnit;
        }
    }
}
