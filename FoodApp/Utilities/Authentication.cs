using Azure.Core;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
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
                { JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(tokenExpiryMinutes).ToUnixTimeSeconds() }
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

        public static string GenerateSixDigitCode()
        {
            var bytes = new byte[4];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint value = BitConverter.ToUInt32(bytes, 0);
            return (value % 1000000).ToString("D6");
        }

        private static int? GetUserIdFromToken(string token)
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
                Console.WriteLine($"jwt token:{jwtToken}");
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);
                return userId;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
        }
    
        public static void ValidateToken(HttpRequest request)
        {
            try
            {
                var token = request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
                {
                    throw new UnauthorizedAccessException("You don't have permission to do this");
                }
                token = token.Substring(7);
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
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
        }

        public static int? GetUserIdFromHeader(HttpRequest request)
        {
            var token = request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }

            token = token.Substring(7); // Remove "Bearer " prefix
            var userId = GetUserIdFromToken(token);
            
            return userId;
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