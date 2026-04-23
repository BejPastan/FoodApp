using FoodApp.Utilities;

namespace FoodApp.Models
{
    public class User
    {
        public int id { get; set; }
        public required string name { get; set; }
        public string password { get; set; }//this is the hashed password
        public required string email { get; set; }
        public DateTime? lastLogin { get; set; }
        public UserStatus userStatus { get; set; }

        public override string ToString()
        {
            return $"User: {name} (ID: {id}, Email: {email}, Last Login: {lastLogin}, status: {userStatus})";
        }
    }

    public class SignUpRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class ExtendedUser: User
    {
        public Roles role { get; set; }
        public string roleName { get; set; }

        public override string ToString()
        {
            return $"User: {name} (ID: {id}, Email: {email}, Last Login: {lastLogin}), role: ({role})";
        }
    }

    public class UserUpdateRequest
    {
        public string? name { get; set; }
        public string? email { get; set; }
    }

    public class PasswordResetStartRequest
    {
        public string email { get; set; }
    }

    public class PasswordResetConfirmRequest
    {
        public string token { get; set; }
        public string newPassword { get; set; }
    }

    public class ConfirmSignUpRequest
    {
        public string token { get; set; }
    }
}
