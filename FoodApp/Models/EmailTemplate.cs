namespace FoodApp.Models
{
    public class EmailTemplate
    {
        public int id { get; set; }
        public string name { get; set; }
        public string template { get; set; }
        public string subject {get; set;}
        
        public override string ToString()
        {
            return $"Id: {id}, Name: {name}, Template length: {template?.Length ?? 0}";
        }
    }

    public class CreateEmailTemplateRequest
    {
        public string name { get; set; }
        public string template { get; set; }
        public string subject {get; set;}
    }
}