using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.JournalSetups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Logbooks
{
    public partial class ReorderFields : AdminBasePage, IHasParentLinkParameters
    {
        private Guid JournalSetupId => Guid.TryParse(Page.Request.QueryString["journalsetup"], out var id) ? id : Guid.Empty;

        private Guid[] Fields
        {
            get => (Guid[])ViewState[nameof(Fields)];
            set => ViewState[nameof(Fields)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
                Open();

            base.OnLoad(e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var args = Request.Form["__EVENTARGUMENT"];
            if (string.IsNullOrEmpty(args))
                return;

            var isSave = args.StartsWith("save&");
            if (isSave)
            {
                var startIndex = args.IndexOf('&') + 1;

                Save(args.Substring(startIndex));
            }

            HttpResponseHelper.Redirect($"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupId}&panel=fields");
        }

        private void Open()
        {
            var journalSetup = ServiceLocator.JournalSearch.GetJournalSetup(JournalSetupId, x => x.Fields);
            if (journalSetup == null
                || journalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || !CurrentSessionState.Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write)
                || journalSetup.Fields.Count < 2
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/logbooks/search", true);
            }

            PageHelper.AutoBindHeader(this);

            var orderedFields = journalSetup.Fields.OrderBy(x => x.Sequence).ToList();

            Fields = orderedFields.Select(x => x.JournalSetupFieldIdentifier).ToArray();

            var dataSource = orderedFields
                .Select(x =>
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(x.JournalSetupFieldIdentifier, ContentContainer.DefaultLanguage);
                    var labelText = content?[JournalSetupField.ContentLabels.LabelText].Text.Default;
                    var helpText = content?[JournalSetupField.ContentLabels.HelpText].Text.Default;

                    var fieldType = x.FieldType.ToEnum<JournalSetupFieldType>();

                    return new
                    {
                        Identifier = x.JournalSetupFieldIdentifier,
                        FieldType = fieldType.GetDescription(),
                        IsRequired = x.FieldIsRequired,
                        LabelText = labelText,
                        HelpText = helpText
                    };
                })
                .ToList();

            FieldRepeater.DataSource = dataSource;
            FieldRepeater.DataBind();

            CancelButton.NavigateUrl = $"/ui/admin/records/logbooks/outline?journalsetup={JournalSetupId}&panel=fields";
        }

        private void Save(string args)
        {
            var sequences = new List<(Guid, int)>();
            var oldSequences = args.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < oldSequences.Length; i++)
            {
                var oldSequence = int.Parse(oldSequences[i]);
                var fieldId = Fields[oldSequence - 1];

                sequences.Add((fieldId, i + 1));
            }

            ServiceLocator.SendCommand(new ReorderJournalSetupFields(JournalSetupId, sequences.ToArray()));
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupId}&panel=fields"
                : null;
        }
    }
}