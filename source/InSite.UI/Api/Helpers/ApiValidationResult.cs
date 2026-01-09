using InSite.Persistence.Integration;

namespace InSite.Api.Settings
{
    public class ApiValidationResult
    {
        public ApiValidationResult(bool success, string error)
        {
            Success = success;
            Error = error;
        }

        public bool Success { get; }
        public string Error { get; }

        public ApiDeveloper Developer { get; set; }

        public bool IsLocal { get; internal set; }
        public string OrganizationCode { get; set; }
        public string HostAddress { get; set; }
    }
}