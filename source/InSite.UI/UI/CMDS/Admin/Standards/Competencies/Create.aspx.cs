using System;
using System.Web.UI.WebControls;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.Competencies.Forms
{
    public partial class Create : AdminBasePage, ICmdsUserControl
    {
        private const string SearchUrl = "/ui/cmds/admin/standards/competencies/search";
        private const string EditUrl = "/ui/cmds/admin/standards/competencies/edit";

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            if (Access.Read && !Access.Write)
                Access = Access.SetAll(false);
        }

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

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void UniqueNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var info = CompetencyRepository.Select(Number.Text);

            args.IsValid = info == null;

            if (args.IsValid)
                return;

            var message = info.IsDeleted
                ? "Competency #{0} is assigned to a competency that is now deleted. If you re-number the deleted competency then you can re-use this number."
                : "Competency #{0} is already in use by another competency.";

            UniqueNumber.ErrorMessage = string.Format(message, Number.Text);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var info = StandardFactory.Create(StandardType.Competency);
            info.StandardIdentifier = UniqueIdentifier.Create();

            GetInputValues(info);

            info.AssetNumber = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            info.OrganizationIdentifier = OrganizationIdentifiers.CMDS;

            StandardStore.Insert(info);

            StandardClassificationStore.ReplaceCategory(info.StandardIdentifier, CategoryIdentifier.ValueAsGuid);

            var author = $"{User.FullName} ({User.Email})";
            var change = $"Competency **{info.Code}** created by **{author}**";
            var changed = new CmdsCompetencyChanged(author, change);

            ServiceLocator.ChangeQueue.Publish(changed);

            HttpResponseHelper.Redirect($"{EditUrl}?id={info.StandardIdentifier}&status=saved");
        }

        private void GetInputValues(QStandard info)
        {
            info.Code = Number.Text;
            info.SourceDescriptor = NumberOld.Text;
            info.CreditHours = ProgramHours.ValueAsDecimal;
            info.ContentTitle = Summary.Text;
        }
    }
}