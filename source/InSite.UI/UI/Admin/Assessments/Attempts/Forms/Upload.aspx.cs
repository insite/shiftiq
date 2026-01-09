using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Write;
using InSite.Application.Events.Read;
using InSite.Application.Events.Write;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;

using static InSite.Admin.Assessments.Attempts.Forms.ConfirmUploadModel;

namespace InSite.Admin.Assessments.Attempts.Forms
{
    public partial class Upload : AdminBasePage
    {
        #region Constants

        private const string ParentUrl = "/ui/admin/assessments/home";

        #endregion

        #region Properties

        private Guid? DefaultEventIdentifier
            => Guid.TryParse(Request["event"], out var value) ? value : (Guid?)null;

        private UploadModel Step1Data
        {
            get => (UploadModel)ViewState[nameof(Step1Data)];
            set => ViewState[nameof(Step1Data)] = value;
        }

        private UploadModel Step2Data
        {
            get => (UploadModel)ViewState[nameof(Step2Data)];
            set => ViewState[nameof(Step2Data)] = value;
        }

        private ConfirmUploadModel ConfirmData
        {
            get => (ConfirmUploadModel)ViewState[nameof(ConfirmData)];
            set => ViewState[nameof(ConfirmData)] = value;
        }

        protected string FormatExtensionsJson
        {
            get => (string)(ViewState[nameof(FormatExtensionsJson)] ?? "null");
            set => ViewState[nameof(FormatExtensionsJson)] = value;
        }

        #endregion

        #region Methods (loading and initialization)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EventIdentifier.AutoPostBack = true;
            EventIdentifier.ValueChanged += (s, a) => SetupEvent(EventIdentifier.Value);

            FormIdentifier.AutoPostBack = true;
            FormIdentifier.ValueChanged += (s, a) =>
            {
                bool isEventSelected = EventIdentifier.Value.HasValue;
                bool isFormSelected = FormIdentifier.Value.HasValue;
                if (!isEventSelected && isFormSelected)
                    ValidateFormAssertions(new Guid[] { FormIdentifier.Value.Value });
            };

            Step1NextButton.Click += Step1NextButton_Click;

            UploadFileFormat.AutoPostBack = true;
            UploadFileFormat.ValueChanged += UploadFileFormat_ValueChanged;

            FileUploadExtensionValidator.ServerValidate += FileUploadExtensionValidator_ServerValidate;
            FileUploadButton.Click += FileUploadButton_Click;

            SelectedAttemptRepeater.DataBinding += SelectedAttemptRepeater_DataBinding;
            SelectedAttemptRepeater.ItemCreated += SelectedAttemptRepeater_ItemCreated;

            ConfirmAllowDuplicates.AutoPostBack = true;
            ConfirmAllowDuplicates.CheckedChanged += ConfirmAllowDuplicates_CheckedChanged;

            ConfirmSaveButton.Click += ConfirmSaveButton_Click;
            ConfirmationUpdatePanel.Request += ConfirmationUpdatePanel_Request;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Server.ScriptTimeout = 60 * 60 * 5; // 5 hours

            if (!CanCreate)
                HttpResponseHelper.Redirect(ParentUrl);

            if (IsPostBack)
                return;

            SetupFormTitle(null);

            UploadFileFormat.LoadItems(
                AttemptUploadFileParser.FileFormats,
                nameof(AttemptUploadFileFormat.ID),
                nameof(AttemptUploadFileFormat.Title)
            );
            OnUploadFileFormatChanged();

            Step1CancelButton.NavigateUrl = ParentUrl;

            EventIdentifier.Filter.EventType = "Exam";

            LoadDefaultEvent();
        }

        private void LoadDefaultEvent()
        {
            var @event = DefaultEventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(DefaultEventIdentifier.Value, x => x.ExamForms, x => x.VenueLocation)
                : null;
            if (@event == null || @event.OrganizationIdentifier != Organization.Identifier)
                return;

            EventIdentifier.Value = @event.EventIdentifier;

            SetupEvent(@event);

            FormIdentifier.Value = @event.ExamForms.FirstOrDefault()?.FormIdentifier;

            Step1CancelButton.NavigateUrl = $"/ui/admin/events/exams/outline?event={@event.EventIdentifier}&tab=grades";
        }

        #endregion

        #region Methods (event handling)

