using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace Engine.Common
{
    internal static class SwaggerExtensions
    {
        public static void AddDocumentation(this IServiceCollection services, IEngineConfiguration config)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0.0",
                    Title = $"{config.AppName}"
                });

                c.DocumentFilter<PathLowercaseDocumentFilter>();

                // By Default, all endpoints are grouped by the controller name. We want to group first
                // by "Group Name", then by controller name (if no group is provided).
                c.TagActionsBy((api) => [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"]]);

                // Include all endpoints available in the documentation.
                c.DocInclusionPredicate((docName, apiDesc) => { return true; });

                // Include XML comments from source code.
                var xmlCommentsPath = Path.Combine(AppContext.BaseDirectory, $"{config.AssemblyName}.xml");
                if (File.Exists(xmlCommentsPath))
                    c.IncludeXmlComments(xmlCommentsPath);

                // Add security definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

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
                        new string[]{}
                    }
                });
            });
        }

        public static void UseDocumentation(this WebApplication app, IEngineConfiguration config)
        {
            var publicContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Public");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(publicContentPath)
            });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
                options.DefaultModelsExpandDepth(-1);
                options.SwaggerEndpoint("v1/swagger.json", config.AppName + " API v1");
                options.InjectStylesheet("../swagger-ui/custom.css");
            });
        }
    }
}
