using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api
{
    public class ShiftControllerBase : ControllerBase
    {
        protected ActionResult SerializedContent(string json)
        {
            return Content(json, "application/json");
        }

        protected ActionResult BadRequest(string? detail = null)
        {
            return ProblemFactory.BadRequest(detail)
                .ToActionResult(this);
        }

        protected ActionResult NotFound(string? detail = null)
        {
            return ProblemFactory.NotFound(detail)
                .ToActionResult(this);
        }

        protected ActionResult ServerError(string detail, Uri instance)
        {
            return ProblemFactory.InternalServerError(detail, instance)
                .ToActionResult(this);
        }

        protected ActionResult Unauthorized(string? detail = null)
        {
            return ProblemFactory.Unauthorized(detail)
                .ToActionResult(this);
        }
    }
}
