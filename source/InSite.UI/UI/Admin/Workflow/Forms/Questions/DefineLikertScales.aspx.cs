using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Workflow.Forms.Questions.Models;
using InSite.Application.Surveys.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Workflow.Forms.Questions
{
    public partial class DefineLikertScales : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/workflow/forms/search";

        private DefineLikertScalesState Current { get; set; }
        private DefineLikertScalesNavigator LikertNavigator { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ScaleDtoRepeater.ItemDataBound += ScaleDtoRepeater_ItemDataBound;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            LoadCurrentState();

            if (!Page.IsPostBack)
                BindControls();
        }

        private void LoadCurrentState()
        {
            var qs = new DefineLikertScalesQueryString(Page.Request.QueryString);
            Current = new DefineLikertScalesState();
            Current.Survey = ServiceLocator.SurveySearch.GetSurveyState(qs.Survey);
            Current.Question = Current.Survey?.Form.FindQuestion(qs.Question);
            LikertNavigator = new DefineLikertScalesNavigator(qs.Survey, qs.Question);
        }

        private void BindControls()
        {
            var survey = Current.Survey;
            var question = Current.Question;

            if (question == null || survey.Form.Tenant != Organization.OrganizationIdentifier || survey.Form.Locked.HasValue)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, qualifier: survey.Form.Name);

            QuestionText.Text = Markdown.ToHtml(question.Content?.Title?.Text.Default ?? "(Question)");

            var columnData = CreateDataSourceForColumnDtoRepeater(question.Options.Lists.FirstOrDefault());
            var hasColumnData = columnData.Length > 0;

            NoDataMessage.Visible = !hasColumnData;

            ColumnDtoRepeater.Visible = hasColumnData;
            ColumnDtoRepeater.DataSource = columnData;
            ColumnDtoRepeater.DataBind();

            ScaleDtoRepeater.DataSource = CreateDataSourceForScaleDtoRepeater(question);
            ScaleDtoRepeater.DataBind();

            CancelButton.NavigateUrl = LikertNavigator.GetOutlineUrl();
        }

        private ColumnDto[] CreateDataSourceForColumnDtoRepeater(SurveyOptionList list)
        {
            return list?.Items.Select(item => new ColumnDto
            {
                Title = (item.Content?.Title?.GetText()).IfNullOrEmpty("-"),
                Points = item.Points,
                Category = item.Category,
            }).ToArray() ?? new ColumnDto[0];
        }

        private List<ScaleDto> CreateDataSourceForScaleDtoRepeater(SurveyQuestion question)
        {
            return question.Scales
                .Select(item => new ScaleDto
                {
                    Category = item.Category,
                    Rows = question.Options.Lists
                        .Where(x => x.Category == item.Category)
                        .Select(row => row.Content.Title?.GetText())
                        .ToList()
                })
                .ToList();
        }

        private void ScaleDtoRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var category = ((ScaleDto)e.Item.DataItem).Category;
            var scale = Current.Question.Scales.FirstOrDefault(x => x.Category == category);
            if (scale == null)
                return;

            SetSurveyScaleItem("A", 0);
            SetSurveyScaleItem("B", 1);
            SetSurveyScaleItem("C", 2);
            SetSurveyScaleItem("D", 3);
            SetSurveyScaleItem("E", 4);
            SetSurveyScaleItem("F", 5);

            void SetSurveyScaleItem(string letter, int index)
            {
                var i = e.Item;

                var minimum = (NumericBox)i.FindControl("Minimum" + letter);
                var maximum = (NumericBox)i.FindControl("Maximum" + letter);
                var grade = (ITextBox)i.FindControl("Grade" + letter);
                var feedback = (ITextBox)i.FindControl("Feedback" + letter);
                var calculation = (ComboBox)i.FindControl("Calculation" + letter);

                if (index >= scale.Items.Count)
                    return;

                var item = scale.Items[index];
                minimum.ValueAsDecimal = item.Minimum;
                maximum.ValueAsDecimal = item.Maximum;
                grade.Text = item.Grade;
                feedback.Text = item.Content.Description.Text.Default;
                if (!string.IsNullOrEmpty(item.Calculation))
                {
                    var box = calculation.FindOptionByValue(item.Calculation);
                    if (box != null)
                        box.Selected = true;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Current.Question.Scales.Count; i++)
            {
                var existing = Current.Question.Scales[i];
                var scale = new SurveyScale
                {
                    Category = existing.Category,
                    Content = existing.Content?.Clone() ?? new ContentContainer()
                };

                AddScaleItem(scale, existing, "A", 0, i);
                AddScaleItem(scale, existing, "B", 1, i);
                AddScaleItem(scale, existing, "C", 2, i);
                AddScaleItem(scale, existing, "D", 3, i);
                AddScaleItem(scale, existing, "E", 4, i);
                AddScaleItem(scale, existing, "F", 5, i);

                scale.SyncDefaultContent();

                ServiceLocator.SendCommand(new ChangeSurveyScale(Current.Survey.Form.Identifier, Current.Question.Identifier, scale));
            }

            LikertNavigator.RedirectToOutline();
        }

        private void AddScaleItem(SurveyScale scale, SurveyScale existing, string letter, int itemIndex, int index)
        {
            var repeaterItem = ScaleDtoRepeater.Items[index];
            var minimum = (NumericBox)repeaterItem.FindControl("Minimum" + letter);
            var maximum = (NumericBox)repeaterItem.FindControl("Maximum" + letter);
            var grade = (ITextBox)repeaterItem.FindControl("Grade" + letter);
            var feedback = (ITextBox)repeaterItem.FindControl("Feedback" + letter);
            var calculation = (ComboBox)repeaterItem.FindControl("Calculation" + letter);

            var hasValue = minimum.ValueAsDecimal.HasValue
                && maximum.ValueAsDecimal.HasValue
                && grade.Text.HasValue()
                && feedback.Text.HasValue();

            if (!hasValue)
                return;

            var item = new SurveyScaleItem
            {
                Minimum = minimum.ValueAsDecimal.Value,
                Maximum = maximum.ValueAsDecimal.Value,
                Grade = grade.Text,
                Calculation = calculation.Value
            };

            if (itemIndex < existing.Items.Count)
                item.Content = existing.Items[itemIndex].Content?.Clone();

            if (item.Content == null)
                item.Content = new ContentContainer();

            item.Content.Description.Text.Default = StringHelper.BreakHtml(feedback.Text);

            scale.Items.Add(item);
        }

        public string GetParentLinkParameters(IWebRoute parent)
            => LikertNavigator.GetParentLinkParameters(parent.Name);
    }
}
