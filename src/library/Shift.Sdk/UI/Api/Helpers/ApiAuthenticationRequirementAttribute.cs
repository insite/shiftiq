using System;

namespace Shift.Sdk.UI
{
    public enum ApiAuthenticationType
    {
        None,    // Unauthenticated requests are permitted. Use with caution!
        Request, // The HTTP request must be validated.
        Jwt,     // A valid JSON Web Tokens is required.
        Header,  // A user/developer token is required in the HTTP request header. This is our old default approach.
        Cookie   // The request is authenticated by security cookie and session
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAuthenticationRequirementAttribute : Attribute
    {
        public ApiAuthenticationType Type { get; set; }

        public ApiAuthenticationRequirementAttribute(ApiAuthenticationType type)
        {
            Type = type;
        }
    }
}