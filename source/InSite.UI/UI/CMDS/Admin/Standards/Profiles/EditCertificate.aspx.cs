using System;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class EditCertificate : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/cmds/admin/standards/profiles/search";
        private const string EditUrl = "/ui/cmds/admin/standards/profiles/edit";

        private Guid StandardIdentifier => Guid.TryParse(Request.QueryString["id"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UniqueNumber.ServerValidate += UniqueNumber_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            Open();

            CancelButton.NavigateUrl = EditUrl + $"?id={StandardIdentifier}";
        }

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            var isNotLocked = StandardSearch.Select(StandardIdentifier)?.IsLocked == false;

            SaveButton.Visible = Access.Write && isNotLocked;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/edit"))
                return "id=" + StandardIdentifier;

            return null;
        }

        private void UniqueNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var list = StandardSearch.Select(x => x.StandardType == "Profile" && x.Code == Number.Text);

            args.IsValid = list.Count == 0 || list[0].StandardIdentifier == StandardIdentifier;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!Save())
                return;

            Open();

            SetStatus(ScreenStatus, StatusType.Saved);
        }

        private void Open()
        {
            var entity = StandardSearch.Select(StandardIdentifier);
            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            if (!string.Equals(entity.StandardType, "Profile", StringComparison.OrdinalIgnoreCase))
                HttpResponseHelper.Redirect(SearchUrl);

            SetInputValues(entity);

            var count = CompetencyList.LoadData(entity.StandardIdentifier, entity.IsLocked);

            CompetencyTab.SetTitle("Competencies", count);
        }

        private bool Save()
        {
            var info = ServiceLocator.StandardSearch.GetStandard(StandardIdentifier);

            GetInputValues(info);

            StandardStore.Update(info);

            CompetencyList.SaveHours(info.StandardIdentifier);

            return true;
        }

        private void SetInputValues(Standard info)
        {
            Number.Text = info.Code;
            TitleInput.Text = info.ContentTitle;
            CertificationIsAvailable.Checked = info.IsCertificateEnabled;
            CertificationHoursPercentCore.ValueAsDecimal = info.CertificationHoursPercentCore;
            CertificationHoursPercentNonCore.ValueAsDecimal = info.CertificationHoursPercentNonCore;
        }

        private void GetInputValues(QStandard info)
        {
            info.Code = Number.Text;
            info.ContentTitle = TitleInput.Text;
            info.IsCertificateEnabled = CertificationIsAvailable.Checked;
            info.CertificationHoursPercentCore = CertificationHoursPercentCore.ValueAsDecimal;
            info.CertificationHoursPercentNonCore = CertificationHoursPercentNonCore.ValueAsDecimal;
        }
    }
}