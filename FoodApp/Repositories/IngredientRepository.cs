using Dapper;
using FoodApp.Models;
using FoodApp.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FoodApp.Repositories
{
    public interface IIngredientRepository
    {
        IEnumerable<Ingredient> GetIngredients(int? recipeId, int? foodId);
        Ingredient? GetIngredientById(int id);
        Ingredient CreateIngredient(int foodId, int unitId, decimal unitAmount, int recipeId);
        Ingredient? UpdateIngredient(int id, int? foodId, int? unitId, decimal? unitAmount, int? recipeId);
        bool DeleteIngredient(int id);
    }

    public class IngredientRepository : IIngredientRepository
    {
        public IEnumerable<Ingredient> GetIngredients(int? recipeId, int? foodId)
        {
            var sql = "SELECT * FROM ingredients WHERE 1=1";
            if (recipeId.HasValue) sql += " AND recipeId = @recipeId";
            if (foodId.HasValue) sql += " AND foodId = @foodId";
            sql += " ORDER BY id ASC;";
            return DBConnector.QueryDatabase<Ingredient>(sql, new { recipeId = recipeId, foodId = foodId });
        }

        public Ingredient? GetIngredientById(int id)
        {
            var sql = "SELECT * FROM ingredients WHERE id = @id;";
            return DBConnector.QueryDatabase<Ingredient>(sql, new { id = id }).FirstOrDefault();
        }

        public Ingredient CreateIngredient(int foodId, int unitId, decimal unitAmount, int recipeId)
        {
            var sql = "INSERT INTO ingredients (foodId, unitId, unitAmount, recipeId) OUTPUT INSERTED.* VALUES (@foodId, @unitId, @amt, @recipeId);";
            var list = DBConnector.QueryDatabase<Ingredient>(sql, new { foodId = foodId, unitId = unitId, amt = unitAmount, recipeId = recipeId }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed");
        }

        public Ingredient? UpdateIngredient(int id, int? foodId, int? unitId, decimal? unitAmount, int? recipeId)
        {
            var sets = new List<string>();
            var parameters = new DynamicParameters();
            if (foodId.HasValue) { sets.Add("foodId = @foodId"); parameters.Add("foodId", foodId.Value); }
            if (unitId.HasValue) { sets.Add("unitId = @unitId"); parameters.Add("unitId", unitId.Value); }
            if (unitAmount.HasValue) { sets.Add("unitAmount = @amt"); parameters.Add("amt", unitAmount.Value); }
            if (recipeId.HasValue) { sets.Add("recipeId = @recipeId"); parameters.Add("recipeId", recipeId.Value); }
            if (sets.Count == 0) return null;
            var sql = $"UPDATE ingredients SET {string.Join(", ", sets)} WHERE id = @id; SELECT * FROM ingredients WHERE id = @id;";
            parameters.Add("id", id);
            return DBConnector.QueryDatabase<Ingredient>(sql, parameters).FirstOrDefault();
        }

        public bool DeleteIngredient(int id)
        {
            var sql = "DELETE FROM ingredients WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
