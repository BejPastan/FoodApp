using FoodApp.Models;
using FoodApp.Utilities;

namespace FoodApp.Repositories
{
    public interface IEmailTemplateRepository
    {
        EmailTemplate? GetTemplate(Guid? id, string? name);
        EmailTemplate AddTemplate(CreateEmailTemplateRequest request);
    }

    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        public EmailTemplate? GetTemplate(Guid? id, string? name)
        {
            var parameters = new { Id = id, Name = name ?? string.Empty };
            
            var sql = @"SELECT * FROM email_template WHERE ";
            if(id.HasValue)
            {
                sql+= " id = @Id;";
            }
            if(!String.IsNullOrEmpty(name))
            {
                sql+= " name = @Name;";
            }
            var emailTemplate = DBConnector.QueryDatabase<EmailTemplate>(sql, parameters);
            return emailTemplate.FirstOrDefault();
        }

        public EmailTemplate AddTemplate(CreateEmailTemplateRequest request)
        {
            var sql = @"INSERT INTO email_template (name, template, subject) 
                        OUTPUT INSERTED.* 
                        VALUES (@Name, @Template, @Subject);";

            var result = DBConnector.QueryDatabase<EmailTemplate>(sql, new 
            { 
                Name = request.name, 
                Template = request.template,
                Subject = request.subject 
            }).ToList();

            if (result.Count > 0)
            {
                return result[0];
            }
            
            throw new InvalidOperationException("Failed to insert email template.");
        }
    }
}