using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Exceptions;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Events.Candidates.Controls
{
    public partial class CandidatePanel : SearchResultsGridViewController<NullFilter>
    {
        #region Properties

        protected override bool IsFinder => false;

        protected override int DefaultPageSize => 200;

        protected Guid EventIdentifier
        {
            get => (Guid)ViewState[nameof(EventIdentifier)];
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        public int CandidateCount
        {
            get => (int)(ViewState[nameof(CandidateCount)] ?? 0);
            private set => ViewState[nameof(CandidateCount)] = value;
        }

        public bool IsInited
        {
            get => (bool)(ViewState[nameof(IsInited)] ?? false);
            private set => ViewState[nameof(IsInited)] = value;
        }

        protected bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            private set => ViewState[nameof(CanWrite)] = value;
        }

        #endregion

        #region Fields

        private List<QEventAssessmentForm> _exams;

        private List<Tuple<Guid, string>> _itemsState;

        #endregion

        #region Events

        public event EventHandler Refreshed;
        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        public event EventHandler ExamAssigned;
        private void OnExamAssigned() => ExamAssigned?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_ItemDataBound;

            FilterButton.Click += FilterButton_Click;

            SortSelector.AutoPostBack = true;
            SortSelector.ValueChanged += SortSelector_ValueChanged;

            DownloadScrapPaper.Click += DownloadScrapPaper_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                TryLoadSettings();
        }

        public void LoadData(QEvent @event, Guid? registrationIdentifier, bool canWrite)
        {
            CanWrite = canWrite;

            TryLoadSettings();

            EventIdentifier = @event.EventIdentifier;

            VerifyCandidates.NavigateUrl = $"/ui/admin/registrations/exams/verify?event={@event.EventIdentifier}";
            AddCandidates.NavigateUrl = $"/ui/admin/registrations/exams/add?event={@event.EventIdentifier}";
            AddCandidates.Enabled = @event.EventSchedulingStatus != "Cancelled";

            VerifyCandidates.Visible = canWrite;
            AddCandidates.Visible = canWrite;

            Search(new NullFilter());

            if (registrationIdentifier.HasValue)
                ScriptManager.RegisterStartupScript(Page, GetType(), "scrollto_registration", $"$(document).ready(function() {{ candidatePanel.scrollToRegistration('{registrationIdentifier.ToString().ToLower()}'); }});", true);
        }

        private void TryLoadSettings()
        {
            if (IsInited)
                return;

            IsInited = true;

            var settings = LoadSettings();
            if (settings == null)
                return;

            var option = SortSelector.FindOptionByValue(settings.SortBy, true);
            if (option != null)
                option.Selected = true;
        }

        #endregion

        #region Methods (event handling)

        private void SortSelector_ValueChanged(object sender, EventArgs e)
        {
            RefreshGrid();

            SaveSettings(new ControlSettings
            {
                SortBy = SortSelector.Value
            });
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(CandidatePanel),
                "filter_focus",
                $"inSite.common.baseInput.focus('{FilterTextBox.ClientID}', true);",
                true);
        }

        protected override void OnRowCommand(GridViewRow row, string name, object argument)
        {
            Server.ScriptTimeout = 60 * 5; // 5 minutes

            var isHandled = false;

            if (name == "CancelSelection")
            {
                CancelSelection(row);

                isHandled = true;
            }
            else if (name == "CompleteSelection")
            {
                CompleteSelection(row);
                OnExamAssigned();

                isHandled = true;
            }
            else if (name == "StartVerification")
            {
                StartVerification(row);

                isHandled = true;
            }

            if (isHandled)
            {
                SaveItemsState();

                SearchWithCurrentPageIndex(Filter);
            }
            else
                base.OnRowCommand(row, name, argument);
        }

        private void SaveItemsState()
        {
            _itemsState = new List<Tuple<Guid, string>>();

            foreach (GridViewRow row in Grid.Rows)
            {
                if (!IsContentItem(row))
                    continue;

                var examAssetKeyInput = (ComboBox)row.FindControl("ExamAssetKey");

                var rowId = Grid.GetDataKey<Guid>(row);
                var examAssetKey = examAssetKeyInput.Visible
                    ? examAssetKeyInput.Value
                    : null;

                _itemsState.Add(new Tuple<Guid, string>(rowId, examAssetKey));
            }
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var container = e.Row;

            var registrationIdentifier = (Guid)DataBinder.Eval(container.DataItem, "RegistrationIdentifier");
            var form = (Guid?)DataBinder.Eval(container.DataItem, "FormIdentifier");
            var isAssigned = form.HasValue;

            var stateItem = _itemsState != null
                ? _itemsState.FirstOrDefault(x => x.Item1 == registrationIdentifier)
                : null;

            var assetLink = container.FindControl("AssetLink");
            var assetNumber = container.FindControl("AssetNumber");
            assetLink.Visible = isAssigned;
            assetNumber.Visible = isAssigned;

            var assignedExamPanel = container.FindControl("AssignedExamPanel");
            assignedExamPanel.Visible = CanWrite && isAssigned;

            var unassignedExamPanel = container.FindControl("UnassignedExamPanel");
            unassignedExamPanel.Visible = CanWrite && !isAssigned;

            var registrationPanel = container.FindControl("RegistrationPanel");
            registrationPanel.Visible = isAssigned;

            var cancelSelection = (IconButton)container.FindControl("CancelRegistrationButton");
            cancelSelection.Visible = isAssigned;

            if (!isAssigned)
            {
                if (_exams == null)
                    _exams = ServiceLocator.EventSearch.GetEventAssessmentForms(EventIdentifier);

                var examComboBox = (ComboBox)container.FindControl("ExamAssetKey");
                examComboBox.LoadItems(_exams.Select(x => new Shift.Common.ListItem
                {
                    Value = x.FormIdentifier.ToString(),
                    Text = x.FormTitle,
                    Description = x.FormName
                }));

                if (stateItem != null && stateItem.Item2.IsNotEmpty())
                {
                    var option = examComboBox.FindOptionByValue(stateItem.Item2);
                    if (option != null)
                        option.Selected = true;
                }
            }

            container.Attributes["data-registration"] = DataBinder.Eval(container.DataItem, "RegistrationIdentifier").ToString().ToLower();

            var registrationLink = (IconLink)e.Row.FindControl("RegistrationLink");
            registrationLink.NavigateUrl = new ReturnUrl($"event={EventIdentifier}&panel=candidates").GetRedirectUrl($"/ui/admin/registrations/exams/edit?registration={registrationIdentifier}");

            var returnUrl = $"/ui/admin/registrations/exams/edit?registration={registrationIdentifier}";
            var historyLink = (IconLink)e.Row.FindControl("HistoryLink");
            historyLink.NavigateUrl = $"/ui/admin/logs/aggregates/outline?aggregate={registrationIdentifier}&returnURL=" + HttpUtility.UrlEncode(returnUrl);

            var deleteButton = (IconLink)e.Row.FindControl("DeleteButton");
            deleteButton.NavigateUrl = new ReturnUrl($"event={EventIdentifier}&panel=candidates")
                .GetRedirectUrl($"/ui/admin/registrations/exams/delete?id={registrationIdentifier}");
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SearchWithCurrentPageIndex(Filter);
            OnRefreshed();
        }

        private void DownloadScrapPaper_Click(object sender, EventArgs e)
        {
            var data = ScrapsPrint.RenderPdf(EventIdentifier);
            if (data != null)
                Response.SendFile($"Scrap-Paper", "pdf", data);
        }

        #endregion

        #region Methods (searching)

        protected override int SelectCount(NullFilter filter)
        {
            var f = new QRegistrationFilter
            {
                EventIdentifier = EventIdentifier,
                Candidate = FilterTextBox.Text
            };

            CandidateCount = ServiceLocator.RegistrationSearch.CountRegistrations(f);
            return CandidateCount;
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var isFullNameOrder = SortSelector.Value == "full";
            var orderBy = isFullNameOrder ? "Candidate.UserFullName" : "Candidate.UserLastName,Candidate.UserFirstName";

            var registrations = ServiceLocator.RegistrationSearch
                .GetRegistrationsByEvent(EventIdentifier, FilterTextBox.Text, orderBy: orderBy);

            var candidates = PersonCriteria
                .Bind(
                    x => new
                    {
                        x.UserIdentifier,
                        x.PersonCode,
                        x.User.FullName,
                        x.User.FirstName,
                        x.User.LastName
                    },
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.OrganizationIdentifier,
                        IncludeUserIdentifiers = registrations.Select(x => x.CandidateIdentifier).Distinct().ToArray()
                    })
                .ToDictionary(x => x.UserIdentifier);

            var formIds = registrations.Where(x => x.ExamFormIdentifier.HasValue).Select(x => x.ExamFormIdentifier.Value).Distinct().ToArray();
            var forms = ServiceLocator.BankSearch.GetForms(formIds).ToDictionary(x => x.FormIdentifier);

            var list = new List<ExamCandidate>(registrations.Count);

            foreach (var registration in registrations)
            {
                var item = new ExamCandidate
                {
                    RegistrationIdentifier = registration.RegistrationIdentifier,
                    CandidateIdentifier = registration.CandidateIdentifier,
                    FormIdentifier = registration.ExamFormIdentifier
                };

                if (candidates.TryGetValue(item.CandidateIdentifier, out var candidate))
                {
                    item.CandidateCode = candidate.PersonCode;
                    item.CandidateName = isFullNameOrder ? candidate.FullName : $"{candidate.LastName}, {candidate.FirstName}";
                }

                if (item.FormIdentifier.HasValue && forms.TryGetValue(item.FormIdentifier.Value, out var form))
                {
                    item.FormCode = form?.FormCode;
                    item.FormTitle = form?.FormTitle;
                    item.FormName = form?.FormName;
                }

                list.Add(item);
            }

            return list.ToSearchResult();
        }

        #endregion

        #region Methods (commands)

        private void CancelSelection(GridViewRow row)
        {
            var registrationIdentifier = Grid.GetDataKey<Guid>(row);
            ServiceLocator.SendCommand(new UnassignExamForm(registrationIdentifier));
            ServiceLocator.SendCommand(new LimitExamTime(registrationIdentifier));
        }

        private void CompleteSelection(GridViewRow row)
        {
            var formIdentifier = ((ComboBox)row.FindControl("ExamAssetKey")).ValueAsGuid;
            if (!formIdentifier.HasValue)
                return;

            var registrationIdentifier = Grid.GetDataKey<Guid>(row);
            ServiceLocator.SendCommand(new AssignExamForm(registrationIdentifier, formIdentifier.Value, null));
            ServiceLocator.SendCommand(new LimitExamTime(registrationIdentifier));
        }

        private void StartVerification(GridViewRow row)
        {
            var registration = Grid.GetDataKey<Guid>(row);

            try
            {
                ServiceLocator.SendCommand(new ChangeEligibility(registration, "Check Eligibility in DA", null));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ScriptManager.RegisterStartupScript(
                        Page,
                        GetType(),
                        nameof(StartVerification),
                        "alert(" + HttpUtility.JavaScriptStringEncode(ex.InnerException.Message, true) + ");",
                        true);
                else
                    throw;
            }
        }

        #endregion

        #region Methods (personalization)

        private class ControlSettings
        {
            public string SortBy { get; set; }
        }

        private ControlSettings LoadSettings() =>
            PersonalizationRepository.GetValue<ControlSettings>(Organization.OrganizationIdentifier, User.UserIdentifier, typeof(CandidatePanel).FullName, false);

        private void SaveSettings(ControlSettings value) =>
            PersonalizationRepository.SetValue(Organization.OrganizationIdentifier, User.UserIdentifier, typeof(CandidatePanel).FullName, value);

        #endregion

        #region Helper methods

        protected string GetCandidateStatus(object o)
        {
            var candidate = (ExamCandidate)o;
            var html = new StringBuilder();

            try
            {
                var registration = ServiceLocator.RegistrationSearch.GetRegistration(candidate.RegistrationIdentifier);

                if (registration != null)
                {
                    html.Append("<div class='workflow-status float-end'>");

                    if (registration.SynchronizationStatus != null)
                    {
                        html.Append("<span class='badge bg-custom-default'>");
                        html.Append(registration.SynchronizationStatus);
                        html.Append("</span> ");
                    }

                    AddRegistrationProcess(registration, html);

                    html.Append("</div>");
                }
            }
            catch (AggregateNotFoundException)
            {
                html.Append($"Registration not found: {candidate.RegistrationIdentifier}");
            }

            return html.ToString();
        }

        private static void AddRegistrationProcess(QRegistration registration, StringBuilder html)
        {
            if (registration.SynchronizationProcess == null)
                return;

            var sync = ServiceLocator.Serializer.Deserialize<ProcessState>(registration.SynchronizationProcess);
            if (sync == null)
                return;

            if (sync.HasErrors)
            {
                html.Append("<span class='badge bg-danger'>");
                html.Append("Synchronization Error".ToQuantity(sync.Errors.Length));
                html.Append("</span> ");
            }

            if (sync.HasWarnings)
            {
                html.Append("<span class='badge bg-warning'>");
                html.Append("Synchronization Warning".ToQuantity(sync.Warnings.Length));
                html.Append("</span> ");
            }
        }

        protected string GetRegistrationStatus(object id)
        {
            var html = new StringBuilder();

            if (id is Guid guid)
            {

                try
                {
                    var registration = ServiceLocator.RegistrationSearch.GetRegistration(
                        guid,
                        x => x.Accommodations,
                        x => x.RegistrationInstructors.Select(y => y.Instructor)
                    );

                    if (registration != null)
                    {
                        html.Append(registration.ApprovalStatus ?? "Pending");

                        html.Append("<div class='workflow-status float-end'>");

                        AddRegistrationScore(registration, html);
                        AddAttendanceStatus(registration, html);
                        AddAccommodations(registration, html);
                        AddTrainingProviders(registration, html);

                        html.Append("</div>");

                        html.Append($"<div class='form-text'>Exam Password: {registration.RegistrationPassword}</div>");
                    }
                }
                catch (AggregateNotFoundException)
                {
                    html.Append($"Registration not found: {id}");
                }
            }
            else
            {
                html.Append("Pending exam form selection");
            }

            return html.ToString();
        }

        private static void AddRegistrationScore(QRegistration registration, StringBuilder html)
        {
            if (registration.Score.HasValue)
            {
                html.Append($"{registration.Score:p0} {registration.Grade}");
                return;
            }

            var eligibility = ServiceLocator.Serializer.Deserialize<ProcessState>(registration.EligibilityProcess);

            if (eligibility != null && eligibility.HasErrors)
            {
                {
                    html.Append("<span class='badge bg-danger'>");
                    html.Append("Verification Error".ToQuantity(eligibility.Errors.Length));
                    html.Append("</span> ");
                }
            }

            if (eligibility != null && eligibility.HasWarnings)
            {
                {
                    html.Append("<span class='badge bg-warning'>");
                    html.Append("Verification Warning".ToQuantity(eligibility.Warnings.Length));
                    html.Append("</span> ");
                }
            }
        }

        private static void AddAttendanceStatus(QRegistration registration, StringBuilder html)
        {
            if (registration.IsPresent)
                html.Append("<span class='badge bg-success'>Attended</span>");
            else if (registration.AttendanceStatus == "Absent")
                html.Append("<span class='badge bg-danger'>No Show</span>");
        }

        private static void AddAccommodations(QRegistration registration, StringBuilder html)
        {
            if (registration.Accommodations.Count == 0)
                return;

            var accommodations = string.Join("<br>", registration.Accommodations.Select(x => x.AccommodationType).OrderBy(x => x));
            html.Append($"<div class='accommodation'>{accommodations}</div>");
        }

        private static void AddTrainingProviders(QRegistration registration, StringBuilder html)
        {
            if (registration.SchoolIdentifier == null)
                return;

            var school = ServiceLocator.GroupSearch.GetGroup(registration.SchoolIdentifier.Value);
            if (school == null)
                return;

            html.Append($"<div class='instructor'>{school.GroupName}</div>");
        }

        protected string GetFormCode(object candidate) =>
            (candidate as ExamCandidate)?.FormCode;

        protected string GetFormIdentifier(object candidate) =>
            candidate is ExamCandidate w ? w.FormIdentifier.ToString() : string.Empty;

        protected string GetFormTitle(object candidate) =>
            (candidate as ExamCandidate)?.FormTitle;

        protected string GetFormName(object candidate) =>
            (candidate as ExamCandidate)?.FormName;

        #endregion
    }
}