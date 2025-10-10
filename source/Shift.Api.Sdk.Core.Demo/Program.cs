using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Refit;

using Shift.Api.Sdk.Core;
using Shift.Api.Sdk.Core.Demo;
using Shift.Contract.Requests;
;

// Load API configuration settings from AppSettings. Remember to add your own settings to this JSON file!

var configuration = new ConfigurationBuilder().AddJsonFile($"AppSettings.json").Build();
ApiSettings config = new ApiSettings();
configuration.Bind("ApiSettings", config);

// Create a service provider for the API.

var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<ApiTokenProvider>()
    .AddRefitClient<IApiMethods>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async () => await s.GetRequiredService<ApiTokenProvider>().GetTokenAsync(config.DeveloperSecret, config.EndpointUrl)
    })
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(config.EndpointUrl));

var provider = services.BuildServiceProvider();

var api = provider.GetRequiredService<IApiMethods>();

// Send a sample request to the API and display the response.

var request = new ComplianceSummaryRequest
{
    Departments = new[] { Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
    Learners = new[] { Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
    Measurements = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
    ProfileCondition = 2,
    LearnerMustHaveProfile = true
};

var response = await api.ComplianceSummaries(request, null);

foreach (var item in response)
    Console.WriteLine(JsonSerializer.Serialize(item));

// ... and then exit.

Console.WriteLine("Done. Press any key to exit.");
Console.ReadKey();