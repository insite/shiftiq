using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Logs.Aggregates;
using InSite.Application.Attempts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Domain.Events;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class Detail : BaseUserControl
    {
        #region Properties

        public Guid? EventIdentifier
        {
            get => (Guid?)ViewState[nameof(EventIdentifier)];
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        protected bool CanWrite { get; set; }

        public string EventExamType
        {
            get => (string)ViewState[nameof(EventExamType)];
            private set => ViewState[nameof(EventExamType)] = value;
        }

        public string EventSchedulingStatus
        {
            get => (string)ViewState[nameof(EventSchedulingStatus)];
            private set => ViewState[nameof(EventSchedulingStatus)] = value;
        }

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddFormButton.Click += AddExamFormButton_Click;
            FormRepeater.DataBinding += FormRepeater_DataBinding;
            FormRepeater.ItemDataBound += FormRepeater_ItemDataBound;
            FormRepeater.ItemCommand += FormRepeater_ItemCommand;
            FormPublicationRestart.Click += PublicationRestart_Clicked;

            WithholdGrades.AutoPostBack = true;
            WithholdGrades.CheckedChanged += WithholdGrades_CheckedChanged;

            WithholdDistribution.AutoPostBack = true;
            WithholdDistribution.CheckedChanged += WithholdDistribution_CheckedChanged;

            MarkGrid.Published += MarkGrid_Published;
        }

        #endregion

        #region Events

        public event EventHandler ExamFormsChanged;
        private void OnExamFormsChanged()
        {
            ExamFormsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event ExamMarkGrid.PublishedEventHandler Published;
        private void OnPublished(ExamMarkGrid.PublishedEventArgs e)
        {
            Published?.Invoke(MarkGrid, e);
        }

        #endregion

        #region Methods (event handling)

        private void PublicationRestart_Clicked(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new StartEventPublication(EventIdentifier.Value));
            BindPublicationPanel("Started", null);
        }

        private void WithholdGrades_CheckedChanged(object sender, EventArgs e) => OnIntegrationInputsChanged();

        private void WithholdDistribution_CheckedChanged(object sender, EventArgs e) => OnIntegrationInputsChanged();

        private void OnIntegrationInputsChanged()
        {
            ServiceLocator.SendCommand(new ConfigureIntegration(
                EventIdentifier.Value,
                WithholdGrades.Checked,
                WithholdDistribution.Checked));
        }

        private void MarkGrid_Published(object sender, ExamMarkGrid.PublishedEventArgs e)
        {
            OnPublished(e);
        }

        private void VenueSelected(Guid? office, Guid? location)
        {
            var mailingAddress = office.HasValue
                ? ServiceLocator.GroupSearch.GetAddress(office.Value, AddressType.Shipping)
                : null;

            var physicalAddress = location.HasValue
                ? ServiceLocator.GroupSearch.GetAddress(location.Value, AddressType.Physical)
                : null;

            PhysicalAddress.BindAddress(physicalAddress, "Physical Location", "This is the physical address of the location where the event occurs.");
            ShippingAddress.BindAddress(mailingAddress, "Shipping Address", "This is the address to which exam materials must be sent.");
        }

        #endregion

        #region Methods (data binding)

        public void ShowTab(string tab)
        {
            if (tab == "grades")
                GradeTab.IsSelected = true;
        }

        public void BindPublicationPanel(string status, string errors)
        {
            FormPublicationPanel.Visible = true;

            if (!string.IsNullOrEmpty(errors))
            {
                // FormPublicationPanel.CssClass = "alert alert-danger";
                FormPublicationStatus.Text = "Not Published";
                FormPublicationStatus.CssClass = "badge bg-danger";
                FormPublicationStatus.ToolTip = errors;
            }
            else if (!string.IsNullOrEmpty(status))
            {
                if (string.Equals(status, PublicationStatus.Drafted.GetDescription(), StringComparison.OrdinalIgnoreCase))
                {
                    // FormPublicationPanel.CssClass = "alert alert-warning";
                    FormPublicationStatus.Text = "Publication Pending";
                    FormPublicationStatus.CssClass = "badge bg-warning";
                    FormPublicationStatus.ToolTip = "Waiting for Direct Access...";
                }
                else
                {
                    // FormPublicationPanel.CssClass = "alert alert-success";
                    FormPublicationStatus.Text = "Published";
                    FormPublicationStatus.CssClass = "badge bg-success";
                }
            }
            else
            {
                FormPublicationPanel.Visible = false;
            }
        }

        public void SetDefaultInputValues()
        {
            DeleteEventLink.Visible = false;
        }

        public void SetInputValues(EventState state, QEvent @event, bool canWrite, bool canDelete)
        {
            CanWrite = canWrite;

            FormPopupSelectorWindow.Filter.IsPublished = @event.ExamType != Shift.Common.EventExamType.Test.Value;

            EventIdentifier = @event.EventIdentifier;
            EventExamType = @event.ExamType;
            EventSchedulingStatus = @event.EventSchedulingStatus;

            StatusField.Visible = true;
            NumberField.Visible = true;
            CrmCaseNumberField.Visible = false;
            EventTitleField.Visible = true;
            LocationPanel.Visible = true;

            EventScheduledStart.Text = TimeZones.Format(@event.EventScheduledStart, User.TimeZone);
            EventScheduled2.Text = EventScheduledStart.Text;
            ChangeEventScheduledStart.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";

            if (@event.DurationQuantity.HasValue && @event.DurationQuantity.HasValue)
                ExamDuration.Text = $"{@event.DurationQuantity} {@event.DurationUnit.Pluralize()}";
            else
                ExamDuration.Text = "None";
            ChangeExamDuration.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";
            ChangeExamFormat.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";

            ChangeEventSchedulingStatus.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";
            RequestStatus.Text = @event.EventRequisitionStatus ?? "None";
            ChangeRequestStatus.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";

            CapacityMaximum.Text = @event.CapacityMaximum.HasValue ? @event.CapacityMaximum.ToString() : "None";
            CapacityMaximumEdit.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";

            InvigilatorMinimum.Text = @event.InvigilatorMinimum.HasValue ? @event.InvigilatorMinimum.ToString() : "None";
            InvigilatorMinimumEdit.NavigateUrl = $"/ui/admin/events/exams/reschedule?event={EventIdentifier}";

            EventTypeHelp.InnerHtml = ExamTypeComboBox.GetDescription(@event.ExamType);

            EventFormat.Text = @event.EventFormat;
            EventNumber.Text = @event.EventNumber.ToString();
            DeleteEventLink.NavigateUrl = $"/admin/events/exams/delete?event={EventIdentifier}";
            EventTitle.Text = @event.EventTitle;

            ClassCode.Text = @event.EventClassCode;
            ChangeClassCode.NavigateUrl = $"/ui/admin/events/exams/recode?event={EventIdentifier}";

            BillingCode.Text = GetBillingText(@event.EventBillingType);
            ChangeBillingCode.NavigateUrl = $"/ui/admin/events/exams/recode?event={EventIdentifier}";

            if (!@event.ExamMaterialReturnShipmentCode.HasValue() && !@event.ExamMaterialReturnShipmentReceived.HasValue)
                ExamMaterialReturnShipmentCode.Text = "None";
            if (@event.ExamMaterialReturnShipmentCode.HasValue())
                ExamMaterialReturnShipmentCode.Text = @event.ExamMaterialReturnShipmentCode;
            if (@event.ExamMaterialReturnShipmentReceived.HasValue)
            {
                ExamMaterialReturnShipmentReceived.Text = @event.ExamMaterialReturnShipmentReceived.Format(User.TimeZone, true);
                if (@event.ExamMaterialReturnShipmentCondition == "Full")
                    ExamMaterialReturnShipmentCondition.Text = $"<span class='badge bg-success'>Fully Received</span>";
                else if (@event.ExamMaterialReturnShipmentCondition == "Partial")
                    ExamMaterialReturnShipmentCondition.Text = $"<span class='badge bg-danger'>Partially Received</span>";
            }
            ChangeExamMaterialReturnShipment.NavigateUrl = $"/ui/admin/events/exams/return-material?event={EventIdentifier}";

            ViewHistoryLink.NavigateUrl = Outline.GetUrl(EventIdentifier.Value, $"/ui/admin/events/exams/outline?event={EventIdentifier}");

            BindPublicationPanel(@event.EventPublicationStatus, @event.PublicationErrors);

            if (@event.VenueOfficeIdentifier.HasValue && @event.VenueLocationIdentifier.HasValue)
            {
                VenueSelected(@event.VenueOfficeIdentifier, @event.VenueLocationIdentifier);
                VenueOfficeName.Text = $"<a href='/ui/admin/contacts/groups/edit?contact={@event.VenueOfficeIdentifier}'>{@event.VenueOfficeName}</a>";
                VenueLocationName.Text = $"<a href='/ui/admin/contacts/groups/edit?contact={@event.VenueLocationIdentifier}'>{@event.VenueLocationName}</a>";
            }
            else
            {
                VenueSelected(null, null);
                VenueOfficeName.Text = @event.VenueOfficeName;
                VenueLocationName.Text = @event.VenueLocationName;
            }

            ExamChangeVenue1.NavigateUrl = ExamChangeVenue2.NavigateUrl = ExamChangeVenue3.NavigateUrl = $"/ui/admin/events/exams/change-venue?event={EventIdentifier}";
            VenueRoom.Text = @event.VenueRoom ?? "None";

            var isClass = @event.ExamType == Shift.Common.EventExamType.Class.Value;
            ClassCodeField.Visible = isClass;
            VenueRoomField.Visible = isClass;

            // Integration

            IntegrationTab.Visible = true;
            WithholdGrades.Checked = state.Integration.WithholdGrades;
            WithholdDistribution.Checked = state.Integration.WithholdDistribution;

            // Distribution

            DistributionTab.Visible = true;
            ChangeDistributionLink.NavigateUrl = $"/ui/admin/events/exams/change-distribution?event={EventIdentifier}";

            AttemptStarted.Text = @event.ExamStarted.Format(null, true, nullValue: "None");
            ScheduleTimer.Text = @event.ExamMaterialReturnShipmentReceived == null
                ? @event.EventScheduledStart.Humanize()
                : (@event.ExamMaterialReturnShipmentReceived.Value - @event.EventScheduledStart).Humanize();

            if (@event.EventFormat != EventExamFormat.Online.Value)
            {
                DistributionExpected.Text = @event.DistributionExpected.Format(null, true, nullValue: "None");
                ExamMaterialReturnShipmentReceived2.Text = @event.ExamMaterialReturnShipmentReceived.Format(null, true, nullValue: "None");
                DistributionOrdered.Text = @event.DistributionOrdered.Format(null, true, nullValue: "None");
                DistributionShipped.Text = @event.DistributionShipped.Format(null, true, nullValue: "None");
                DistributionProcessOutput.Text = DistributionProcess.GetText(@event.DistributionProcess)
                    ?? DistributionProcess.Items[0].Text;
                DistributionCourier.Text = @event.ExamMaterialReturnShipmentCode ?? "None";
            }
            else
            {
                DistributionOrderedField.Visible = false;
                DistributionExpectedField.Visible = false;
                DistributionShippedField.Visible = false;
                ExamMaterialReturnShipmentReceivedField.Visible = false;
                DistributionCourierField.Visible = false;
                DistributionProcessField.Visible = false;
            }

            // Exam Forms

            FormRepeater.DataBind();
            FormTab.Visible = true;

            ViewHistoryLink.Visible = true;

            // Other

            var count = MarkGrid.LoadData(@event.EventIdentifier);
            GradeTab.SetTitle("Grades", count);

            ShowScoreWarning(@event);

            var panel = Request["panel"];
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(panel) && string.Equals(panel, "event.distribution", StringComparison.OrdinalIgnoreCase))
                    DistributionTab.IsSelected = true;
            }

            FormPublicationRestart.Visible = canWrite;
            DeleteEventLink.Visible = canDelete;
            ChangeEventScheduledStart.Visible = canWrite;
            ChangeExamDuration.Visible = canWrite;
            ChangeExamFormat.Visible = canWrite;
            ChangeEventSchedulingStatus.Visible = canWrite;
            ChangeRequestStatus.Visible = canWrite;
            CapacityMaximumEdit.Visible = canWrite;
            InvigilatorMinimumEdit.Visible = canWrite;
            ChangeClassCode.Visible = canWrite;
            ChangeBillingCode.Visible = canWrite;
            ChangeExamMaterialReturnShipment.Visible = canWrite;
            ExamChangeVenue1.Visible = ExamChangeVenue2.Visible = ExamChangeVenue3.Visible = canWrite;
            ChangeDistributionLink.Visible = canWrite;
            NewEventLink.Visible = canWrite;

            Separator1.Visible = canWrite;
            Separator2.Visible = canWrite;

            AddFormButton.Visible = canWrite;
        }

        private void ShowScoreWarning(QEvent @event)
        {
            if (!string.Equals(@event.ExamType, Shift.Common.EventExamType.Class.Value, StringComparison.OrdinalIgnoreCase))
                return;

            var filter = new QAttemptFilter { EventIdentifier = @event.EventIdentifier };
            var attempts = ServiceLocator.AttemptSearch
                .GetAttempts(filter)
                .GroupBy(x => x.FormIdentifier)
                .ToDictionary(x => x.Key, x => x.ToList());

            var forms = new List<Form>();

            foreach (var formId in attempts.Keys)
            {
                var form = ServiceLocator.BankSearch.GetFormData(formId);
                forms.Add(form);
            }

            forms.Sort((a, b) => a.Name.CompareTo(b.Name));

            var warnings = new List<string>();

            foreach (var form in forms)
            {
                var formAttempts = attempts[form.Identifier];
                var below70 = formAttempts.Count(x => !InstructorAttemptStore.AttemptIsPassing(x.AttemptScore, 0.7m));

                if (formAttempts.Count / 2 <= below70)
                {
                    var warning = "score".ToQuantity(below70) + " less than 70 %";
                    if (forms.Count > 1)
                        warning = $"<strong>{form.Name}</strong>: {warning}";

                    warnings.Add(warning);
                }
            }

            WarningRepeater.Visible = warnings.Count > 0;
            WarningRepeater.DataSource = warnings;
            WarningRepeater.DataBind();
        }

        private string GetBillingText(string code)
        {
            var billingCode = TCollectionItemCache.SelectFirst(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CollectionName = CollectionName.Activities_Exams_Billing_Type,
                ItemName = code
            });

            if (billingCode != null)
                return $"{billingCode.ItemName}: {billingCode.ItemDescription}";

            return "None";
        }

        public void CheckFull(int candidateCount, int actualInvigilatorCount)
        {
            FullLabel.Visible = candidateCount > 0
                         && !string.IsNullOrEmpty(CapacityMaximum.Text)
                         && int.TryParse(CapacityMaximum.Text, out var maximum)
                         && maximum <= candidateCount;

            int requiredInvigilatorMinimum = 0;
            if (!string.IsNullOrEmpty(InvigilatorMinimum.Text) && int.TryParse(InvigilatorMinimum.Text, out var n))
                requiredInvigilatorMinimum = n;
            InvigilatorError.Visible = actualInvigilatorCount < requiredInvigilatorMinimum;
            InvigilatorError.InnerText = $"{requiredInvigilatorMinimum - actualInvigilatorCount} Needed";

            var candidateToInvigilatorRatio = 24.0;
            var recommendedInvigilatorMinimum = (int)Math.Ceiling(candidateCount / candidateToInvigilatorRatio);
            InvigilatorWarning.Visible = requiredInvigilatorMinimum < recommendedInvigilatorMinimum;
            InvigilatorWarning.InnerText = $"{recommendedInvigilatorMinimum} Recommended";
        }

        #endregion

        #region Methods (Exam Forms)

        private void AddExamFormButton_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(AddFormIdentifier.Value, out Guid form))
                return;

            var formData = ServiceLocator.BankSearch.GetFormData(form);

            if (formData != null && !string.IsNullOrEmpty(formData.Code) && !string.IsNullOrEmpty(formData.Name))
            {
                ServiceLocator.SendCommand(new AddEventAssessment(EventIdentifier.Value, form));

                AddFormIdentifier.Value = null;

                FormRepeater.DataBind();

                OnExamFormsChanged();
            }
            else if (formData == null)
            {
                AddFormAlert.AddMessage(AlertType.Error, "Form is not found");
            }
            else
            {
                var error = new StringBuilder();

                if (string.IsNullOrEmpty(formData.Code))
                    error.Append("The assessment form you are attempting to add to this Exam event does not have a Code. Please edit the form to include a code prior to adding it to this event.");

                if (string.IsNullOrEmpty(formData.Name))
                {
                    if (error.Length > 0)
                        error.Append("<br/>");

                    error.Append("Form Name field is required.");
                }

                AddFormAlert.AddMessage(AlertType.Error, error.ToString());
            }
        }

        private void FormRepeater_DataBinding(object sender, EventArgs e)
        {
            var forms = ServiceLocator.EventSearch.GetEventAssessmentForms(EventIdentifier.Value);
            FormRepeater.DataSource = forms;
            FormRepeater.Visible = forms.Count > 0;
        }

        private void FormRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var formId = Guid.Parse(e.CommandArgument.ToString());
                var attempts = ServiceLocator.AttemptSearch.GetAttempts(new QAttemptFilter
                {
                    EventIdentifier = EventIdentifier,
                    FormIdentifier = formId
                });

                if (attempts.Count > 0)
                {
                    var message = $"This exam form cannot be detached from this event because it is selected for {"exam candidate".ToQuantity(attempts.Count)}.";

                    ScriptManager.RegisterStartupScript(Page, typeof(Detail), "DetachExamForm", $"alert('{message}');", true);
                }
                else
                {
                    ServiceLocator.SendCommand(new RemoveEventAssessment(EventIdentifier.Value, formId));

                    FormRepeater.DataBind();

                    OnExamFormsChanged();
                }
            }
        }

        private void FormRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
            {
                return;
            }

            var form = (QEventAssessmentForm)e.Item.DataItem;

            ShowFormMaterials(form, e.Item);
        }

        private void ShowFormMaterials(QEventAssessmentForm form, RepeaterItem item)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(form.BankIdentifier);
            var f = bank.FindForm(form.FormIdentifier);

            var materialsForDistribution = f.Content?.MaterialsForDistribution?.Default;
            var materialsForParticipation = f.Content?.MaterialsForParticipation?.Default;

            if (materialsForDistribution.HasValue() || materialsForParticipation.HasValue())
            {
                var materials = item.FindControl("FormMaterials");
                materials.Visible = true;

                var html = new StringBuilder();
                if (materialsForDistribution.HasValue())
                {
                    html.Append("<div class='form-group mb-3'>");
                    html.Append("<label class='form-label'>Materials for Distribution</label>");
                    html.Append("<div>" + Markdown.ToHtml(materialsForDistribution) + "</div>");
                    html.Append("</div>");
                }
                if (materialsForParticipation.HasValue())
                {
                    html.Append("<div class='form-group mb-3'>");
                    html.Append("<label class='form-label'>Materials for Participation/Candidates</label>");
                    html.Append("<div>" + Markdown.ToHtml(materialsForParticipation) + "</div>");
                    html.Append("</div>");
                }
                ((ITextControl)materials).Text = html.ToString();
            }
        }

        #endregion
    }
}