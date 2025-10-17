using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Engine.Common
{
    public static class ProblemExtensions
    {
        public static IActionResult ToActionResult(this Problem problem, ControllerBase controller)
        {
            return controller.StatusCode(problem.Status ?? 500, problem);
        }
    }
}
