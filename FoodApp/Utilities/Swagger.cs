using FoodApp.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;

using Microsoft.AspNetCore.Authentication.JwtBearer;

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

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                };

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[]{}
                    } 
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
