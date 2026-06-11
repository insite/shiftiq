using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shift.Api;

public class AliasOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var alias = context.MethodInfo
            .GetCustomAttributes(typeof(AliasForAttribute), false)
            .FirstOrDefault() as AliasForAttribute;

        if (alias != null)
        {
            operation.Extensions.Add("x-alias-for", new OpenApiString(alias.AliasFor));
        }
    }
}