        private void ValidateFormAssertions(Guid[] formIdentifiers)
        {
            var ok = true;

            foreach (var formIdentifier in formIdentifiers)
            {
                var form = ServiceLocator.BankSearch.GetFormData(formIdentifier);

                if (form != null && form.Specification.Type == SpecificationType.Static && form.StaticQuestionOrder.IsNotEmpty())
                {
                    var currentQuestions = form.GetStaticFormQuestionIdentifiersInOrder();
                    var assertionIsTrue = form.StaticQuestionOrder.SequenceEqual(currentQuestions);

                    if (!assertionIsTrue)
                    {
                        var error = GetQuestionOrderVerificationError(form);
                        ScreenStatus.AddMessage(AlertType.Error, error);
                        ok = false;
                    }
                }
            }

            Step1NextButton.Enabled = ok;
        }

        public static string GetQuestionOrderVerificationError(Domain.Banks.Form form)
        {
            var bankId = form.Specification.Bank.Identifier;
            var verified = form.StaticQuestionOrderVerified.Format(User.TimeZone, isHtml: false, nullValue: "Unknown");
            var link = $"/ui/admin/assessments/banks/outline?bank={bankId}&form={form.Identifier}";

            return
                $"The questions in assessment form <a href='{link}'>{form.Name}</a> are not in the same order as when it was last verified at {verified}. " +
                $"Please make sure that the questions are arranged exactly as intended and click <i>Verify Question Order</i> to indicate the questions order is valid and expected.";
        }

        private void Step1NextButton_Click(object sender, EventArgs e)
        {
            UploadTab.Visible = false;
            DataTab.Visible = false;

            if (!Page.IsValid)
                return;

            var form = ServiceLocator.BankSearch.GetForm(FormIdentifier.Value.Value);

            Step2Data = Step1Data?.Clone() ?? new UploadModel();
            Step2Data.BankIdentifier = form.BankIdentifier;
            Step2Data.FormIdentifier = form.FormIdentifier;
            Step2Data.FormTitle = form.FormTitle;

            SetupFormTitle(Step2Data);

            UploadTab.Visible = true;
            UploadTab.IsSelected = true;

            OnUploadFileFormatChanged();
        }

        private void UploadFileFormat_ValueChanged(object sender, EventArgs e) =>
            OnUploadFileFormatChanged();

        private void OnUploadFileFormatChanged()
        {
            var formatId = UploadFileFormat.Value;
            var info = AttemptUploadFileParser.FileFormats.Single(x => x.ID == formatId);
            var hasTypeItems = info.TypeItems.IsNotEmpty();

            if (info == AttemptUploadFileParser.FileFormatCsv)
                InstructionMultiView.SetActiveView(InstructionCsvView);
            else if (info == AttemptUploadFileParser.FileFormatScantron)
                InstructionMultiView.SetActiveView(InstructionScantronView);
            else if (info == AttemptUploadFileParser.FileFormatLxrMerge)
                InstructionMultiView.SetActiveView(InstructionLxrMergeView);

            UploadFileFormatTypeField.Visible = hasTypeItems;

            FormatExtensionsJson = JsonHelper.SerializeJsObject(info.Extensions);
            FileUploadLabel.InnerHtml = info.UploadLabel;

            if (hasTypeItems)
            {
                UploadFileFormatTypeLabel.InnerHtml = info.TypeLabel;
                UploadFileFormatType.LoadItems(
                    info.TypeItems,
                    nameof(AttemptUploadFileType.ID),
                    nameof(AttemptUploadFileType.Title));
            }
            else
            {
                UploadFileFormatType.Items.Clear();
            }
        }

        private void FileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (UploadFileFormat.Value.IsEmpty() || !FileUpload.HasFile)
                return;

            var formatId = UploadFileFormat.Value;
            var info = AttemptUploadFileParser.FileFormats.Single(x => x.ID == formatId);
            var ext = Path.GetExtension(FileUpload.PostedFile.FileName);

