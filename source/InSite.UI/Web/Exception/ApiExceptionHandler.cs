using System.Net;
using System.Web.Http.ExceptionHandling;

namespace InSite.Web
{
    public class ApiExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var exception = context.Exception.InnerException ?? context.Exception;

            // Report exception to Sentry.
            AppSentry.SentryError(exception);

            // Create a valid HTTP 500 response.
            context.Result = new ServerErrorActionResult(exception.Message, context.Request, HttpStatusCode.InternalServerError);

            // Allow the base class to handle the rest.
            base.Handle(context);
        }
    }
}