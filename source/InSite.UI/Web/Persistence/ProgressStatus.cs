using System;
using System.Net;
using System.Web;

using Shift.Common;

using ProgressContext = Shift.Sdk.UI.ProgressPanelContext;

namespace InSite.Web.Persistence
{
    public class ProgressStatus : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var response = context.Response;

            var contextId = context.Request["context"];
            var contextData = contextId.IsNotEmpty() ? GetContext(contextId) : null;

            if (contextData != null && !contextData.IsEmpty)
            {
                contextData.IsCancelled = string.Equals(context.Request["cancelled"], "true", StringComparison.OrdinalIgnoreCase);

                response.ContentType = "application/json";
                contextData.ToJson(response.Output);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
            }
        }

        internal static ProgressContext GetContext(string contextId)
        {
            var key = GetContextKey(contextId);
            var context = (ProgressContext)HttpContext.Current.Application[key];

            if (context != null)
                HttpContext.Current.Application.Remove(key);

            return context;
        }

        internal static void SetContext(string contextId, ProgressContext context) =>
            SetContext(HttpContext.Current.Application, contextId, context);

        internal static void SetContext(HttpApplicationState app, string contextId, ProgressContext context)
        {
            var key = GetContextKey(contextId);

            app[key] = context;
        }

        internal static void RemoveContext(string contextId) =>
            RemoveContext(HttpContext.Current.Application, contextId);

        internal static void RemoveContext(HttpApplicationState app, string contextId)
        {
            var key = GetContextKey(contextId);
            app.Remove(key);
        }

        private static string GetContextKey(string contextId)
        {
            return nameof(ProgressStatus) + "_" + contextId;
        }
    }
}