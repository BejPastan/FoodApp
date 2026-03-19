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
        int FindUserMealId(int userId, int mealId, DateTime mealDate);
        UserMeal? UpdateUserMeal(int id, int? userId, int? mealId, DateTime? mealDate);
    }

    public class UserMealRepository : IUserMealRepository
    {
        public IEnumerable<UserMealWithData> GetUserMeals(int userId, DateTime? startDate, DateTime? endDate)
        {
            Console.WriteLine(userId);
            Console.WriteLine(startDate);
            Console.WriteLine(endDate);
            string sql = "SELECT us.mealDate, recipe.name, recipe.time, recipe.portion, recipe.id as recipeId, meal.name as meal FROM user_meals us LEFT JOIN recipe  ON recipe.id = recipeId LEFT JOIN meal ON meal.id = mealId WHERE userId = @userId ";
            if (startDate.HasValue) sql += " AND mealDate >= @startDate";
            if (endDate.HasValue) sql += " AND mealDate <= @endDate";
            sql += " ORDER BY mealDate ASC;";
            var response = DBConnector.QueryDatabase<UserMealWithData>(sql, new { userId = userId, startDate = startDate.Value.Date, endDate = endDate.Value.Date });
            return response;
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
    
        /// <summary>
        /// Finds a user meal by user ID, meal ID, and meal date
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="mealId">The ID of the meal</param>
        /// <param name="mealDate">The date of the meal</param>
        /// <returns>return id or -1 if not found</returns>
        public int FindUserMealId(int userId, int mealId, DateTime mealDate)
        {
            var sql = "SELECT id FROM user_meals WHERE userId = @userId AND mealId = @mealId AND mealDate = @mealDate;";
            var result = DBConnector.QueryDatabase<int>(sql, new { userId = userId, mealId = mealId, mealDate = mealDate.Date }).FirstOrDefault();
            return result > 0 ? result : -1;
        }

        /// <summary>
        /// Updates a user meal record with the provided optional fields
        /// </summary>
        /// <param name="id">The ID of the user meal to update</param>
        /// <param name="userId">Optional new user ID</param>
        /// <param name="mealId">Optional new meal ID</param>
        /// <param name="mealDate">Optional new meal date</param>
        /// <returns>Updated UserMeal record or null if no fields to update</returns>
        public UserMeal? UpdateUserMeal(int id, int? userId, int? mealId, DateTime? mealDate)
        {
            var sets = new List<string>();
            var parameters = new Dictionary<string, object> { { "id", id } };
            
            if (userId.HasValue) 
            { 
                sets.Add("userId = @userId"); 
                parameters.Add("userId", userId.Value); 
            }
            if (mealId.HasValue) 
            { 
                sets.Add("mealId = @mealId"); 
                parameters.Add("mealId", mealId.Value); 
            }
            if (mealDate.HasValue) 
            { 
                sets.Add("mealDate = @mealDate"); 
                parameters.Add("mealDate", mealDate.Value.Date); 
            }
            
            if (sets.Count == 0) return null;
            
            var sql = $"UPDATE user_meals SET {string.Join(", ", sets)} WHERE id = @id; SELECT * FROM user_meals WHERE id = @id;";
            return DBConnector.QueryDatabase<UserMeal>(sql, parameters).FirstOrDefault();
        }
    }
}
