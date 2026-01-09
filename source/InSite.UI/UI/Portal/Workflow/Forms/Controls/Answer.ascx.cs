using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Files.Read;
using InSite.Application.Responses.Write;
using InSite.Application.Surveys.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;
using InSite.UI.Portal.Issues.Controls;
using InSite.UI.Portal.Workflow.Forms.Models;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class Answer : SubmissionSessionControl
    {
        private AnswerInputControlBuilder _builder;
        private GlossaryHelper _glossaryHelper;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnswerGroupRepeater.ItemDataBound += AnswerGroupRepeater_ItemDataBound;
            LaunchButton.Click += LaunchButton_Click;
            PreviousButton.Click += PreviousButton_Click;
            NextButton.Click += NextButton_Click;
            ConfirmButton.Click += ConfirmButton_Click;
        }

        private void AnswerGroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var group = (AnswerGroup)e.Item.DataItem;
            var itemRepeater = (Repeater)e.Item.FindControl("AnswerItemRepeater");
            itemRepeater.ItemDataBound += AnswerItemRepeater_ItemDataBound;
            itemRepeater.DataSource = group.AnswerPackets;
            itemRepeater.DataBind();
        }

        private void AnswerItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (AnswerItem)e.Item.DataItem;
            var question = Current.Survey.FindQuestion(item.Question);

            if (!question.DisplayAnswerInput())
                return;

            var ph = (PlaceHolder)e.Item.FindControl("InputPlaceholder");
            var answer = GetAnswerText(Current.Session, question.Identifier);

            foreach (var ctrl in _builder.CreateInputControls(Current.SessionIdentifier, question, answer, item))
                ph.Controls.Add(ctrl);

            if (question.ListEnableOtherText)
                ph.Controls.Add(_builder.CreateOtherControl(question, answer));
        }

        private void LaunchButton_Click(object sender, EventArgs e)
            => SubmissionSessionNavigator.RedirectTo(ResponseVerb.Launch, Current.SessionIdentifier);

        private void PreviousButton_Click(object sender, EventArgs e)
            => Previous();

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Next();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Confirm();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ActionButtons.Visible = false;

            if (!Current.IsValid || Current.FormIdentifier == Guid.Empty)
                return;

            if (!IsSurveyOpen())
                return;

            if (CheckGradebookLocked())
                return;

            EnsureSubmissionStarted();

            if (!Current.IsAdminAccess)
            {
                if (Current.IsLocked)
                    Navigator.RedirectToCompletePage(Current.SessionIdentifier);

                if (Current.IsCompleted && (Current.IsRespondentAnonymous || !Current.IsRespondentValid))
                    Navigator.RedirectToCompletePage(Current.SessionIdentifier);
            }

            IeWarning.Visible = HttpRequestHelper.IsIE;

            _glossaryHelper = new GlossaryHelper(Current.Language, true);
            _builder = new AnswerInputControlBuilder(Current, _glossaryHelper);

            if (Current.PageNumber < 1)
                Navigator.RedirectToAnswerPage(Current.SessionIdentifier, 1, null);

            if (Current.PageNumber > Current.PageCount)
                Navigator.RedirectToConfirmPage(Current.SessionIdentifier);

            var questions = SubmissionSessionHelper.GetCurrentPageActiveQuestions(Current);
            if (questions.Count == 0)
                Navigator.RedirectToNextPage(Current.SessionIdentifier, Current.PageNumber);

            if (CheckMissedAnswers())
                Current.ReloadCurrentSession();

            var answers = questions
                .Select(x => new AnswerItem
                {
                    Question = x.Identifier,
                    QuestionSequence = Navigator.GetQuestionNumber(x.Identifier),
                    QuestionBody = x.Content != null
                        ? _glossaryHelper.Process(
                            x.Identifier,
                            ContentLabel.Title,
                            x.Content.Title.GetHtml(Current.Language)
                                .IfNullOrEmpty(x.Content.Title.GetHtml()))
                        : string.Empty,
                    QuestionIsNested = x.IsNested,
                    QuestionIsHidden = !x.DisplayAnswerInput(),
                    QuestionIsRequired = x.IsRequired,
                    AnswerInputType = x.Type,
                })
                .ToList();

            var prefix = AppPage.Translator.Translate("Question");

            foreach (var answer in answers)
            {
                var question = Current.Survey.FindQuestion(answer.Question);
                if (question.Code.HasValue())
                {
                    var indicator = question.GetIndicatorStyleName();
                    answer.QuestionHeader = $"{prefix} <span class='fs-2 badge bg-{indicator}'>{question.Code}</span>";
                }
                else if (answer.QuestionSequence > 0)
                {
                    answer.QuestionHeader = $"{prefix} {answer.QuestionSequence}";
                }
                else
                {
                    answer.IsQuestionHeaderVisible = false;
                }
            }

            var groups = CreateAnswerGroups(answers);

            AnswerGroupRepeater.DataSource = groups;
            AnswerGroupRepeater.DataBind();

            var pageHeaderHtml = Markdown.ToHtml(Current.Survey.Content?.GetText(ContentLabel.PageHeader, Current.Language, true));
            var pageFooterHtml = Markdown.ToHtml(Current.Survey.Content?.GetText(ContentLabel.PageFooter, Current.Language, true));

            PageHeaderPanel.Visible = !string.IsNullOrEmpty(pageHeaderHtml);
            PageHeader.Text = pageHeaderHtml;

            PageFooterPanel.Visible = !string.IsNullOrEmpty(pageFooterHtml);
            PageFooter.Text = pageFooterHtml;

            ActionButtons.Visible = true;
            LaunchButton.Visible = Current.PageNumber == 1 && !Launch.AutoStart(Current);
            PreviousButton.Visible = Current.PageNumber > 1;
            NextButton.Visible = Current.PageNumber < Current.PageCount;
            ConfirmButton.Visible = Current.PageNumber == Current.PageCount;

            TermsData.Value = _glossaryHelper.GetJsonDictionary().IfNullOrEmpty("null");

            {
                var autoCalcValues = Current.Survey.Questions
                    .Where(x => x.NumberEnableAutoCalc)
                    .SelectMany(x => x.NumberAutoCalcQuestions.EmptyIfNull())
                    .Distinct()
                    .ToDictionary(x => x, x => GetAnswerText(Current.Session, x));

                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(Answer),
                    "setup_autocalc",
                    $"answerPage.setupAutoCalc({JsonHelper.SerializeJsObject(autoCalcValues)});",
                    true);
            }
        }

        private bool IsSurveyOpen()
        {
            if (Current.Survey.Status == SurveyFormStatus.Closed || Current.Survey.Status == SurveyFormStatus.Archived)
            {
                var instruction = Current.Survey.Content?.GetHtml(ContentLabel.ClosedInstructions);
                if (instruction != null)
                {
                    ErrorAlert.AddMessage(AlertType.Error, $"{Translate(instruction).Trim()}");
                    return false;
                }

                StatusAlert.AddMessage(AlertType.Error, Translate("Form has been closed."));
                return false;
            }

            return true;
        }

        private void EnsureSubmissionStarted()
        {
            if (Current.Session.ResponseSessionStarted.HasValue)
                return;

            var submission = Current.Session;
            var noStatusChange = !string.Equals(submission.ResponseSessionStatus, ResponseSessionStatus.Created.ToString(), StringComparison.OrdinalIgnoreCase);
            
            ServiceLocator.SendCommand(new StartResponseSession(
                submission.ResponseSessionIdentifier,
                submission.ResponseSessionCreated,
                noStatusChange
            ));
        }

        private bool CheckMissedAnswers()
        {
            var sessionId = Current.Session.ResponseSessionIdentifier;
            var sessionAnswers = Current.Session.QResponseAnswers.Select(x => x.SurveyQuestionIdentifier).ToHashSet();
            var sessionOptions = Current.Session.QResponseOptions.Select(x => x.SurveyOptionIdentifier).ToHashSet();

            var commands = new List<ICommand>();

            foreach (var question in Current.Survey.Questions)
            {
                if (!question.HasInput)
                    continue;

                if (!sessionAnswers.Contains(question.Identifier))
                    commands.Add(new AddResponseAnswer(Current.Session.ResponseSessionIdentifier, question.Identifier));

                var options = question.FlattenOptionItems();
                if (options.Length == 0)
                    continue;

                var missedOptions = options.Select(x => x.Identifier).Where(x => !sessionOptions.Contains(x)).ToArray();
                if (missedOptions.Length > 0)
                    commands.Add(new AddResponseOptions(sessionId, question.Identifier, missedOptions));
            }

            var hasChanges = commands.Count > 0;

            if (hasChanges)
                ServiceLocator.SendCommands(commands);

            return hasChanges;
        }

        private bool CheckGradebookLocked()
        {
            if (!LockedGradebookHelper.HasLockedGradebook(Current.Survey.Identifier, Current.Survey.Hook))
                return false;

            StatusAlert.AddMessage(AlertType.Error, Translate("The gradebook is locked, please contact the administrator for details."));

            return true;
        }

        private AnswerGroup[] CreateAnswerGroups(List<AnswerItem> answers)
        {
            var groups = new List<AnswerGroup>();

            for (int i = 0; i < answers.Count; i++)
            {
                var packet = answers[i];

                if (i != 0 && packet.QuestionIsNested)
                {
                    var nest = groups.Last();
                    nest.AnswerPackets.Add(packet);
                }
                else
                {
                    groups.Add(new AnswerGroup(packet));
                }
            }

            return groups.ToArray();
        }

        private void Previous()
        {
            if (SaveAnswers())
                Navigator.RedirectToPreviousPage(Current.SessionIdentifier, Current.PageNumber);
        }

        private void Next()
        {
            if (SaveAnswers())
                Navigator.RedirectToNextPage(Current.SessionIdentifier, Current.PageNumber);
        }

        private void Confirm()
        {
            if (SaveAnswers())
                Navigator.RedirectToConfirmPage(Current.SessionIdentifier);
        }

        private bool SaveAnswers()
        {
            var questions = SubmissionSessionHelper.GetCurrentPageActiveQuestions(Current);

            if (!CheckUploadsValid(questions))
                return false;

            var newValues = new Dictionary<Guid, string>();

            foreach (var question in questions)
            {
                var inputID = $"Question_{question.Sequence}";
                var control = ControlHelper.GetControl(AnswerGroupRepeater, inputID);
                if (control != null)
                {
                    string oldAnswer = GetAnswerText(Current.Session, question.Identifier);
                    string newAnswer = oldAnswer;

                    if (question.Type == SurveyQuestionType.Number)
                        newAnswer = question.NumberEnableAutoCalc ? oldAnswer : ((HtmlInputText)control).Value;

                    if (control is DateSelector date)
                        newAnswer = date.Value?.ToString("yyyy-MM-dd");

                    else if (control is Common.Web.UI.TextBox text)
                        newAnswer = text.Text;

                    else if (control is FormFileUpload upload)
                    {
                        newAnswer = UploadFile(upload.UploadedFiles, oldAnswer);

                        upload.ClearUploadedFiles();

                        if (question.IsRequired && string.IsNullOrEmpty(oldAnswer) && string.IsNullOrEmpty(newAnswer))
                        {
                            var error = AppPage.GetDisplayHtml("Missing Required Form Answer", $"Please answer all required questions on this page.");
                            ErrorAlert.AddMessage(AlertType.Error, "fas fa-exclamation-square", error);
                            return false;
                        }
                    }

                    if (oldAnswer != newAnswer)
                    {
                        newValues[question.Identifier] = newAnswer;
                        SendCommand(new ChangeResponseAnswer(Current.Session.ResponseSessionIdentifier, question.Identifier, newAnswer));
                    }
                }

                if (question.ListEnableOtherText)
                {
                    var otherID = $"Other_{question.Sequence}";
                    var other = Common.Web.UI.ControlHelper.GetControl(AnswerGroupRepeater, otherID);

                    if (other is Common.Web.UI.TextBox text)
                    {
                        string oldAnswer = GetAnswerText(Current.Session, question.Identifier);
                        string newAnswer = text.Text;

                        if (oldAnswer != newAnswer)
                        {
                            newValues[question.Identifier] = newAnswer;
                            SendCommand(new ChangeResponseAnswer(Current.Session.ResponseSessionIdentifier, question.Identifier, newAnswer));
                        }
                    }
                }
            }

            foreach (var question in Current.Survey.Questions)
            {
                if (!question.NumberEnableAutoCalc)
                    continue;

                decimal? sumValue = null;

                foreach (var questionId in question.NumberAutoCalcQuestions)
                {
                    var strValue = newValues.ContainsKey(questionId)
                        ? newValues[questionId]
                        : GetAnswerText(Current.Session, questionId);

                    if (!decimal.TryParse(strValue, out var decValue))
                        continue;

                    if (!sumValue.HasValue)
                        sumValue = 0;

                    sumValue += decValue;
                }

                var oldAnswer = GetAnswerText(Current.Session, question.Identifier);
                var newAnswer = sumValue.ToString();

                if (oldAnswer != newAnswer)
                    SendCommand(new ChangeResponseAnswer(Current.Session.ResponseSessionIdentifier, question.Identifier, newAnswer));
            }

            return true;
        }

        private bool CheckUploadsValid(List<SurveyQuestion> questions)
        {
            var missingFiles = new List<string>();

            foreach (var question in questions)
            {
                var inputID = $"Question_{question.Sequence}";
                var control = ControlHelper.GetControl(AnswerGroupRepeater, inputID);

                if (!(control is FormFileUpload upload) || upload.UploadedFiles.Length == 0)
                    continue;

                var clearUploadedFiles = false;

                foreach (var uploadedFile in upload.UploadedFiles)
                {
                    if (uploadedFile == null || FileUploadV2.CanSaveFile(uploadedFile.FileIdentifier))
                        continue;

                    clearUploadedFiles = true;
                    missingFiles.Add(uploadedFile.FileName);
                }

                if (clearUploadedFiles)
                    upload.ClearUploadedFiles();
            }

            if (missingFiles.Count > 0)
            {
                const string defaultErrorMessage = "These uploaded files were not saved due to inactivity. Please re-upload and click the next button within 60 minutes, to ensure your response is saved.";

                var missingFilesText = "<ul>" + string.Join("", missingFiles.Select(x => $"<li>{x}</li>")) + "</ul>";
                var errorMessage = AppPage.GetDisplayHtml("Files Deleted Due to Inactivity", defaultErrorMessage);
                var combinedErrorMessage = errorMessage + missingFilesText;

                ErrorAlert.AddMessage(AlertType.Error, "fas fa-exclamation-square", combinedErrorMessage);
            }

            return missingFiles.Count == 0;
        }

        private void SendCommand(Command command)
        {
            var user = CurrentSessionState.Identity?.User?.Identifier;

            if (command.OriginUser == Guid.Empty)
                command.OriginUser = user ?? Current.Session.RespondentUserIdentifier;

            ServiceLocator.SendCommand(command);
        }

        private string UploadFile(UploadFileInfo[] uploadedFiles, string oldRelativeUrls)
        {
            if (uploadedFiles.Length == 0)
                return oldRelativeUrls;

            if (!string.IsNullOrEmpty(oldRelativeUrls))
            {
                var urls = StringHelper.Split(oldRelativeUrls);
                foreach (var url in urls)
                {
                    var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(url);
                    if (fileIdentifier == null)
                        continue;

                    ServiceLocator.StorageService.Delete(fileIdentifier.Value);
                }
            }

            var relativeUrls = new List<string>();

            foreach (var metadata in uploadedFiles)
            {
                var claimGroups = CurrentSessionState.Identity.Organization.Toolkits.Surveys?.ResponseUploadClaimGroups;

                var file = FileUploadV2.SaveFile(metadata, Current.Session.ResponseSessionIdentifier, FileObjectType.Response, null);
                CaseDocumentList.ApplyPermissionRule(file, claimGroups);

                var allowLearnerToView = CurrentSessionState.Identity.Organization.Toolkits.Issues?.DefaultCandidateUploadFileView ?? true;
                if (allowLearnerToView != file.Properties.AllowLearnerToView)
                {
                    var userId = CurrentSessionState.Identity.User?.Identifier ?? Shift.Constant.UserIdentifiers.Someone;
                    file.Properties.AllowLearnerToView = allowLearnerToView;

                    ServiceLocator.StorageService.ChangeProperties(file.FileIdentifier, userId, file.Properties, false);
                }

                var relativePath = ServiceLocator.StorageService.GetFileUrl(file.FileIdentifier, file.FileName);
                relativeUrls.Add(relativePath);
            }

            return CsvConverter.ListToStringList(relativeUrls, ";");
        }

        private string GetAnswerText(QResponseSession response, Guid question)
        {
            return response.QResponseAnswers
                .Where(x => x.SurveyQuestionIdentifier == question)
                .Select(x => x.ResponseAnswerText)
                .FirstOrDefault();
        }
    }
}