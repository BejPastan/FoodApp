using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FoodApp.Utilities
{
    public static class Authentication
    {
        private static int tokenExpiryMinutes;
        private static string secretKey;

        public static void Initialize(string key, int expiryMinutes)
        {
            secretKey = key;
            tokenExpiryMinutes = expiryMinutes;
        }

        public static string CreateAuthToken(int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtHeader header = new(credentials);
            JwtPayload payload = new()
            {
                { "userId", userId },
                { "expiry", DateTimeOffset.Now.AddMinutes(tokenExpiryMinutes).ToUnixTimeSeconds() }
            };

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA384);
        }

        public static bool CheckPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash, HashType.SHA384);
        }

        public static int? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}

public struct AuthTokenPayload
{
    public int userId;
    public long expiry;
}

public struct AuthTokenHeader
{
    public string type;
    public string alg;

    public AuthTokenHeader()
    {
        type = "JWT";
        alg = "HS256";
    }
}
