using Newtonsoft.Json.Serialization;

namespace Shift.Api;

public class ApiContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1);
    }
}