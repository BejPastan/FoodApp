using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// Interface for managing user token
    /// </summary>
    public interface IUserTokenRepository
    {
        /// <summary>
        /// Add token or edit if exist already one for user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        UserToken AddToken(CreateUserTokenRequest request);
        UserToken? GetValidToken(string tokenValue);
       
        /// <summary>
        /// Change token state to used
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        bool MarkTokenAsUsed(Guid tokenId);

        /// <summary>
        /// Get user by valid token, if token is invalid return null
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        User GetUserByToken(string token);
    }

    /// <summary>
    /// IUserTokenRepo implementation
    /// </summary>
    public class UserTokenRepository : IUserTokenRepository
    {
        /// <inheritdoc/>
        public UserToken AddToken(CreateUserTokenRequest request)
        {
            var sql = @"MERGE INTO user_token AS target
                        USING (SELECT @userId, @token, @expirationDate) AS source (userId, token, expirationDate)
                            ON (target.userId = source.userId)

                        WHEN MATCHED THEN
                            UPDATE SET 
                                target.token = source.token,
                                target.expirationDate = source.expirationDate,
                                target.used = 0
                        WHEN NOT MATCHED THEN
                            INSERT (userId, token, expirationDate, used)
                            VALUES (source.userId, source.token, source.expirationDate, 0)
                        OUTPUT INSERTED.*;";

            var result = DBConnector.QueryDatabase<UserToken>(sql, new 
            { 
                token = request.token, 
                expirationDate = request.expirationDate, 
                used = request.used, 
                userId = request.userId 
            }).ToList();

            if (result.Count > 0)
            {
                return result[0];
            }
            
            throw new InvalidOperationException("Failed to insert user token.");
        }
        /// <inheritdoc/>
        public User GetUserByToken(string token)
        {
            var sql = "SELECT users.* FROM users JOIN user_token ut ON users.id = ut.userId WHERE ut.token = @token AND expirationDate>GETDATE() AND ut.used = 0";
            var parameters = new { token };
            var response = DBConnector.QueryDatabase<User>(sql, parameters);
            return response.FirstOrDefault();
        }


        /// <inheritdoc/>
        public UserToken? GetValidToken(string tokenValue)
        {
            var date = DateTime.UtcNow;
            Console.WriteLine(date);

            var sql = @"SELECT * FROM user_token
                        WHERE token = @Token AND used = 0 AND expirationDate >= @date;";

            return DBConnector.QueryDatabase<UserToken>(sql, new { Token = tokenValue ?? string.Empty, date }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool MarkTokenAsUsed(Guid tokenId)
        {
            var sql = @"UPDATE user_token SET used = 1 WHERE id = @Id;";
            DBConnector.QueryDatabase<Guid>(sql, new { Id = tokenId }).ToList();
            return true;
        }
    }
}