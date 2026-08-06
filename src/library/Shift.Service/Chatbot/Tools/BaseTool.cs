using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Shift.Common;

namespace Shift.Service.Chatbot.Tools;

abstract class BaseTool<ArgsType, ResultType>(string name, string description, string? resource) : ITool where ResultType : notnull
{
    protected record InternalToolResponse(ResultType ToLLM, string? DirectText = null, string? Table = null);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly JsonSchemaExporterOptions ExporterOptions = new()
    {
        TransformSchemaNode = (context, node) =>
        {
            if (context.PropertyInfo != null && node is JsonObject jsonObject)
            {
                var descriptionAttr = context.PropertyInfo.AttributeProvider?
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .OfType<DescriptionAttribute>()
                    .FirstOrDefault();

                if (descriptionAttr != null)
                    jsonObject["description"] = descriptionAttr.Description;
            }
            return node;
        },
        TreatNullObliviousAsNonNullable = true
    };

    public string Name => name;
    public string Description => description;
    public ITool[] Tools => [];

    public bool IsAllowed(ToolContext toolContext)
    {
        return string.IsNullOrEmpty(resource)
            || toolContext.PermissionCache.Matrix.IsAllowed(toolContext.Principal.Organization.Slug, resource, toolContext.Roles, DataAccess.Read);
    }
    
    public JsonNode CreateArgsSchema()
    {
        return SerializerOptions.GetJsonSchemaAsNode(typeof(ArgsType), ExporterOptions);
    }

    public async Task<ToolResponse> ExecuteAsync(ToolContext toolContext, BinaryData args)
    {
        if (!IsAllowed(toolContext))
            throw new NotSupportedException();

        var deserialized = ParseArgs(args);

        var result = await InternalExecuteAsync(toolContext, deserialized);

        var toLLM = typeof(ResultType) == typeof(string)
            ? (string)(object)result.ToLLM
            : JsonSerializer.Serialize(result.ToLLM, SerializerOptions);

        return new ToolResponse(toLLM, result.DirectText, result.Table);
    }

    protected abstract Task<InternalToolResponse> InternalExecuteAsync(ToolContext toolContext, ArgsType? args);

    private static ArgsType? ParseArgs(BinaryData args)
    {
        return JsonSerializer.Deserialize<ArgsType>(args, SerializerOptions);
    }
}