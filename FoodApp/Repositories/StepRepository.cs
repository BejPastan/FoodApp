using Dapper;
using FoodApp.Models;
using FoodApp.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FoodApp.Repositories
{
    public interface IStepRepository
    {
        IEnumerable<Step> GetSteps(int? recipeId);
        Step? GetStepById(int id);
        Step CreateStep(int recipeId, string instruction, int stepNumber);
        Step? UpdateStep(int id, int? recipeId, string? instruction, int? stepNumber);
        bool DeleteStep(int id);
    }

    public class StepRepository : IStepRepository
    {
        public IEnumerable<Step> GetSteps(int? recipeId)
        {
            var sql = "SELECT * FROM steps WHERE 1=1";
            if (recipeId.HasValue) sql += " AND recipeId = @recipeId";
            sql += " ORDER BY id ASC;";
            return DBConnector.QueryDatabase<Step>(sql, new { recipeId = recipeId });
        }

        public Step? GetStepById(int id)
        {
            var sql = "SELECT * FROM steps WHERE id = @id;";
            return DBConnector.QueryDatabase<Step>(sql, new { id = id }).FirstOrDefault();
        }

        public Step CreateStep(int recipeId, string instruction, int stepNumber)
        {
            var sql = "INSERT INTO steps (recipeId, instruction, stepNumber) OUTPUT INSERTED.* VALUES (@recipeId, @instr, @stepNum);";
            var list = DBConnector.QueryDatabase<Step>(sql, new { recipeId = recipeId, instr = instruction ?? string.Empty , stepNum = stepNumber}).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed");
        }

        public Step? UpdateStep(int id, int? recipeId, string? instruction, int? stepNumber)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();
            if (recipeId.HasValue) 
            {
                sets.Add("recipeId = @recipeId"); 
                parameters.Add("recipeId", recipeId.Value); 
            }
            if (instruction != null)
            {
                sets.Add("instruction = @instr");
                parameters.Add("instr", instruction); 
            }
            if(stepNumber != null)
            {
                sets.Add("stepNumber = @stepNumber");
                parameters.Add("stepNumber", stepNumber);
            }

            if (sets.Count == 0) return null;
            var sql = $"UPDATE steps SET {string.Join(", ", sets)} WHERE id = @id; SELECT * FROM steps WHERE id = @id;";
            parameters.Add("id", id);
            return DBConnector.QueryDatabase<Step>(sql, parameters).FirstOrDefault();
        }

        public bool DeleteStep(int id)
        {
            var sql = "DELETE FROM steps WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
