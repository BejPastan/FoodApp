using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IUserMealRepository
    {
        IEnumerable<UserMealWithData> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate);
        UserMeal? GetUserMealById(int id);
        UserMeal CreateUserMeal(int userId, int recipeId, int mealId, DateTime date);
        bool DeleteUserMeal(int id);
    }

    public class UserMealRepository : IUserMealRepository
    {
        public IEnumerable<UserMealWithData> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate)
        {
            string sql = "SELECT us.mealDate, recipe.name, recipe.time, recipe.portion, recipe.id as recipeId, meal.name as meal FROM user_meals us LEFT JOIN recipe  ON recipe.id = recipeId LEFT JOIN meal ON meal.id = mealId WHERE userId = @userId ";
            if (startDate.HasValue) sql += " AND mealDate >= @startDate";
            if (endDate.HasValue) sql += " AND mealDate <= @endDate";
            sql += " ORDER BY mealDate ASC;";
            return DBConnector.QueryDatabase<UserMealWithData>(sql, new { userId = userId, startDate = startDate, endDate = endDate });
        }

        public UserMeal? GetUserMealById(int id)
        {
            var sql = "SELECT * FROM user_meals WHERE id = @id;";
            return DBConnector.QueryDatabase<UserMeal>(sql, new { id = id }).FirstOrDefault();
        }

        public UserMeal CreateUserMeal(int userId, int recipeId, int mealId, DateTime date)
        {
            var sql = "INSERT INTO user_meals (userId, recipeId, mealId, mealDate) OUTPUT INSERTED.* VALUES (@userId, @recipeId, @mealId, @date);";
            var list = DBConnector.QueryDatabase<UserMeal>(sql, new { userId = userId, recipeId = recipeId, mealId = mealId, date = date }).ToList();
            if (list.Count > 0) return list[0];
            throw new Exception("Insert failed for user_meal");
        }

        public bool DeleteUserMeal(int id)
        {
            var sql = "DELETE FROM user_meals WHERE id = @id;";
            DBConnector.QueryDatabase<int>(sql, new { id = id }).ToList();
            return true;
        }
    }
}