            args.IsValid = info.Extensions.Contains(ext);
        }

        private void FileUploadButton_Click(object sender, EventArgs e)
        {
            DataTab.Visible = false;

            OnUploadFileFormatChanged();

            if (!Page.IsValid || !ParseFile())
                return;

            var attempts = UploadAttemptList.GetData();

            var missingUsers = attempts.Count(x => !x.HasUserAccount);
            if (missingUsers > 0)
            {
                ScreenStatus.Indicator = AlertType.Warning;
                ScreenStatus.Text = $"No user account for <strong>{"Candidate".ToQuantity(missingUsers)}</strong>.";
            }

            var missingRegistrations = attempts.Count(x => !x.HasEventRegistration);
            if (missingRegistrations > 0)
            {
                ScreenStatus.Indicator = AlertType.Warning;
                ScreenStatus.Text = $"No exam event registration for <strong>{"Candidate".ToQuantity(missingRegistrations)}</strong>.";
            }

            DataTab.Visible = true;
            NavPanel.SelectedIndex = 1;
        }

        private void ConfirmSaveButton_Click(object sender, EventArgs e)
        {
            var newAttemptsCount = SaveAttempts();

            if (newAttemptsCount > 0)
            {
                ScreenStatus.Indicator = AlertType.Success;
                ScreenStatus.Text = "assessment attempt".ToQuantity(newAttemptsCount) + $" uploaded and saved successfully";
            }
            else
            {
                ScreenStatus.Indicator = AlertType.Warning;
                ScreenStatus.Text = "No attempts saved to the database.";
            }

            UploadAttemptList.LoadData(null);

            DataTab.Visible = false;
            NavPanel.SelectedIndex = 0;

            if (EventIdentifier.Value == DefaultEventIdentifier)
                HttpResponseHelper.Redirect(Step1CancelButton.NavigateUrl);
        }

        private void ConfirmationUpdatePanel_Request(object sender, EventArgs e)
        {
            var preprocessor = new AttemptUploadPreprocessor(ServiceLocator.BankSearch, ServiceLocator.AttemptSearch, PersonSearch.GetUserIdentifier);
            var uploadAttempts = UploadAttemptList.GetData().Where(x => x.HasUserAccount && x.IsValid).Select(x => x.Clone()).ToArray();
            var databaseAttempts = new List<DatabaseAttemptInfo>();

            preprocessor.CalculateScores(uploadAttempts);

            var sequence = 1;

            foreach (var formGroup in uploadAttempts.GroupBy(x => x.FormIdentifier.Value))
            {
                var existAttempts = ServiceLocator.AttemptSearch.BindAttempts(a => new DatabaseAttemptInfo
                {
                    FormIdentifier = a.FormIdentifier,
                    LearnerUserIdentifier = a.LearnerUserIdentifier,
                    AttemptSubmitted = a.AttemptSubmitted,
                    AttemptScore = a.AttemptScore,
                    AttemptIsPassing = a.AttemptIsPassing,
                    Questions = a.Questions.Select(q => new DatabaseQuestionInfo
                    {
                        QuestionSequence = q.QuestionSequence,
                        AnswerOptionSequence = a.Options
                            .Where(o => o.QuestionIdentifier == q.QuestionIdentifier && o.OptionKey == q.AnswerOptionKey)
                            .Select(o => (int?)o.OptionSequence)
                            .FirstOrDefault()
                    })
                }, new QAttemptFilter
                {
                    FormIdentifier = formGroup.Key,
                    LearnerUserIdentifiers = formGroup.Select(x => x.LearnerUserIdentifier.Value).Distinct().ToArray(),
                    IsImported = true
                });

                databaseAttempts.AddRange(existAttempts);

                foreach (var attempt in formGroup)
                {
                    if (!attempt.AttemptGraded.HasValue)
                        attempt.AttemptGraded = AttemptGraded.Value.Value;

                    attempt.Sequence = sequence++;

                    if (existAttempts.Any(x => x.FormIdentifier == attempt.FormIdentifier && x.LearnerUserIdentifier == attempt.LearnerUserIdentifier))
                        attempt.HasAttemptMatch = existAttempts.Any(x => x.IsMatch(attempt));
                }
            }

            ConfirmAllowDuplicates.Checked = true;

            ConfirmData = new ConfirmUploadModel
            {
                UploadAttempts = uploadAttempts,
                DatabaseAttempts = databaseAttempts.ToArray()
            };

            SelectedAttemptRepeater.DataBind();
        }

        private void ConfirmAllowDuplicates_CheckedChanged(object sender, EventArgs e)
        {
            SelectedAttemptRepeater.DataBind();
        }

        private void SelectedAttemptRepeater_DataBinding(object sender, EventArgs e)
        {
            var importData = GetConfirmUploadAttempts(ConfirmAllowDuplicates.Checked);
            var importCount = importData.Length;
            var hasData = importCount > 0;

            SelectedAttemptRepeater.DataSource = importData;

            ConfirmTitle.Text = $@"Are you sure you want to import {"exam attempt".ToQuantity(importCount)}?";

            if (ConfirmSaveButton.Visible != hasData)
            {
                ConfirmSaveButton.Visible = hasData;
                ConfirmButtonsUpdatePanel.Update();
            }
        }

        private void SelectedAttemptRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var attemptGraded = (DateTimeOffsetSelector)e.Item.FindControl("AttemptGraded");
            attemptGraded.AutoPostBack = true;
            attemptGraded.ValueChanged += ConfirmAttemptGraded_ValueChanged;
        }

        private void ConfirmAttemptGraded_ValueChanged(object sender, EventArgs e)
        {
            var dateSelector = (DateTimeOffsetSelector)sender;
            var repeaterItem = (RepeaterItem)dateSelector.NamingContainer;
            var sequence = int.Parse(((ITextControl)repeaterItem.FindControl("AttemptSequence")).Text);
            var dataItem = ConfirmData.UploadAttempts.FirstOrDefault(x => x.Sequence == sequence);

            dataItem.AttemptGraded = dateSelector.Value ?? AttemptGraded.Value.Value;

            if (dataItem.HasAttemptMatch.HasValue)
                dataItem.HasAttemptMatch = ConfirmData.DatabaseAttempts.Any(x => x.IsMatch(dataItem));

            SelectedAttemptRepeater.DataBind();
        }

        #endregion

        #region Methods (data binding)

        private void SetupEvent(Guid? id)
        {
            var @event = id.HasValue
                ? ServiceLocator.EventSearch.GetEvent(id.Value, x => x.VenueLocation, x => x.ExamForms)
                : null;

            SetupEvent(@event);
        }

        private void SetupEvent(QEvent @event)
        {
            var isEventFound = @event != null;

            EventInfoFields.Visible = isEventFound;
            ExamCandidateCol.Visible = isEventFound;

            FormIdentifier.Value = null;
            AttemptGraded.Enabled = true; // !isEventFound;
            AttemptGraded.Value = null;

            Step1Data = new UploadModel(@event);

            if (isEventFound)
            {
                var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(
                    @event.EventIdentifier, includeAttempt: true, includeCandidate: true);

                FormIdentifier.Filter.FormIdentifiers = @event.ExamForms.Count == 0
                    ? new[] { Guid.Empty }
                    : @event.ExamForms.Select(x => x.FormIdentifier).ToArray();

                ValidateFormAssertions(FormIdentifier.Filter.FormIdentifiers);

                VenueName.Text = @event.VenueLocation?.GroupName;
                VenueRoom.Text = @event.VenueRoom;

                StartTime.Text = @event.EventScheduledStart.Format(User.TimeZone);
                ClassSessionCode.Text = @event.EventClassCode ?? "None";

                AttemptGraded.Value = @event.EventScheduledStart.AddMinutes(@event.ExamDurationInMinutes ?? 0);
                if (registrations.Where(x => x.Attempt?.AttemptGraded != null).Any())
                    AttemptGraded.Value = registrations.Where(x => x.Attempt?.AttemptGraded != null).Max(x => x.Attempt.AttemptGraded.Value);

                ExamCandidateRepeater.DataSource = registrations;
                ExamCandidateRepeater.DataBind();
            }
            else
            {
                FormIdentifier.Filter.FormIdentifiers = null;
            }
        }

        private void SetupFormTitle(UploadModel data)
        {
            string qualifier = null;

            if (data != null)
            {
                if (data.EventIdentifier.HasValue && data.FormIdentifier.HasValue)
                    qualifier = $"{data.EventTitle} <span class=\"form-text\">{data.FormTitle}</span>";
                else if (data.EventIdentifier.HasValue)
                    qualifier = data.EventTitle;
                else if (data.FormIdentifier.HasValue)
                    qualifier = data.FormTitle;
            }

            PageHelper.AutoBindHeader(this, qualifier: qualifier);
        }

        private AttemptUploadAnswer[] GetConfirmUploadAttempts(bool allowDuplicates)
        {
            return allowDuplicates
                ? ConfirmData.UploadAttempts.Where(x => (x.HasAttemptMatch ?? false) == false).ToArray()
                : ConfirmData.UploadAttempts.Where(x => x.HasAttemptMatch == null).ToArray();
        }

        #endregion

        #region Methods (file uploading)

        private bool ParseFile()
        {
            AttemptUploadFileLine[] uploadItems;

            try
            {
                var formatId = UploadFileFormat.Value;
                var formatInfo = AttemptUploadFileParser.FileFormats.Single(x => x.ID == formatId);
                var typeId = UploadFileFormatType.Visible ? int.Parse(UploadFileFormatType.Value) : -1;

                uploadItems = formatInfo.Parse(FileUpload.FileContent, typeId, Encoding.UTF8);
            }
            catch (ApplicationError ex)
            {
                ScreenStatus.Indicator = AlertType.Error;
                ScreenStatus.Text = HttpUtility.HtmlEncode(ex.Message);

                return false;
            }

            var preprocessor = new AttemptUploadPreprocessor(ServiceLocator.BankSearch, ServiceLocator.AttemptSearch, PersonSearch.GetUserIdentifier);
            var attempts = preprocessor.Process(uploadItems, AttemptGraded.Value.Value.GetTimeZone(), Step2Data.EventIdentifier, Step2Data.FormIdentifier);

            if (attempts == null)
                return false;

            if (attempts.Length == 0)
            {
                ScreenStatus.Indicator = AlertType.Error;
                ScreenStatus.Text = !string.IsNullOrEmpty(preprocessor.Error)
                    ? preprocessor.Error
                    : "The file you uploaded has no data.";

                return false;
            }

            InstructionMultiView.SetActiveView(InstructionSummaryView);

            SummaryRepeater.DataSource = preprocessor.Summaries;
            SummaryRepeater.DataBind();

            WarningRepeater.Visible = preprocessor.Warnings.Count > 0;
            WarningRepeater.DataSource = preprocessor.Warnings;
            WarningRepeater.DataBind();

            UploadAttemptList.LoadData(attempts);

            ScreenStatus.Indicator = AlertType.Success;
            ScreenStatus.Text = $"<strong>{attempts.Length}</strong> attempts have been uploaded and ready to be saved.";

            SaveButton.Visible = attempts.Any(x => x.HasUserAccount);

            return true;
        }

        #endregion

        #region Methods (database saving)

        private int SaveAttempts()
        {
            var count = 0;
            var allowDuplicates = ConfirmAllowDuplicates.Checked;
            var validAttempts = GetConfirmUploadAttempts(allowDuplicates);
            var registrations = Step2Data.EventIdentifier.HasValue
                ? ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(Step2Data.EventIdentifier.Value)
                : null;

            for (var i = 0; i < validAttempts.Length; i++)
            {
                var attempt = validAttempts[i];
                var form = ServiceLocator.BankSearch.GetFormData(attempt.FormIdentifier.Value);
                var formQuestions = form.GetQuestions();

                count++;

                var answers = new List<AnswerHandle>();

                for (int j = 0; j < attempt.Questions.Length; j++)
                {
                    var formQuestion = formQuestions[j];
                    var handle = new AnswerHandle
                    {
                        Question = formQuestion.Identifier,
                        Options = formQuestion.Options.Select(x => x.Number).ToArray()
                    };

                    var attemptQuestion = attempt.Questions[j];
                    var optionIndex = attemptQuestion.OptionIndex;
                    if (optionIndex >= 0 && optionIndex < formQuestion.Options.Count)
                        handle.Answer = formQuestion.Options[optionIndex].Number;

                    answers.Add(handle);
                }

                var candidate = attempt.LearnerUserIdentifier.Value;
                var registration = registrations?.FirstOrDefault(x => x.CandidateIdentifier == candidate);

                ServiceLocator.SendCommand(new ImportAttempt(
                    UniqueIdentifier.Create(),
                    Organization.OrganizationIdentifier,
                    answers.ToArray(),
                    Step2Data.EventScheduled,
                    attempt.AttemptGraded.Value,
                    attempt.Tag,
                    form.Specification.Bank.Identifier,
                    attempt.FormIdentifier.Value,
                    candidate,
                    registration?.RegistrationIdentifier,
                    registration != null && attempt.IsAttended,
                    "en"
                ));

                if (registration != null && !registration.IsPresent)
                {
                    registration.AttendanceStatus = "Present";
                    ServiceLocator.SendCommand(new TakeAttendance(registration.RegistrationIdentifier, "Present", null, null));
                }
            }

            if (registrations != null)
            {
                foreach (var registration in registrations)
                {
                    if (!registration.IsPresent && registration.AttendanceStatus != "Absent")
                        ServiceLocator.SendCommand(new TakeAttendance(registration.RegistrationIdentifier, "Absent", null, null));
                }
            }

            if (count > 0)
                ServiceLocator.SendCommand(new AnalyzeForm(
                    Step2Data.BankIdentifier.Value,
                    Step2Data.FormIdentifier.Value
                ));

            if (Step2Data.EventIdentifier.HasValue)
                ServiceLocator.SendCommand(new ImportExamAttempts(Step2Data.EventIdentifier.Value, allowDuplicates));

            return count;
        }

        #endregion
    }
}