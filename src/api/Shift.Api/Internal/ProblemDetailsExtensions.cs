using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Api
{
    public static class ProblemExtensions
    {
        public static IActionResult ToActionResult(this Problem problem, ControllerBase controller)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return new JsonResult(problem, jsonSettings)
            {
                StatusCode = problem.Status ?? 500
            };
        }
    }
}
