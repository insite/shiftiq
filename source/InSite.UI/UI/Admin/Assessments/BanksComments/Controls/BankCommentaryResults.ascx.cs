using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Assessments.Reports.Controls
{
    public partial class BankCommentaryResults : SearchResultsGridViewController<BankCommentaryFilter>
    {
        #region Classes

        private class DataItem
        {
            public string EventFormat { get; set; }
            public Guid AuthorIdentifier { get; set; }
            public string AuthorName { get; set; }
            public string AuthorRole { get; set; }
            public string BankName { get; set; }
            public string BankTitle { get; set; }
            public string CommentFlag { get; set; }
            public Guid CommentIdentifier { get; set; }
            public DateTimeOffset CommentPosted { get; set; }
            public string CommentText { get; set; }
            public string FeedbackCategory { get; set; }
            public string ContainerSubtype { get; set; }
            public Guid BankIdentifier { get; set; }
            public Guid? FieldIdentifier { get; set; }
            public Guid? FormIdentifier { get; set; }
            public Guid? QuestionIdentifier { get; set; }
            public Guid? SpecificationIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }
        }

        private class ExportItem : DataItem
        {
            public string FormName { get; set; }
            public int? FormQuestionNumber { get; set; }
            public string Instructor { get; set; }
            public string QuestionAssetNumber { get; set; }
            public string Revisor { get; set; }
            public DateTimeOffset? Revised { get; set; }
            public string ExamEvent { get; set; }
        }

        #endregion

        #region Properties

        protected BankCommentaryCriteria.InnerFilter InnerFilter =>
            (BankCommentaryCriteria.InnerFilter)Filter;

        #endregion

        #region Fields

        private Dictionary<Guid, BankState> _banks = null;
        private ReturnUrl _returnUrl = new ReturnUrl();

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_ItemDataBound;
        }

        #endregion

        #region Event handlers

        protected override void GridDataBind()
        {
            _returnUrl = new ReturnUrl();

            base.GridDataBind();
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var data = (DataItem)e.Row.DataItem;
            var type = data.ContainerSubtype.ToEnum<CommentType>();

            var subject = (ITextControl)e.Row.FindControl("Subject");
            var editLink = (IconLink)e.Row.FindControl("EditLink");
            var entityLink = (IconLink)e.Row.FindControl("EntityLink");

            if (_banks.TryGetValue(data.BankIdentifier, out var bank))
            {
                if (type == CommentType.Bank)
                {
                    if (bank.Identifier == data.BankIdentifier)
                        subject.Text = $"Bank: {bank.Name} <span class='form-text'>Asset #{bank.Asset}</span>";
                }
                else if (type == CommentType.Question)
                {
                    var question = bank.FindQuestion(data.QuestionIdentifier.Value);
                    if (question != null)
                    {
                        subject.Text = $"Question #{question.BankIndex + 1}:"
                            + $" <span style='white-space:pre-wrap;'>{Markdown.ToText(question.Content.Title?.Default)}</span>"
                            + $" <span class='form-text'>Asset #{question.Asset}</span>"
                            + $"<div class='form-text'>{question.Set.Bank.Name} / {question.Set.Name}</div>";

                        entityLink.Visible = true;
                        entityLink.ToolTip = "Jump to Question";
                        entityLink.NavigateUrl = _returnUrl
                            .GetRedirectUrl($"/ui/admin/assessments/banks/outline?bank={data.BankIdentifier}&question={question.Identifier}");
                    }
                }
                else if (type == CommentType.Specification)
                {
                    var spec = bank.FindSpecification(data.SpecificationIdentifier.Value);
                    if (spec != null)
                        subject.Text = $"Specification: {spec.Name} <span class='form-text'>Asset #{spec.Asset}</span>"
                            + $"<div class='form-text'>{spec.Bank.Name}</div>";
                }
                else if (type == CommentType.Form)
                {
                    var form = bank.FindForm(data.FormIdentifier.Value);
                    if (form != null)
                        subject.Text = $"Form: {form.Name} <span class='form-text'>Asset #{form.Asset}</span>"
                            + $"<div class='form-text'>{form.Specification.Bank.Name} / {form.Specification.Name}</div>";
                }
                else if (type == CommentType.Field)
                {
                    var field = bank.FindField(data.FieldIdentifier.Value);
                    if (field != null)
                    {
                        subject.Text = $"Question #{field.FormSequence}:"
                            + $" <span style='white-space:pre-wrap;'>{Markdown.ToText(field.Question.Content.Title?.Default)}</span>"
                            + $" <span class='form-text'>Asset #{field.Question.Asset}</span>"
                            + $"<div class='form-text'>{field.Section.Form.Specification.Bank.Name} / {field.Section.Form.Name}</div>";

                        entityLink.Visible = true;
                        entityLink.ToolTip = "Jump to Question";
                        entityLink.NavigateUrl = _returnUrl
                            .GetRedirectUrl($"/ui/admin/assessments/banks/outline?bank={data.BankIdentifier}&question={field.QuestionIdentifier}");
                    }
                }
            }

            editLink.Visible = true;
            editLink.NavigateUrl = _returnUrl
                .GetRedirectUrl($"/ui/admin/assessments/comments/revise?bank={data.BankIdentifier}&comment={data.CommentIdentifier}");
        }

        #endregion

        protected override int SelectCount(BankCommentaryFilter filter)
        {
            return ServiceLocator.BankSearch.CountComments(filter);
        }

        protected override IListSource SelectData(BankCommentaryFilter filter)
        {
            _banks = new Dictionary<Guid, BankState>();

            var comments = ServiceLocator.BankSearch.BindComments(x => new DataItem
            {
                EventFormat = x.EventFormat,
                AuthorIdentifier = x.AuthorUserIdentifier,
                AuthorName = x.AuthorUserName,
                AuthorRole = x.AuthorUserRole,
                BankName = x.BankName,
                BankTitle = x.BankTitle,
                CommentFlag = x.CommentFlag,
                CommentIdentifier = x.CommentIdentifier,
                CommentPosted = x.CommentPosted,
                CommentText = x.CommentText,
                FeedbackCategory = x.CommentCategory,
                ContainerSubtype = x.ContainerSubtype,
                BankIdentifier = x.AssessmentBankIdentifier.Value,
                FieldIdentifier = x.AssessmentFieldIdentifier.Value,
                FormIdentifier = x.AssessmentFormIdentifier.Value,
                QuestionIdentifier = x.AssessmentQuestionIdentifier.Value,
                SpecificationIdentifier = x.AssessmentSpecificationIdentifier.Value,
                OrganizationIdentifier = x.OrganizationIdentifier,
            }, filter);

            foreach (var c in comments)
                if (!_banks.ContainsKey(c.BankIdentifier))
                    _banks.Add(c.BankIdentifier, null);

            if (_banks.Count > 0)
            {
                var bankData = ServiceLocator.BankSearch.GetBankStates(_banks.Keys);
                foreach (var b in bankData)
                    _banks[b.Identifier] = b;
            }

            return comments.ToSearchResult();
        }

        public override IListSource GetExportData(BankCommentaryFilter filter, bool empty)
        {
            var result = new List<ExportItem>();

            if (empty)
                return result.ToSearchResult();

            var banks = new Dictionary<Guid, BankState>();
            var dataItems = ServiceLocator.BankSearch.BindComments(
                x => new ExportItem
                {
                    EventFormat = x.EventFormat,
                    AuthorIdentifier = x.AuthorUserIdentifier,
                    AuthorName = x.AuthorUserName,
                    AuthorRole = x.AuthorUserRole,
                    BankName = x.BankName,
                    BankTitle = x.BankTitle,
                    CommentFlag = x.CommentFlag,
                    CommentIdentifier = x.CommentIdentifier,
                    CommentPosted = x.CommentPosted,
                    CommentText = x.CommentText,
                    FeedbackCategory = x.CommentCategory,
                    ContainerSubtype = x.ContainerSubtype,
                    BankIdentifier = x.AssessmentBankIdentifier.Value,
                    FieldIdentifier = x.AssessmentFieldIdentifier.Value,
                    FormIdentifier = x.AssessmentFormIdentifier.Value,
                    QuestionIdentifier = x.AssessmentQuestionIdentifier.Value,
                    SpecificationIdentifier = x.AssessmentSpecificationIdentifier.Value,
                    OrganizationIdentifier = x.OrganizationIdentifier,

                    Instructor = x.TrainingProviderGroupName,
                    Revisor = x.RevisorUserName,
                    Revised = x.CommentRevised,
                }, filter);

            foreach (var dataItem in dataItems)
            {
                var type = dataItem.ContainerSubtype.ToEnum<CommentType>();

                if (type == CommentType.Question || type == CommentType.Field)
                {
                    if (!banks.TryGetValue(dataItem.BankIdentifier, out var bank))
                        banks.Add(dataItem.BankIdentifier, bank = ServiceLocator.BankSearch.GetBankState(dataItem.BankIdentifier));

                    if (bank != null)
                    {
                        if (type == CommentType.Question)
                        {
                            var question = bank.FindQuestion(dataItem.QuestionIdentifier.Value);
                            if (question != null)
                                dataItem.QuestionAssetNumber = $"{question.Asset}.{question.AssetVersion}";

                            if (dataItem.FormIdentifier.HasValue && dataItem.FieldIdentifier.HasValue)
                            {
                                var form = bank.FindForm(dataItem.FormIdentifier.Value);

                                if (form != null)
                                {
                                    dataItem.FormName = form.Name;

                                    var field = form.Sections.SelectMany(x => x.Fields)
                                        .Where(x => x.QuestionIdentifier == dataItem.FieldIdentifier.Value)
                                        .FirstOrDefault();
                                    if (field != null)
                                        dataItem.FormQuestionNumber = field.FormSequence;
                                }
                            }
                        }
                        else if (type == CommentType.Field)
                        {
                            var field = bank.FindField(dataItem.FieldIdentifier.Value);
                            if (field != null)
                            {
                                dataItem.FormName = field.Section.Form.Name;
                                dataItem.FormQuestionNumber = field.FormSequence;
                                dataItem.QuestionAssetNumber = $"{field.Question.Asset}.{field.Question.AssetVersion}";
                            }
                        }
                    }
                }

                result.Add(dataItem);
            }

            return result.ToSearchResult();
        }

        #region Methods (helpers)

        protected static string GetFlagDescription(string value)
        {
            var flag = value.ToEnumNullable<FlagType>();
            return flag.HasValue ? $"{flag.Value.ToIconHtml()} {flag.Value.GetDescription()}" : string.Empty;
        }

        #endregion
    }
}