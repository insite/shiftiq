using System;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Events;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class Describe : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private const string SearchUrl = "/ui/admin/events/classes/search";

        private const string InstructionsPillId = "Instructions";

        private Guid? EventIdentifier =>
            Guid.TryParse(Request["event"], out var value) ? value : (Guid?)null;

        private string Tab => Request["tab"];

        private string SubTab => Request["subtab"];

        private string OutlineUrl =>
            $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=content&tab={GetTabTitle(ContentEditor.GetCurrentTab())}&subtab={ContentEditor.GetCurrentSubTab()}";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();
            CancelButton.Click += (s, a) => HttpResponseHelper.Redirect(OutlineUrl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value) : null;
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            SetInputValues(@event);
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var title = ContentEditor.GetValue(ContentSectionDefault.Title);
            var summary = ContentEditor.GetValue(ContentSectionDefault.Summary);
            var description = ContentEditor.GetValue(ContentSectionDefault.Description);
            var materials = ContentEditor.GetValue(ContentSectionDefault.MaterialsForParticipation);

            var instructions = new[]
            {
                new EventInstruction
                {
                    Type = EventInstructionType.Contact,
                    Text = ContentEditor.GetValue(InstructionsPillId, EventInstructionType.Contact)
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Accommodation,
                    Text = ContentEditor.GetValue(InstructionsPillId, EventInstructionType.Accommodation)
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Additional,
                    Text = ContentEditor.GetValue(InstructionsPillId, EventInstructionType.Additional)
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Cancellation,
                    Text = ContentEditor.GetValue(InstructionsPillId, EventInstructionType.Cancellation)
                },
                new EventInstruction
                {
                    Type = EventInstructionType.Completion,
                    Text = ContentEditor.GetValue(InstructionsPillId, EventInstructionType.Completion)
                }
            };

            var classLink = ContentEditor.GetValue(ContentSectionDefault.ClassLink);

            ServiceLocator.SendCommand(new DescribeEvent(EventIdentifier.Value, title, summary, description, materials, instructions, classLink));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(QEvent @event)
        {
            var content = ContentEventClass.Deserialize(@event.Content);
            var uploadFolderPath = $"/events/{@event.EventNumber}";

            if (content.Title.Default.IsEmpty())
                content.Title.Default = @event.EventTitle;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            if (ContentEditor.IsEmpty)
            {
                ContentEditor.Add(ContentSectionDefault.Title, content);

                {
                    var summaryPill = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Summary, content);
                    summaryPill.AllowUpload = true;
                    summaryPill.UploadFolderPath = uploadFolderPath;
                    ContentEditor.Add(summaryPill);
                }

                {
                    var descriptionPill = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.Description, content);
                    descriptionPill.AllowUpload = true;
                    descriptionPill.UploadFolderPath = uploadFolderPath;
                    ContentEditor.Add(descriptionPill);
                }

                {
                    var materialsForParticipationPill = (AssetContentSection.Markdown)AssetContentSection.Create(ContentSectionDefault.MaterialsForParticipation, content);
                    materialsForParticipationPill.AllowUpload = true;
                    materialsForParticipationPill.UploadFolderPath = uploadFolderPath;
                    ContentEditor.Add(materialsForParticipationPill);
                }

                {
                    ContentEditor.Add(new AssetContentSection.TabList(InstructionsPillId)
                    {
                        Title = "Instructions",
                        Tabs = new[]
                        {
                            CreateInstructionPill(EventInstructionType.Contact, "Contact and Support"),
                            CreateInstructionPill(EventInstructionType.Accommodation, "Accommodation"),
                            CreateInstructionPill(EventInstructionType.Additional, "Additional"),
                            CreateInstructionPill(EventInstructionType.Cancellation, "Cancellation and Refund"),
                            CreateInstructionPill(EventInstructionType.Completion, "Registration Completion")
                        }
                    });

                    AssetContentSection.Markdown CreateInstructionPill(EventInstructionType id, string title)
                    {
                        var idName = id.GetName();

                        return new AssetContentSection.Markdown(idName)
                        {
                            Title = title,
                            AllowUpload = true,
                            UploadFolderPath = uploadFolderPath,
                            Value = content.Get(idName) ?? new MultilingualString()
                        };
                    }
                }

                {
                    var classLinkPill = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.ClassLink, content);
                    ContentEditor.Add(classLinkPill);
                }

                ContentEditor.OpenTab(GetTabID(Tab), SubTab);
            }
        }

        #endregion

        #region Helper methods

        private string GetTabID(string tabTitle)
        {
            if (tabTitle == "Materials")
                return ContentSectionDefault.MaterialsForParticipation.ToString();

            return tabTitle;
        }

        private string GetTabTitle(string id)
        {
            if (id == ContentSectionDefault.MaterialsForParticipation.ToString())
                return "Materials";

            return id;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"event={EventIdentifier}"
                : null;
        }

        #endregion
    }
}