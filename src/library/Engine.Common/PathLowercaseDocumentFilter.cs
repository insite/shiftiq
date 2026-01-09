using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Engine.Common
{
    public class PathLowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument document, DocumentFilterContext context)
        {
            var oldPaths = document.Paths.ToDictionary(x => ToLowercase(x.Key), x => x.Value);
            var newPaths = new OpenApiPaths();
            foreach (var path in oldPaths)
                newPaths.Add(path.Key, path.Value);
            document.Paths = newPaths;
        }

        private static string ToLowercase(string key)
        {
            var parts = key.Split('/').Select(part => part.Contains("}") ? part : part.ToLowerInvariant());
            return string.Join('/', parts);
        }
    }
}
