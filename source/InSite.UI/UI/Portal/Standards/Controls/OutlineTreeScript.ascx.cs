using System;
using System.Web.UI;

using InSite.Common.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Standards.Controls
{
    public partial class OutlineTreeScript : BaseUserControl
    {
        public string TermsData
        {
            get => (string)ViewState[nameof(TermsData)];
            set => ViewState[nameof(TermsData)] = value;
        }

        private bool _updateTerms = false;

        public void UpdateTerms()
        {
            _updateTerms = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_updateTerms)
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(OutlineTreeScript),
                    "update_terms",
                    $"termWindow.updateTerms({TermsData.IfNullOrEmpty("null")});",
                    true);

            base.OnPreRender(e);
        }
    }
}