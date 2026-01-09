using Newtonsoft.Json.Linq;

using Shift.Common;

namespace Shift.Api;

public class OpenApiService
{
    private readonly HttpClient _httpClient;

    public OpenApiService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    /// <summary>
    /// Fetches OpenAPI specification from a URL and returns endpoints with routes containing the specified path
    /// </summary>
    /// <param name="openApiUrl">URL to the OpenAPI JSON specification</param>
    /// <param name="routePath">The path to filter routes (e.g., "/user")</param>
    /// <returns>List of endpoints matching the route path</returns>
    public async Task<List<OpenApiEndpoint>> GetEndpointsByRoutePathAsync(string openApiUrl, string? routePath)
    {
        try
        {
            // Fetch the OpenAPI specification
            var response = await _httpClient.GetAsync(openApiUrl);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadAsStringAsync();
            var openApiSpec = JObject.Parse(jsonContent);

            var endpoints = new List<OpenApiEndpoint>();

            // Parse the paths section
            var paths = openApiSpec["paths"];
            if (paths == null) return endpoints;

            foreach (var path in paths.Cast<JProperty>())
            {
                var route = path.Name; // The route/path

                // Check if route contains the specified path
                if (routePath != null && !route.Contains(routePath, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Parse each HTTP method for this path
                var pathObject = path.Value as JObject;
                if (pathObject == null) continue;

                foreach (var method in pathObject)
                {
                    var httpMethod = method.Key.ToUpper();

                    // Skip non-HTTP method properties
                    if (!IsHttpMethod(httpMethod)) continue;

                    var operation = method.Value as JObject;
                    if (operation == null) continue;

                    var endpoint = new OpenApiEndpoint
                    {
                        Route = route,
                        Method = httpMethod,
                        Summary = operation["summary"]?.ToString() ?? "",
                        Description = operation["description"]?.ToString() ?? "",
                        OperationId = operation["operationId"]?.ToString() ?? ""
                    };

                    // Extract authorization information
                    ExtractAuthorizationInfo(endpoint, operation, openApiSpec);

                    endpoints.Add(endpoint);
                }
            }

            return endpoints.OrderBy(e => e.Route).ThenBy(e => e.Method).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching or parsing OpenAPI specification: {ex.Message}", ex);
        }
    }

    private static bool IsHttpMethod(string method)
    {
        var httpMethods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS", "TRACE" };
        return httpMethods.Contains(method);
    }

    /// <summary>
    /// Extracts authorization information from the OpenAPI operation
    /// </summary>
    private void ExtractAuthorizationInfo(OpenApiEndpoint endpoint, JObject operation, JObject openApiSpec)
    {
        // Check operation-level security requirements
        var security = operation["security"];
        if (security != null)
        {
            ProcessSecurityRequirements(endpoint, security, openApiSpec);
        }
        else
        {
            // Fall back to global security requirements if no operation-level security
            var globalSecurity = openApiSpec["security"];
            if (globalSecurity != null)
            {
                ProcessSecurityRequirements(endpoint, globalSecurity, openApiSpec);
            }
        }

        // Check for .NET specific authorization extensions
        ExtractDotNetAuthorizationExtensions(endpoint, operation);
    }

    /// <summary>
    /// Processes security requirements from OpenAPI spec
    /// </summary>
    private void ProcessSecurityRequirements(OpenApiEndpoint endpoint, JToken security, JObject openApiSpec)
    {
        if (security.Type == JTokenType.Array)
        {
            foreach (var securityRequirement in security)
            {
                if (securityRequirement.Type == JTokenType.Object)
                {
                    var securityObj = securityRequirement as JObject;
                    if (securityObj != null && securityObj.Count > 0)
                    {
                        endpoint.RequiresAuthorization = true;

                        foreach (var scheme in securityObj)
                        {
                            endpoint.SecuritySchemes.Add(scheme.Key);

                            // Extract scopes/policies if they exist

                            if (scheme.Value!.Type == JTokenType.Array)
                            {
                                foreach (var scope in scheme.Value)
                                {
                                    var scopeValue = scope.ToString();

                                    if (!string.IsNullOrEmpty(scopeValue))
                                    {
                                        endpoint.AuthorizationPolicies.Add(scopeValue);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Extracts .NET specific authorization extensions from OpenAPI spec
    /// </summary>
    private void ExtractDotNetAuthorizationExtensions(OpenApiEndpoint endpoint, JObject operation)
    {
        // Check for authorization policies extension (added by our custom filter)
        var authPoliciesExtension = operation["x-authorization-policies"];
        if (authPoliciesExtension != null)
        {
            endpoint.RequiresAuthorization = true;

            if (authPoliciesExtension.Type == JTokenType.Array)
            {
                foreach (var policy in authPoliciesExtension)
                {
                    var policyName = policy.ToString();
                    if (!string.IsNullOrEmpty(policyName) && !endpoint.AuthorizationPolicies.Contains(policyName))
                    {
                        endpoint.AuthorizationPolicies.Add(policyName);
                    }
                }
            }
        }

        // Check for other common authorization extensions
        var extensions = new[]
        {
            "x-ms-authorization",
            "x-authorize",
            "x-authorization-policy",
            "x-policy",
            "x-require-authorization",
            "x-hybrid-authorize",
            "x-custom-authorize",
            "x-security-policies"
        };

        foreach (var ext in extensions)
        {
            var authExtension = operation[ext];
            if (authExtension != null)
            {
                endpoint.RequiresAuthorization = true;

                if (authExtension.Type == JTokenType.String)
                {
                    var policyName = authExtension.ToString();
                    if (!string.IsNullOrEmpty(policyName) && !endpoint.AuthorizationPolicies.Contains(policyName))
                    {
                        endpoint.AuthorizationPolicies.Add(policyName);
                    }
                }
                else if (authExtension.Type == JTokenType.Array)
                {
                    foreach (var policy in authExtension)
                    {
                        var policyName = policy.ToString();
                        if (!string.IsNullOrEmpty(policyName) && !endpoint.AuthorizationPolicies.Contains(policyName))
                        {
                            endpoint.AuthorizationPolicies.Add(policyName);
                        }
                    }
                }
                else if (authExtension.Type == JTokenType.Object)
                {
                    var authObj = authExtension as JObject;
                    var policy = authObj?["policy"]?.ToString();
                    if (!string.IsNullOrEmpty(policy) && !endpoint.AuthorizationPolicies.Contains(policy))
                    {
                        endpoint.AuthorizationPolicies.Add(policy);
                    }

                    // Also check for "policies" array
                    var policies = authObj?["policies"];
                    if (policies != null && policies.Type == JTokenType.Array)
                    {
                        foreach (var p in policies)
                        {
                            var policyName = p.ToString();
                            if (!string.IsNullOrEmpty(policyName) && !endpoint.AuthorizationPolicies.Contains(policyName))
                            {
                                endpoint.AuthorizationPolicies.Add(policyName);
                            }
                        }
                    }
                }
            }
        }

        // Check for standard responses that indicate authorization (401, 403)
        var responses = operation["responses"];
        if (responses != null)
        {
            if (responses["401"] != null || responses["403"] != null)
            {
                endpoint.RequiresAuthorization = true;
            }
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
