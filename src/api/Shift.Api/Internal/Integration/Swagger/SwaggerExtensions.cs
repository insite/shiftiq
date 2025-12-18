using System.Reflection;

using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace Shift.Api;

public static class SwaggerExtensions
{
    public static void AddDocumentation(this IServiceCollection services, string branding, string developerDocsUrl)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Version = "v2.0.0",

                Title = "Developer API",

                Description = $"This is the library of application programming interfaces for developers and integrators working with the <strong>{branding}</strong> learning management system. It provides access to all parts of the platform, including courses, learners, enrollments, assessments, achievements, competencies, and more.",

                Contact = new OpenApiContact
                {
                    Name = "Developer Documentation",
                    Url = new Uri(developerDocsUrl)
                },
            });

            c.DocumentFilter<PathLowercaseDocumentFilter>();

            // By Default, all endpoints are grouped by the controller name. We want to group first
            // by "Group Name", then by controller name (if no group is provided).
            c.TagActionsBy((api) => [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"]]);

            // Include all endpoints available in the documentation.
            c.DocInclusionPredicate((docName, apiDesc) => { return true; });

            // Include XML comments from source code.
            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            c.IncludeXmlComments(xmlCommentsPath);

            // Add security definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = AuthenticationSchemeNames.Bearer,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "Bearer",
                Description = "-"
            });

            // Add security definition
            c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
            {
                Scheme = AuthenticationSchemeNames.Cookie,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = "InSite.WebToken",
                Description = "-"
            });

            // Add security definition
            c.AddSecurityDefinition("Secret", new OpenApiSecurityScheme
            {
                Scheme = AuthenticationSchemeNames.Secret,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "Secret",
                Description = "-"
            });

            // Add the custom operation filter to handle Authorize attributes
            c.OperationFilter<AuthorizeOperationFilter>();

            // Add the custom operation filter to handle Alias attributes
            c.OperationFilter<AliasOperationFilter>();
        });
    }

    public static void UseDocumentation(this WebApplication app, ReleaseSettings release)
    {
        var environment = release.GetEnvironment();

        var isLocal = environment.IsLocal();

        var publicContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Public");

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(publicContentPath)
        });

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.DefaultModelsExpandDepth(-1);

            options.SwaggerEndpoint("v2/swagger.json", release.Brand + " API v2");

            options.InjectStylesheet("../swagger-ui/custom.css");

            options.InjectJavascript("../swagger-ui/custom.js");
        });
    }
}
