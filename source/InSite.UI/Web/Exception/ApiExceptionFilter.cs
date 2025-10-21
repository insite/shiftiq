using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.Filters;

using InSite.Application.Files.Read;

using Shift.Common;

namespace InSite.Web
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException responseException
                && responseException.Response.StatusCode == HttpStatusCode.UnsupportedMediaType
                )
            {
                context.Response = ServerErrorActionResult.CreateResponse($"Unsupported Media Type", HttpStatusCode.UnsupportedMediaType);
                return;
            }

            var exception = context.Exception.InnerException == null
                    || context.Exception is ReadFileStreamFailedException
                    || context.Exception is ApplicationError
                ? context.Exception
                : context.Exception.InnerException;

            if (exception is COMException com
                && (
                    com.ErrorCode == -2147023901 // 0x800703E3
                    || com.ErrorCode == -2147024895 // 0x80070001
                    || com.ErrorCode == -2147024832 // 0x80070040
                )
                && (
                    context.Request.RequestUri.LocalPath.Equals("/api/assets/files", StringComparison.OrdinalIgnoreCase)
                    || context.Request.RequestUri.LocalPath.Equals("/api/assets/files/legacyupload", StringComparison.OrdinalIgnoreCase)
                ))
            {
                context.Response = ServerErrorActionResult.CreateResponse("Unexpected error", HttpStatusCode.ServiceUnavailable);
                return;
            }

            // Report exception to Sentry.
            AppSentry.SentryError(exception);

            // Create a valid HTTP 500 response.
            context.Response = ServerErrorActionResult.CreateResponse(exception.Message, HttpStatusCode.InternalServerError);
        }
    }
}