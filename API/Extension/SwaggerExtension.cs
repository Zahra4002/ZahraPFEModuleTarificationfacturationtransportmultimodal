using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Any;

namespace API.Extension
{
        public static class SwaggerExtension
        {
            public static void ConfigureSwagger(this IServiceCollection services)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "TarificationFacturation.Api",
                        Version = "v1",
                        Description = "TarificationFacturation Web Api",
                    });

                    // Configuration du bouton Authorize pour JWT
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    // Configuration pour afficher les noms des enums
                    c.SchemaFilter<EnumSchemaFilter>();



                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
                });
            }
        }

        // Classe pour afficher les noms des énumérations au lieu des valeurs numériques
        public class EnumSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                if (context.Type.IsEnum)
                {
                    schema.Enum.Clear();
                    Enum.GetNames(context.Type)
                        .ToList()
                        .ForEach(name => schema.Enum.Add(new OpenApiString(name)));

                    schema.Type = "string";
                    schema.Format = null;
                }
            }
        }
    }