using FoodApp.Utilities;

namespace FoodApp.Models
{
    public class User
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string password { get; set; }//this is the hashed password
        public string email { get; set; }
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

    /// <summary>
    /// model holding data for updating user record
    /// </summary>
    public class UserUpdateRequest
    {
        /// <summary>
        /// new name for user
        /// </summary>
        public string? name { get; set; }
        /// <summary>
        /// new email for user
        /// </summary>
        public string? email { get; set; }
    }

    /// <summary>
    /// model holding data required to start password reset
    /// </summary>
    public class PasswordResetStartRequest
    {
        /// <summary>
        /// email of user trying to reset password
        /// </summary>
        public required string email { get; set; }
    }

    /// <summary>
    /// Model with data for comfirming password reset
    /// </summary>
    public class PasswordResetConfirmRequest
    {
        /// <summary>
        /// token for password reset
        /// </summary>
        public required string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public required string newPassword { get; set; }
    }

    /// <summary>
    /// model holding data required to confirm sign up
    /// </summary>
    public class ConfirmSignUpRequest
    {
        /// <summary>
        /// token to confirm if user give real email
        /// </summary>
        public required string token { get; set; }
    }

    public class UserWithKitchen : ExtendedUser
    {
        public UserWithKitchen()
        {
            name = "";
            email = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        public UserWithKitchen(ExtendedUser org)
        {
            id = org.id;
            name = org.name??"";
            email = org.email??"";
            password = org.password??"";
            role = org.role;
            roleName = org.roleName??"";
        }
        /// <summary>
        /// details about kitchen
        /// </summary>
        public Kitchen kitchen { get; set; }
        /// <summary>
        /// details about hitchen user
        /// </summary>
        public KitchenUserDetailResponse kitchenUser { get; set; }
    }
}
