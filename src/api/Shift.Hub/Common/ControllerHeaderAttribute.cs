using Microsoft.AspNetCore.Mvc.Filters;

namespace Shift.Hub
{
    public class ControllerHeaderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerType = context.Controller.GetType();
            var fqcn = controllerType.FullName;

            context.HttpContext.Response.Headers["X-Controller-Class"] = fqcn ?? "Unknown";
            base.OnActionExecuted(context);
        }
    }
}
