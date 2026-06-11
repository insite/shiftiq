using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shift.Api;

public class QuerySchemaFilter : ISchemaFilter
{
    private static readonly string[] HiddenProperties =
    [
        nameof(Query<object>.Filter),
        nameof(Query<object>.Identifier),
        nameof(IQueryByOrganization.OrganizationId),
        nameof(Query<object>.Origin),
        nameof(Query<object>.Texts)
    ];

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
            return;

        foreach (var name in HiddenProperties)
        {
            var match = schema.Properties.Keys.FirstOrDefault(key => string.Equals(key, name, StringComparison.OrdinalIgnoreCase));

            if (match != null)
                schema.Properties.Remove(match);
        }
    }
}
