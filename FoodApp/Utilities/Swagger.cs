using Microsoft.OpenApi.Models;

namespace FoodApp.Utilities
{
    public static class Swagger
    {
        public static void AddSwaggerDocumentation(ref WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FoodApp API",
                    Version = "v1"
                });
            });
        }

        public static void UseSwaggerDocumentation(ref WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodApp v1");
                // Serve Swagger UI at application root: https://localhost:<port>/
                c.RoutePrefix = string.Empty;
            });
        }


    }
}
