using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IUserTokenRepository
    {
        UserToken AddToken(CreateUserTokenRequest request);
        UserToken? GetValidToken(string tokenValue);
        bool MarkTokenAsUsed(int tokenId);
    }

    public class UserTokenRepository : IUserTokenRepository
    {
        /// <summary>
        /// Add token or edit if exist already one for user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public UserToken AddToken(CreateUserTokenRequest request)
        {
            var sql = @"MERGE INTO user_token AS target
                        USING (SELECT @userId, @token, @expirationDate) AS source (userId, token, expirationDate)
                            ON (target.userId = source.userId)

                        WHEN MATCHED THEN
                            UPDATE SET 
                                target.token = source.token,
                                target.expirationDate = source.expirationDate,
                                target.used = 1
                        WHEN NOT MATCHED THEN
                            INSERT (userId, token, expirationDate, used)
                            VALUES (source.userId, source.token, source.expirationDate, 0)
                        OUTPUT INSERTED.*;";

            var result = DBConnector.QueryDatabase<UserToken>(sql, new 
            { 
                Token = request.token, 
                ExpirationDate = request.expirationDate, 
                Used = request.used, 
                UserId = request.userId 
            }).ToList();

            if (result.Count > 0)
            {
                return result[0];
            }
            
            throw new InvalidOperationException("Failed to insert user token.");
        }

        public UserToken? GetValidToken(string tokenValue)
        {
            var sql = @"SELECT * FROM user_tokens 
                        WHERE token = @Token AND used = 0 AND expirationDate > GETUTCDATE();";

            return DBConnector.QueryDatabase<UserToken>(sql, new { Token = tokenValue ?? string.Empty }).FirstOrDefault();
        }

        public bool MarkTokenAsUsed(int tokenId)
        {
            var sql = @"UPDATE user_tokens SET used = 1 WHERE id = @Id;";
            DBConnector.QueryDatabase<int>(sql, new { Id = tokenId }).ToList();
            return true;
        }
    }
}