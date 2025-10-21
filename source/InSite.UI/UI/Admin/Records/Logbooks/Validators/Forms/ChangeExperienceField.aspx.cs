using System;

using InSite.Admin.Records.Logbooks;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Portal.Records.Logbooks.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Validators.Forms
{
    public partial class ChangeExperienceField : AdminBasePage, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var value) ? value : Guid.Empty;
        private JournalSetupFieldType? FieldType => Request["field"].ToEnumNullable<JournalSetupFieldType>();

        public Guid JournalIdentifier
        {
            get => (Guid)ViewState[nameof(JournalIdentifier)];
            set => ViewState[nameof(JournalIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!FieldType.HasValue)
                RedirectToSearch();

            var experience = ServiceLocator.JournalSearch.GetExperience(
                ExperienceIdentifier,
                x => x.Journal.JournalSetup, x => x.Journal.JournalSetup.Event);

            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || !ServiceLocator.JournalSearch.ExistsJournalSetupUser(experience.Journal.JournalSetupIdentifier, User.UserIdentifier, JournalSetupUserRole.Validator)
                )
            {
                RedirectToSearch();
            }

            var journalSetupField = ServiceLocator.JournalSearch
                .GetJournalSetupField(experience.Journal.JournalSetupIdentifier, FieldType.GetName());

            if (journalSetupField == null)
                RedirectToSearch();

            LoadData(experience, journalSetupField);

            var header = LogbookHeaderHelper.GetLogbookHeader(experience.Journal.JournalSetup, User.TimeZone);
            PageHelper.AutoBindHeader(this, null, header);

            CancelButton.NavigateUrl = GetUrlToValidate();
        }

        private void LoadData(QExperience experience, QJournalSetupField journalSetupField)
        {
            JournalIdentifier = experience.Journal.JournalIdentifier;

            var content = ServiceLocator.ContentSearch
                .GetBlock(journalSetupField.JournalSetupFieldIdentifier, ContentContainer.DefaultLanguage);
            var fieldDescription = ExperienceFieldDescription.Items[FieldType.Value];

            var field = (IExperienceField)Field
                .LoadControl($"~/UI/Admin/Records/Logbooks/Controls/ExperienceFields/{fieldDescription.ControlName}.ascx");

            field.Title = content[JournalSetupField.ContentLabels.LabelText].Text.Default
                .IfNullOrEmpty(journalSetupField.FieldType);
            field.Help = content[JournalSetupField.ContentLabels.HelpText].Text.Default;
            field.IsRequired = journalSetupField.FieldIsRequired;
            field.ValidationGroup = "Journal";

            fieldDescription.Load(field, experience);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var field = (IExperienceField)Field.GetControl();
            var fieldDescription = ExperienceFieldDescription.Items[FieldType.Value];

            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier, x => x.Journal.JournalSetup);
            var changeAction = fieldDescription.Save(field, experience);
            var command = changeAction(experience);

            ServiceLocator.SendCommand(command);

            RedirectToValidate();
        }

        private void RedirectToSearch() => HttpResponseHelper.Redirect("/ui/admin/records/logbooks/validators/search", true);

        private void RedirectToValidate() => HttpResponseHelper.Redirect(GetUrlToValidate(), true);

        private string GetUrlToValidate() => $"/ui/admin/records/logbooks/validators/validate-experience?experience={ExperienceIdentifier}";

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/validate-experience"))
                return $"experience={ExperienceIdentifier}";

            if (parent.Name.EndsWith("/outline-journal"))
                return $"journal={JournalIdentifier}";

            return null;
        }
    }
}
