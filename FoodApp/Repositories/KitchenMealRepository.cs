using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// default implemenattion of IUserMealRepository
    /// </summary>
    public class KitchenMealRepository : IKitchenMealRepository
    {
        /// <inheritdoc/>
        public IEnumerable<KitchenMealWithData> GetKitchenMeals(Guid kitchenId, DateOnly? startDate, DateOnly? endDate)
        {
            string sql = "SELECT us.id, us.mealDate, recipe.name, recipe.time, recipe.portion, recipe.id as recipeId, meal.name as meal FROM user_meals us LEFT JOIN recipe ON recipe.id = recipeId LEFT JOIN meal ON meal.id = mealId WHERE kitchenId = @kitchenId ";
            if (startDate.HasValue) sql += " AND mealDate >= @startDate";
            if (endDate.HasValue) sql += " AND mealDate <= @endDate";
            sql += " ORDER BY mealDate ASC;";
            var response = DBConnector.QueryDatabase<KitchenMealWithData>(sql, new { kitchenId, startDate, endDate });
            return response;
        }

        /// <inheritdoc/>
        public KitchenMeal? GetKitchenMealById(Guid id)
        {
            var sql = "SELECT * FROM user_meals WHERE id = @id;";
            return DBConnector.QueryDatabase<KitchenMeal>(sql, new { id }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public KitchenMeal CreateKitchenMeal(Guid kitchenId, Guid recipeId, Guid mealId, DateOnly date, int portions = 1)
        {
            var sql = "INSERT INTO user_meals (kitchenId, recipeId, mealId, mealDate, portions) OUTPUT INSERTED.* VALUES (@kitchenId, @recipeId, @mealId, @mealDate, @portions);";
            var list = DBConnector.QueryDatabase<KitchenMeal>(sql, new { kitchenId, recipeId, mealId, mealDate = date, portions }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed for user_meal");
        }

        /// <inheritdoc/>
        public bool DeleteKitchenMeal(Guid id)
        {
            var sql = "DELETE FROM user_meals WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id = id });
            return true;
        }

        /// <inheritdoc/>
        public int[] FindKitchenMealId(Guid kitchenId, Guid mealId, DateOnly mealDate)
        {
            var sql = "SELECT id FROM user_meals WHERE kitchenId = @kitchenId AND mealId = @mealId AND mealDate = @mealDate;";
            var result = DBConnector.QueryDatabase<int>(sql, new { kitchenId = kitchenId, mealId = mealId, mealDate = mealDate }).ToArray();
            return result;
        }

        /// <inheritdoc/>
        public KitchenMeal UpdateKitchenMeal(Guid userMealId, Guid recipeId)
        {
            var sets = new List<string>();
            var parameters = new Dictionary<string, object>
            {
                { "id", userMealId },
                { "recipeId", recipeId }
            };

            var sql = $"UPDATE user_meals SET recipeId=@recipeId WHERE id = @id; SELECT * FROM user_meals WHERE id = @id;";
            return DBConnector.QueryDatabase<KitchenMeal>(sql, parameters).FirstOrDefault();
        }

        /// <inheritdoc/>
        public RecipeRecord[] GetRecipesToChoose(Guid userId, Guid mealId, int excludedWeeks)
        {
            DateTime cutOffDate = DateTime.Now.AddDays(-excludedWeeks * 7);
            string sql = "With recipe_usage AS (Select rm.recipeId, COUNT(um.recipeId) eaten from recipe_meal rm JOIN meal ON meal.id = rm.mealId LEFT JOIN user_meals um ON um.recipeId = rm.recipeId AND um.mealDate >= @cutDate AND um.userId = @userId WHERE meal.id = @mealId GROUP BY rm.recipeId),ranked_recipes AS (SELECT TOP(3) ROW_NUMBER() OVER(ORDER BY ru.eaten asc, NEWID()) as rank_order, ru.recipeId as recipeId FROM recipe_usage ru ORDER BY rank_order) SELECT * FROM recipe WHERE recipe.id IN(SELECT ranked_recipes.recipeId FROM ranked_recipes);";
            Recipe[] recipes = DBConnector.QueryDatabase<Recipe>(sql, new { cutDate = cutOffDate, mealId = mealId, userId = userId }).ToArray();
            Console.WriteLine($"recipeCount in repo: {recipes.Length}");
            return recipes;
        }

        /// <inheritdoc/>
        public Kitchen? GetKitchenByMealId(Guid mealId)
        {
            string sql = "SELECT k.* FROM user_meals um JOIN Kitchen k ON k.id = um.kitchenId WHERE um.id = @mealId";
            return DBConnector.QueryDatabase<Kitchen>(sql, new { mealId = mealId }).FirstOrDefault();
        }
    }
}
