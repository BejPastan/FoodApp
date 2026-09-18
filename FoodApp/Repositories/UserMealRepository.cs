using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// default implemenattion of IUserMealRepository
    /// </summary>
    public class UserMealRepository : IUserMealRepository
    {
        /// <inheritdoc/>
        public IEnumerable<UserMealWithData> GetUserMeals(Guid userId, DateOnly? startDate, DateOnly? endDate)
        {
            string sql = "SELECT us.id, us.mealDate, recipe.name, recipe.time, recipe.portion, recipe.id as recipeId, meal.name as meal FROM user_meals us LEFT JOIN recipe  ON recipe.id = recipeId LEFT JOIN meal ON meal.id = mealId WHERE userId = @userId ";
            if (startDate.HasValue) sql += " AND mealDate >= @startDate";
            if (endDate.HasValue) sql += " AND mealDate <= @endDate";
            sql += " ORDER BY mealDate ASC;";
            var response = DBConnector.QueryDatabase<UserMealWithData>(sql, new { userId, startDate, endDate });
            return response;
        }

        /// <inheritdoc/>
        public UserMeal? GetUserMealById(Guid id)
        {
            var sql = "SELECT * FROM user_meals WHERE id = @id;";
            return DBConnector.QueryDatabase<UserMeal>(sql, new { id }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public UserMeal CreateUserMeal(Guid userId, Guid recipeId, Guid mealId, DateOnly date, int portions = 1)
        {
            var sql = "INSERT INTO user_meals (userId, recipeId, mealId, mealDate, portions) OUTPUT INSERTED.* VALUES (@userId, @recipeId, @mealId, @mealDate, @portions);";
            var list = DBConnector.QueryDatabase<UserMeal>(sql, new { userId, recipeId, mealId, mealDate = date, portions }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed for user_meal");
        }

        /// <inheritdoc/>
        public bool DeleteUserMeal(Guid id)
        {
            var sql = "DELETE FROM user_meals WHERE id = @id;";
            DBConnector.QueryDatabase<Guid>(sql, new { id = id });
            return true;
        }

        /// <inheritdoc/>
        public int[] FindUserMealId(Guid userId, Guid mealId, DateOnly mealDate)
        {
            var sql = "SELECT id FROM user_meals WHERE userId = @userId AND mealId = @mealId AND mealDate = @mealDate;";
            var result = DBConnector.QueryDatabase<int>(sql, new { userId = userId, mealId = mealId, mealDate = mealDate }).ToArray();
            return result;
        }

        /// <inheritdoc/>
        public UserMeal UpdateUserMeal(Guid userMealId, Guid recipeId)
        {
            var sets = new List<string>();
            var parameters = new Dictionary<string, object>
            {
                { "id", userMealId },
                { "recipeId", recipeId }
            };

            var sql = $"UPDATE user_meals SET recipeId=@recipeId WHERE id = @id; SELECT * FROM user_meals WHERE id = @id;";
            return DBConnector.QueryDatabase<UserMeal>(sql, parameters).FirstOrDefault();
        }
    }
}
