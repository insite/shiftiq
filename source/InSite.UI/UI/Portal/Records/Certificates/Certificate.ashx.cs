using System;
using System.Web;
using System.Web.SessionState;

using InSite.Common.Web;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Records.Certificates
{
    public class CertificateModel
    {
        public CertificateRequest Request { get; set; }
        public CertificateVariables Variables { get; set; }
    }

    public partial class Certificate : IHttpHandler, IRequiresSessionState
    {
        private HttpResponse Response;

        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            Response = context.Response;

            try
            {
                var model = CreateCertificateModel();

                if (model.Request.FileType == "pdf")
                    GeneratePdf(model.Request, model.Variables);
                else
                    GeneratePng(model.Request, model.Variables);
            }
            catch (Exception ex)
            {
                var action = Global.HandleException(ex);
                if (action.Type != ExceptionActionType.Ignore)
                {
                    if (action.Type == ExceptionActionType.Warning)
                        AppSentry.SentryWarning(ex);
                    else
                        AppSentry.SentryError(ex);
                }
            }
        }

        private CertificateModel CreateCertificateModel()
        {
            var json = CurrentSessionState.AchievementCertificateRequest;

            if (string.IsNullOrEmpty(json))
            {
                HttpResponseHelper.Redirect(ServiceLocator.Urls.RootUrl);
            }

            var request = JsonConvert.DeserializeObject<CertificateRequest>(json);

            var variables = new CertificateVariables(request.Variables);

            return new CertificateModel
            {
                Request = request,
                Variables = variables
            };
        }

        protected void GeneratePdf(CertificateRequest request, CertificateVariables variables)
        {
            var bytes = CertificateBuilder.CreatePdf(request.ImagePath, request.Elements, variables);

            WriteResponse(bytes, "application/pdf", "Certificate.pdf");
        }

        protected void GeneratePng(CertificateRequest request, CertificateVariables variables)
        {
            byte[] bytes = CertificateBuilder.CreateImage(request.ImagePath, request.Elements, variables, 1200);

            WriteResponse(bytes, "image/png", request.FileName != null ? request.FileName : "Certificate.png");
        }

        public static byte[] CreateImage(string json)
        {
            var request = JsonConvert.DeserializeObject<CertificateRequest>(json);

            var variables = new CertificateVariables(request.Variables);

            return CertificateBuilder.CreateImage(request.ImagePath, request.Elements, variables, 1200);
        }

        private void WriteResponse(byte[] bytes, string contentType, string filename)
        {
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
            Response.AddHeader("Content-Length", bytes.Length.ToString());
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
        }
    }
}