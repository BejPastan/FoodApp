namespace FoodApp.Models
{
    public class User
    {
        public int id { get; set; }
        public required string name { get; set; }
        public required string password { get; set; }//this is the hashed password
        public required string email { get; set; }
        public DateTime? last_login { get; set; }

        public override string ToString()
        {
            return $"User: {name} (ID: {id}, Email: {email}, Last Login: {last_login})";
        }
    }
}
