using FoodApp.Models;
using FoodApp.Repositories.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    /// <summary>
    /// Basic implementation of IRefreshTokenRepo
    /// </summary>
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        public User GetUserByToken(string hashedToken)
        {
            var sql = """
                SELECT u.* FROM refresh_token rt JOIN users u ON rt.userId=u.id WHERE rt.token = @token;
                """;
            var user = DBConnector.QueryDatabase<User>(sql, new {token = hashedToken});
            return user.FirstOrDefault();
        }

        /// <inheritdoc/>
        public bool IsRefreshTokenValid(Guid userId, string hashedToken)
        {
            var sql = "SELECT * FROM refresh_token WHERE userId = @userId AND token=@token AND used = 0 AND expirationDate>GETDATE()";
            var token = DBConnector.QueryDatabase<FullRefreshToken>(sql, new { userId = userId, token = hashedToken }).FirstOrDefault();
            if (token?.userId == userId) return true;
            else return false;
        }

        /// <inheritdoc/>
        public FullRefreshToken SetRefreshToken(RefreshTokenData toAdd)
        {
            var sql = """
                MERGE INTO refresh_token AS target
                USING (VALUES (@userId)) AS source (userId)
                	ON target.userId = source.userId

                WHEN MATCHED THEN
                	UPDATE SET
                        userId = @userId
                """;
            if (toAdd.token != null)
            {
                sql += ", token = @token";
            }
            if (toAdd.expirationDate != null)
            {
                sql += ", expirationDate = @expirationDate";
            }
            var usedVal = 0;
            if (toAdd.used != null)
            {
                sql += ", used = @used";
                usedVal = toAdd.used.Value ? 1 : 0;
            }

            sql += """
                 WHEN NOT MATCHED THEN
                    INSERT(token, expirationDate, userId)
                    VALUES(@token, @expirationDate, @userId);
                SELECT * FROM refresh_token WHERE userId = @userId;
                """;
            
            var token = DBConnector.QueryDatabase<FullRefreshToken>(sql, new { userId = toAdd.userId, token = toAdd.token, expirationDate = toAdd.expirationDate, used = usedVal }).FirstOrDefault();
            return token;
        }

    }
}
