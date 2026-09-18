using Azure.Core;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace FoodApp.Utilities
{
    /// <summary>
    /// Class for authentication and authorization, with helpers
    /// </summary>
    public static class Authentication
    {
        private static int tokenExpiryMinutes;
        private static string secretKey;

        /// <summary>
        /// Initialize Authentication class
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiryMinutes"></param>
        public static void Initialize(string key)
        {
            secretKey = key;
            tokenExpiryMinutes = Constants.AuthTokenExpire/60;
        }

        /// <summary>
        /// Create and return new auth token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string CreateAuthToken(Guid userId)
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

        /// <summary>
        /// Hash password and return hased string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA384);
        }

        /// <summary>
        /// Hash token, for hashing password use HashPassword
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string HashToken(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        /// <summary>
        /// Check if password match hashed password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash, HashType.SHA384);
        }

        /// <summary>
        /// Generate random 6 digit code
        /// </summary>
        /// <returns></returns>
        public static string GenerateSixDigitCode()
        {
            var bytes = new byte[4];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint value = BitConverter.ToUInt32(bytes, 0);
            return (value % 1000000).ToString("D6");
        }

        private static Guid? GetUserIdFromToken(string token)
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
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);
                return userId;
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
        }
        
        /// <summary>
        /// Check if token is still valid
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static void ValidateToken(HttpRequest request)
        {
            try
            {
                string token = "";
                request.Cookies.TryGetValue(Constants.authHeader, out token);
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new UnauthorizedAccessException("You don't have permission to do this");
                }
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

        /// <summary>
        /// Return id of user from token header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static Guid? GetUserIdFromHeader(HttpRequest request)
        {
            string token = "";
            request.Cookies.TryGetValue(Constants.authHeader, out token);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }

            var userId = GetUserIdFromToken(token);
            
            return userId;
        }
    
        /// <summary>
        /// Generate random refresh token
        /// </summary>
        /// <returns></returns>
        public static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToHexString(bytes);
        }
    }
}

/// <summary>
/// Payload struct for auth token
/// </summary>
public struct AuthTokenPayload
{
    /// <summary>
    /// user id
    /// </summary>
    public Guid userId;
    /// <summary>
    /// expire date
    /// </summary>
    public long expiry;
}

/// <summary>
/// Header of auth token
/// </summary>
public struct AuthTokenHeader
{
    /// <summary>
    /// token type
    /// </summary>
    public string type;
    /// <summary>
    /// hash algorithm
    /// </summary>
    public string alg;

    /// <summary>
    /// 
    /// </summary>
    public AuthTokenHeader()
    {
        type = "JWT";
        alg = "HS256";
    }
}