namespace Shift.Hub
{
    internal class ApiConfigurationResult : ApiStatusResult
    {
        public required object Configuration { get; set; }
    }
}
