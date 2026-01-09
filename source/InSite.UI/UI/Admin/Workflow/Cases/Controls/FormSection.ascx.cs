using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;
using InSite.Persistence;
using InSite.UI.Portal.Workflow.Forms.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Issues.Outlines.Controls
{
    public partial class FormSection : BaseUserControl
    {
        private Guid IssueIdentifier
        {
            get => (Guid)ViewState[nameof(IssueIdentifier)];
            set => ViewState[nameof(IssueIdentifier)] = value;
        }

        private Guid ResponseSessionIdentifier
        {
            get => (Guid)ViewState[nameof(ResponseSessionIdentifier)];
            set => ViewState[nameof(ResponseSessionIdentifier)] = value;
        }

        protected SubmissionSessionState Current { get; set; }
        protected SubmissionSessionNavigator Navigator { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnswerGroupRepeater.ItemDataBound += AnswerGroupRepeater_ItemDataBound;
        }

        private void AnswerGroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var group = (AnswerGroup)e.Item.DataItem;
                var itemRepeater = (Repeater)e.Item.FindControl("AnswerItemRepeater");
                itemRepeater.ItemDataBound += AnswerItemRepeater_ItemDataBound;
                itemRepeater.DataSource = group.AnswerPackets;
                itemRepeater.DataBind();
            }
        }

        private void AnswerItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (AnswerItem)e.Item.DataItem;
                var ah = (ITextControl)e.Item.FindControl("AnswerHtml");
                ah.Text = CreateAnswerHtml(item);
            }
        }

        public void BindModelToControls(Guid issueIdentifier, Guid responseSessionIdentifier)
        {
            IssueIdentifier = issueIdentifier;
            ResponseSessionIdentifier = responseSessionIdentifier;

            Current = new SubmissionSessionState();
            Navigator = new SubmissionSessionNavigator();

            var query = ServiceLocator.SurveySearch.GetResponseSession(ResponseSessionIdentifier);
            if (query == null)
                return;

            Current.SessionIdentifier = query.ResponseSessionIdentifier;
            Current.FormIdentifier = query.SurveyFormIdentifier;
            Current.UserIdentifier = query.RespondentUserIdentifier;

            SubmissionSessionControl.LoadCurrentStateObjects(Current, Navigator);

            BindRespondentSection();
            BindSurveySection();
            BindResponseSection();

            BindAnswers();
        }

        private void BindRespondentSection()
        {
            var respondent = Current.Survey.EnableUserConfidentiality
                ? null
                : Current.Respondent;

            if (respondent != null)
            {
                RespondentName.Text = $"<a href='/ui/admin/contacts/people/edit?contact={respondent.UserIdentifier}'>{respondent.FullName}</a>";
                RespondentEmail.Text = $"<a href='mailto:{respondent.Email}'>{respondent.Email}</a>";
            }

            RespondentFields.Visible = respondent != null;
        }

        private void BindSurveySection()
        {
            var survey = Current.Survey;

            var surveySummary = new StringBuilder();

            surveySummary
                .Append("<div class='field-summary'><i class='far fa-check-square'></i> ")
                .Append(GetSurveyFormSize())
                .Append("</div>");

            if (!survey.ResponseLimitPerUser.HasValue && !survey.RequireUserIdentification)
                surveySummary
                    .Append("<div class='field-summary'><i class='far fa-user-alt'></i> Allow anonymous submissions</div>");

            if (survey.ResponseLimitPerUser == 1)
                surveySummary
                    .Append("<div class='field-summary'><i class='far fa-box-ballot'></i> Limit to one submission</div>");

            SurveyName.Text = $"<a href='/ui/admin/workflow/forms/outline?form={survey.Identifier}'>{survey.Name}</a>"
                + $" <div class='form-text'>Form #{survey.Asset}</div>"
                + " " + GetSurveyStatusHtml()
                + " " + GetSurveyInvitationText()
                + $" <div class='form-text' style='margin-top:6px;'>{GetSurveyLastModifiedHtml()}</div>";
            SurveyTitle.Text = survey.GetTitle().IfNullOrEmpty("None");
            SurveySummary.Text = surveySummary.ToString();
        }

        private void BindResponseSection()
        {
            var session = Current.Session;

            SubmissionState.Text = "<span style='font-size:16px; float:right;'>"
                + (session.ResponseSessionCompleted.HasValue ? " <span class='badge bg-success'>Completed</span>" : " <span class='badge bg-danger'>Not Completed</span>")
                + "</span>";

            ResponseStarted.Text = session.ResponseSessionStarted.Format(User.TimeZone, true, nullValue: "N/A");
            ResponseCompleted.Text = session.ResponseSessionCompleted.Format(User.TimeZone, true, nullValue: "N/A");
            ResponseTimeTaken.Text = session.ResponseSessionStarted.HasValue && session.ResponseSessionCompleted.HasValue ? (session.ResponseSessionCompleted.Value - session.ResponseSessionStarted.Value).Humanize() : "N/A";

            var lastRevisedDate = session.LastChangeTime.Format(User.TimeZone, true);
            var lastRevisedPerson = PersonSearch.Select(Organization.Identifier, session.LastChangeUser, x => x.User);

            LastRevisedBy.Text = lastRevisedPerson != null
                ? $"{lastRevisedPerson.User.FullName}, {lastRevisedDate}"
                : lastRevisedDate;

            if (session.GroupIdentifier.HasValue)
                ResponseGroup.Text = ServiceLocator.GroupSearch.GetGroup(session.GroupIdentifier.Value)?.GroupName ?? "-";

            if (session.PeriodIdentifier.HasValue)
                ResponsePeriod.Text = (ServiceLocator.PeriodSearch.GetPeriod(session.PeriodIdentifier.Value)?.PeriodName) ?? "-";

            ChangeResponseGroup.NavigateUrl =
            ChangeResponsePeriod.NavigateUrl = $"/ui/admin/workflow/forms/submissions/change?submission={session.ResponseSessionIdentifier}";
        }

        private void BindAnswers()
        {
            var questions = Current.Survey.Questions.Where(x => x.HasInput || x.Type == SurveyQuestionType.BreakQuestion).ToList();
            var answerGroups = GetAnswers(questions);
            AnswerGroupRepeater.DataSource = answerGroups;
            AnswerGroupRepeater.DataBind();
        }

        #region Helper Methods

        private string CreateAnswerHtml(AnswerItem item)
        {
            var html = new StringBuilder();

            if (item.AnswerInputType == SurveyQuestionType.BreakQuestion)
                return html.ToString();

            html.Append("<div class=\"row\"><div class='col-md-12'><ul>");

            foreach (var option in item.AnswerOptions)
                html.Append($"<li>{option}</li>");

            if (item.AnswerText.HasValue())
            {
                if (item.AnswerInputType == SurveyQuestionType.Upload)
                {
                    var urls = StringHelper.Split(item.AnswerText);
                    foreach (var url in urls)
                    {
                        string name = Path.GetFileName(url);
                        html.Append($"<li><a target='_blank' href='{url}'>{name}</a></li>");
                    }
                }
                else
                {
                    var answerTextHtml = !string.IsNullOrEmpty(item.AnswerText)
                        ? item.AnswerText.Replace("\n", "<br/>")
                        : item.AnswerText;

                    html.Append($"<li>{answerTextHtml}</li>");
                }
            }
            else if (item.AnswerOptions.Length == 0)
            {
                html.Append("<li>No Submission</li>");
            }

            html.Append("</ul></div></div>");
            return html.ToString();
        }

        private string GetSurveyFormSize()
        {
            var survey = Current.Survey;

            var pageCount = survey.CountPages();
            var questionCount = survey.Questions.Count;
            var optionListCount = survey.FlattenOptionLists().Length;
            var optionCount = survey.FlattenOptionItems().Length;
            var branchCount = survey.GetBranches().Count;
            var conditionCount = survey.GetConditions().Count;

            var sb = new StringBuilder();

            if (pageCount > 0)
                sb.Append("<span class='text-nowrap'>").Append("Page".ToQuantity(pageCount)).Append("</span>");

            if (questionCount > 0)
                sb.Append(", <span class='text-nowrap'>").Append("Question".ToQuantity(questionCount)).Append("</span>");

            if (optionListCount > 0)
                sb.Append(", <span class='text-nowrap'>").Append("Option List".ToQuantity(optionListCount)).Append("</span>");

            if (optionCount > 0)
                sb.Append(", <span class='text-nowrap'>").Append("Option".ToQuantity(optionCount)).Append("</span>");

            if (branchCount > 0)
                sb.Append(", <span class='text-nowrap'>").Append("Branch".ToQuantity(branchCount)).Append("</span>");

            if (conditionCount > 0)
                sb.Append(", <span class='text-nowrap'>").Append("Condition".ToQuantity(conditionCount)).Append("</span>");

            return sb.ToString();
        }

        private string GetSurveyStatusHtml()
        {
            var survey = Current.Survey;

            var sb = new StringBuilder();

            if (survey.Status == SurveyFormStatus.Closed)
            {
                sb.Append($"<span class='badge bg-primary'>Closed</span> ");
            }
            else if (survey.Status == SurveyFormStatus.Drafted)
            {
                sb.Append("<span class='badge bg-info'>Draft</span> ");
            }
            else if (survey.Status == SurveyFormStatus.Opened)
            {
                sb.Append($"<span class='badge bg-success'>Open</span> ");
            }
            else if (survey.Status == SurveyFormStatus.Archived)
            {
                sb.Append($"<span class='badge bg-secondary'>Archived</span> ");
            }

            return sb.ToString();
        }

        private string GetSurveyInvitationText()
        {
            var survey = Current.Survey;

            var sb = new StringBuilder();
            var count = (survey.Messages?.Count ?? 0);

            if (count == 0)
            {
                sb.Append("<span class='badge bg-warning'><i class='fas fa-exclamation'></i> Missing Form Invitation</span>");
            }
            else if (count > 1)
            {
                sb.Append("<span class='badge bg-danger'><i class='fas fa-exclamation'></i> Multiple Form Invitations</span>");
            }

            return sb.ToString();
        }

        private string GetSurveyLastModifiedHtml()
        {
            var survey = ServiceLocator.SurveySearch.GetSurveyForm(Current.Survey.Identifier);
            var subtitle = UserSearch.GetTimestampHtml(survey.LastChangeUser, survey.LastChangeType, null, survey.LastChangeTime);

            return $"<div class='last-modified'>{subtitle}</div>";
        }

        private AnswerGroup[] GetAnswers(List<SurveyQuestion> questions)
        {
            var answers = questions.Select(
                x => new AnswerItem
                {
                    Question = x.Identifier,
                    QuestionSequence = Navigator.GetQuestionNumber(x.Identifier),
                    QuestionBody = x.Content?.Title.GetHtml(Current.Language),
                    QuestionIsNested = x.IsNested,
                    QuestionIsHidden = !string.IsNullOrEmpty(x.LikertAnalysis),
                    AnswerInputType = x.Type,
                }
                ).ToList();

            foreach (var answer in answers)
            {
                var question = Current.Survey.FindQuestion(answer.Question);
                if (question != null)
                {
                    if (question.Code.HasValue())
                    {
                        var indicator = question.GetIndicatorStyleName();
                        answer.QuestionHeader = $"Question <span class='badge bg-{indicator}'>{question.Code}</span>";
                    }
                    else if (answer.AnswerInputType == SurveyQuestionType.BreakQuestion)
                    {
                        answer.QuestionHeader = $"{answer.QuestionBody}";
                        answer.QuestionBody = string.Empty;
                    }
                    else if (answer.QuestionSequence > 0)
                    {
                        answer.QuestionHeader = $"Question <span class='badge bg-primary'>{answer.QuestionSequence}</span>";
                    }
                    else
                    {
                        answer.IsQuestionHeaderVisible = false;
                    }

                    answer.AnswerText = GetAnswerText(Current.Session, question.Identifier);
                    answer.AnswerOptions = GetAnswerOptions(Current.Session, question.Identifier);
                }
            }

            var groups = CreateAnswerGroups(answers);

            return groups;
        }

        private AnswerGroup[] CreateAnswerGroups(List<AnswerItem> answers)
        {
            var groups = new List<AnswerGroup>();

            for (int i = 0; i < answers.Count; i++)
            {
                var packet = answers[i];

                if (i == 0)
                {
                    groups.Add(new AnswerGroup(packet));
                }
                else if (packet.QuestionIsNested)
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

        private string GetAnswerText(QResponseSession response, Guid question)
        {
            return response.QResponseAnswers
                .Where(x => x.SurveyQuestionIdentifier == question)
                .Select(x => x.ResponseAnswerText)
                .FirstOrDefault();
        }

        private string[] GetAnswerOptions(QResponseSession response, Guid question)
        {
            var options = response.QResponseOptions.Where(x => x.SurveyQuestionIdentifier == question).ToArray();
            if (options.IsEmpty())
                return new string[0];

            var list = new List<string>();

            foreach (var answerOption in options)
            {
                if (!answerOption.ResponseOptionIsSelected)
                    continue;

                var item = Current.Survey.FindOptionItem(answerOption.SurveyOptionIdentifier);
                if (item == null)
                    continue;

                var listTitle = item.List.Content?.Title.GetText(Current.Language);
                var itemTitle = item.Content.Title.GetText(Current.Language);
                list.Add($"<strong class='me-3'>{itemTitle}</strong> {listTitle}");
            }

            return list.ToArray();
        }

        #endregion
    }
}