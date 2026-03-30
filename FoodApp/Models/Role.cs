using FoodApp.Utilities;

namespace FoodApp.Models
{
    public class Role
    {
        public Roles name { get; set; } = Roles.user;
        public int id { get; set; }

        public override string ToString()
        {
            return $"id: {id}, name: {name}";
        }
    }

    public class RoleCreateRequest
    {
        public Roles name { get; set; } = Roles.user;
    }

    public class RoleUpdateRequest
    {
        public Roles? name { get; set; }
    }
}
