namespace Engine.Common
{
    internal class ApiConfigurationResult : ApiStatusResult
    {
        public required object Configuration { get; set; }
    }
}
