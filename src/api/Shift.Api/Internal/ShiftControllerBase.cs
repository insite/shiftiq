using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api
{
    public class ShiftControllerBase : ControllerBase
    {
        protected IActionResult SerializedContent(string json)
        {
            return Content(json, "application/json");
        }

        protected IActionResult BadRequest(string? detail = null)
        {
            return ProblemFactory.BadRequest(detail)
                .ToActionResult(this);
        }

        protected IActionResult NotFound(string? detail = null)
        {
            return ProblemFactory.NotFound(detail)
                .ToActionResult(this);
        }

        protected IActionResult ServerError(string detail, Uri instance)
        {
            return ProblemFactory.InternalServerError(detail, instance)
                .ToActionResult(this);
        }

        protected IActionResult Unauthorized(string? detail = null)
        {
            return ProblemFactory.Unauthorized(detail)
                .ToActionResult(this);
        }
    }
}
