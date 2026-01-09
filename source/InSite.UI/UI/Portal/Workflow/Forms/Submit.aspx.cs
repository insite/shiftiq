using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Common.Web;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Workflow.Forms.Controls;

using Shift.Common;

namespace InSite.UI.Portal.Workflow.Forms
{
    public partial class SubmitPage : PortalBasePage
    {
        public override string ActionUrl => $"ui/portal/workflow/forms/submit/{_verb}";

        private string _verb;
        private SubmissionSessionControl _verbControl;

        private static readonly HashSet<string> _validVerbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "answer",
            "complete",
            "confirm",
            "delete",
            "goto",
            "launch",
            "resume",
            "review",
            "review-print",
        };

        protected override void OnPreInit(EventArgs e)
        {
            IsAuthenticationRequired = false;

            CanonizeUrl();

            base.OnPreInit(e);

            if (!LoadVerbControl())
                HttpResponseHelper.SendHttp404();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (_verbControl != null)
                VerbPlaceholder.Controls.Add(_verbControl);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsPostBack)
                SideContent.BindControlsToModel(_verbControl.Current.Survey);
        }

        private bool LoadVerbControl()
        {
            string controlName;

            if (StringHelper.Equals(_verb, "review-print"))
            {
                controlName = "ReviewPrint.ascx";
                Page.MasterPageFile = "~/UI/Layout/Portal/PortalPrint.master";
            }
            else
            {
                controlName = _verb + ".ascx";
            }

            _verbControl = (SubmissionSessionControl)LoadControl($"./Controls/{controlName}");

            return _verbControl != null;
        }

        private void CanonizeUrl()
        {
            _verb = RouteData.Values["verb"] as string;
            if (_validVerbs.Contains(_verb))
                return;

            var form = RouteData.Values["form"] as string;
            var user = RouteData.Values["user"] as string;

            var url = new WebUrl($"/ui/portal/workflow/forms/submit/launch/{form}/{user}");

            var returnValue = Request.QueryString["return"];
            if (returnValue.IsNotEmpty())
                url.QueryString["return"] = returnValue;

            HttpResponseHelper.Redirect(url);
        }
    }
}