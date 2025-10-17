using Shift.Common;

namespace Engine.Common
{
    internal class ApiStatusResult
    {
        public required string Status { get; set; }
        public required string Version { get; set; }
        public required EnvironmentModel Environment { get; set; }
    }
}
