using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    public interface IEmailTemplateRepository
    {
        EmailTemplate? GetTemplate(Guid? id, string? name);
        EmailTemplate AddTemplate(CreateEmailTemplateRequest request);
    }
}