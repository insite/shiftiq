using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assessments.Attachments.Forms
{
    public partial class Usage : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class QuestionGroupInfo
        {
            public string Title { get; set; }
            public string Icon { get; set; }
            public int Count { get; set; }
            public Question[] Questions { get; set; }
        }

        #endregion

        #region Properties

        private Guid? BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : (Guid?)null;

        private Guid? AttachmentID => Guid.TryParse(Request.QueryString["attachment"], out var value) ? value : (Guid?)null;

        private bool ShowAllVersions => Request.QueryString["version"] == "all";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionGroupRepeater.ItemDataBound += QuestionGroupRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Open())
                RedirectToSearch();

            CancelButton.NavigateUrl = GetParentUrl();
        }

        #endregion

        #region Event handlers

        private void QuestionGroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (QuestionGroupInfo)e.Item.DataItem;

            var repeater = (QuestionRepeater)e.Item.FindControl("QuestionRepeater");
            repeater.LoadData(dataItem.Questions);
        }

        #endregion

        #region Database operations

        private bool Open()
        {
            if (!BankID.HasValue || !AttachmentID.HasValue)
                return false;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID.Value);
            if (bank == null)
                return false;

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            var attachment = bank.FindAttachment(AttachmentID.Value);
            if (attachment == null)
                RedirectToParent();

            var attachments = ShowAllVersions
                ? attachment.EnumerateAllVersions().ToArray()
                : new[] { attachment };
            var hasQuestions = attachments.Any(x => x.QuestionIdentifiers.Count > 0);

            if (hasQuestions)
            {
                var allQuestionIds = attachments.SelectMany(x => x.QuestionIdentifiers).ToHashSet();
                var unassignedIds = allQuestionIds.ToHashSet();
                var questionGroups = new List<QuestionGroupInfo>();

                foreach (var spec in bank.Specifications)
                {
                    foreach (var form in spec.EnumerateAllForms())
                    {
                        var questions = form.Sections.SelectMany(x => x.Fields)
                            .Where(x => allQuestionIds.Contains(x.QuestionIdentifier))
                            .Select(x =>
                            {
                                unassignedIds.Remove(x.Question.Identifier);
                                return x.Question;
                            })
                            .ToArray();

                        if (questions.Length == 0)
                            continue;

                        questionGroups.Add(new QuestionGroupInfo
                        {
                            Title = form.Name,
                            Icon = "window",
                            Count = questions.Length,
                            Questions = questions
                        });
                    }
                }

                if (unassignedIds.Count > 0)
                {
                    var unassignedQuestions = new List<Question>();

                    foreach (var question in bank.Sets.SelectMany(x => x.EnumerateAllQuestions()))
                    {
                        if (!unassignedIds.Contains(question.Identifier))
                            continue;

                        unassignedQuestions.Add(question);
                        unassignedIds.Remove(question.Identifier);

                        if (unassignedIds.Count == 0)
                            break;
                    }

                    if (unassignedQuestions.Count > 0)
                    {
                        questionGroups.Insert(0, new QuestionGroupInfo
                        {
                            Title = "Unassigned",
                            Icon = "question-circle",
                            Count = unassignedQuestions.Count,
                            Questions = unassignedQuestions.ToArray()
                        });
                    }
                }

                QuestionGroupRepeater.DataSource = questionGroups;
                QuestionGroupRepeater.DataBind();
            }

            UsageHistory.LoadData(bank, AttachmentID.Value, ShowAllVersions);
            UsageHistoryCount.InnerText = UsageHistory.RowCount.ToString("n0");

            if (!hasQuestions && UsageHistory.RowCount == 0)
                RedirectToParent();

            return true;
        }

        #endregion

        #region Helper methods

        private void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToParent()
        {
            var url = GetParentUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetParentUrl()
        {
            var url = new ReturnUrl().GetReturnUrl();

            if (url == null)
            {
                url = $"/ui/admin/assessments/banks/outline?bank={BankID}";
                if (AttachmentID.HasValue)
                    url += $"&attachment={AttachmentID}";
            }

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}