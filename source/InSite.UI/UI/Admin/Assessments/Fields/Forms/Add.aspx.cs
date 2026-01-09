using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using Section = InSite.Domain.Banks.Section;

namespace InSite.Admin.Assessments.Fields.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        [Serializable]
        private class QuestionRepeaterDataItem
        {
            public int? FieldIndex { get; set; }
            public int BankSequence { get; set; }
            public int SetIndex { get; set; }
            public Guid Identifier { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }
            public string Condition { get; set; }
            public string Flag { get; set; }
            public string Standard { get; set; }
            public bool Checked { get; set; }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID
        {
            get => (Guid)ViewState[nameof(FormID)];
            set => ViewState[nameof(FormID)] = value;
        }

        private Guid SectionID => Guid.Parse(Request["section"]);

        private QuestionRepeaterDataItem[] QuestionRepeaterDataSource
        {
            get => (QuestionRepeaterDataItem[])ViewState[nameof(QuestionRepeaterDataSource)];
            set => ViewState[nameof(QuestionRepeaterDataSource)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            QuestionIdValidator.ServerValidate += QuestionIdValidator_ServerValidate;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanCreate)
                RedirectToFinder();

            if (!IsPostBack)
            {
                Open();
                return;
            }

            if (QuestionRepeaterDataSource == null)
                return;

            var originalItems = QuestionRepeaterDataSource;
            var updatedItems = new QuestionRepeaterDataItem[originalItems.Length];

            var isSequenceChanged = false;
            var itemsSequence = GetQuestionsSequence();

            for (var i = 0; i < QuestionRepeater.Items.Count; i++)
            {
                var dataItem = originalItems[i];

                if (!dataItem.FieldIndex.HasValue)
                {
                    var repeaterItem = QuestionRepeater.Items[i];
                    var isSelected = (ICheckBoxControl)repeaterItem.FindControl("IsSelected");

                    dataItem.Checked = isSelected.Checked;
                }

                var index = i;

                if (itemsSequence != null)
                {
                    index = itemsSequence[i];
                    if (i != index)
                        isSequenceChanged = true;
                }

                updatedItems[i] = originalItems[index];
            }

            if (isSequenceChanged)
            {
                QuestionRepeaterDataSource = updatedItems;

                QuestionRepeater.DataSource = QuestionRepeaterDataSource;
                QuestionRepeater.DataBind();
            }
        }

        #endregion

        #region Event handlers

        private void QuestionIdValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = QuestionRepeaterDataSource != null && QuestionRepeaterDataSource.Any(x => !x.FieldIndex.HasValue && x.Checked);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToFinder();

            var section = bank.FindSection(SectionID);
            if (section == null || section.Form.Specification.Type != SpecificationType.Static)
                RedirectToBankReader();

            SetInputValues(section);
        }

        private void Save()
        {
            for (var i = 0; i < QuestionRepeaterDataSource.Length; i++)
            {
                var item = QuestionRepeaterDataSource[i];
                if (!item.FieldIndex.HasValue && item.Checked)
                    ServiceLocator.SendCommand(new AddField(BankID, UniqueIdentifier.Create(), SectionID, item.Identifier, i));
            }

            RedirectToBankReader(FormID, SectionID, Guid.Empty);
        }

        #endregion

        #region Settin/getting input values

        private void SetInputValues(Section section)
        {
            var form = section.Form;
            var bank = form.Specification.Bank;

            PageHelper.AutoBindHeader(this, null,
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            FormID = form.Identifier;

            SpecificationField.Visible = bank.IsAdvanced;
            SpecificationName.Text = form.Specification.Name;

            SetField.Visible = bank.IsAdvanced;
            SetName.Text = section.Criterion.Title;

            FormName.Text = form.Name;
            SectionName.Text = $"Section {section.Sequence} of {form.Sections.Count}";

            BindQuestionRepeater(section);

            CancelButton.NavigateUrl = GetReaderUrl(FormID, section.Identifier);
        }

        private void BindQuestionRepeater(Section section)
        {
            var hasQuestions = section.Criterion.Sets.SelectMany(x => x.Questions).Count() > 0;
            QuestionsContainer.Visible = hasQuestions;
            NoQuestionsMessage.Visible = !hasQuestions;

            if (!hasQuestions)
                return;

            var fieldsIndex = new Dictionary<Guid, int>();
            for (var i = 0; i < section.Fields.Count; i++)
                fieldsIndex.Add(section.Fields[i].QuestionIdentifier, i);

            var first = section.Criterion.Sets.First();
            var standardFilter = first.Questions.Select(x => x.Standard).Distinct().ToArray();
            var standards = StandardSearch.Bind(x => new
            {
                x.StandardIdentifier,
                ParentCode = x.Parent.Code,
                x.Code
            }, x => standardFilter.Contains(x.StandardIdentifier)).ToDictionary(x => x.StandardIdentifier, x => x);

            QuestionRepeaterDataSource = first.Questions.Select((x, i) =>
            {
                var standard = standards.ContainsKey(x.Standard) ? standards[x.Standard] : null;
                var fieldIndex = fieldsIndex.ContainsKey(x.Identifier) ? (int?)fieldsIndex[x.Identifier] : null;

                return new QuestionRepeaterDataItem
                {
                    FieldIndex = fieldIndex,
                    BankSequence = x.BankIndex + 1,
                    SetIndex = i,
                    Identifier = x.Identifier,
                    Title = (x.Content.Title?.Default).IfNullOrEmpty("(Untitled)"),
                    Code = x.Classification.Code,
                    Condition = x.Condition,
                    Flag = x.Flag.ToIconHtml(),
                    Standard = standard == null ? null : $"{standard.ParentCode} {standard.Code}",
                    Checked = fieldIndex != null
                };
            }).OrderBy(x => !x.FieldIndex.HasValue).ThenBy(x => x.FieldIndex).ThenBy(x => x.SetIndex).ToArray();

            QuestionRepeater.DataSource = QuestionRepeaterDataSource;
            QuestionRepeater.DataBind();
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToBankReader(Guid? formId = null, Guid? sectionId = null, Guid? fieldId = null)
        {
            var url = GetReaderUrl(formId, sectionId, fieldId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null, Guid? sectionId = null, Guid? fieldId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            if (sectionId.HasValue)
                url += $"&section={sectionId.Value}";

            if (fieldId.HasValue)
                url += "&tab=fields";
            else
                url += "&tab=section";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion

        #region Helper methods

        protected string GetConditionHtml(object status)
        {
            if (status == null)
                return string.Empty;

            return $"<span class='badge bg-custom-default'>{status}</span>";
        }

        private int[] GetQuestionsSequence()
        {
            if (string.IsNullOrEmpty(QuestionsSequence.Value))
                return null;

            var strValues = QuestionsSequence.Value.Split(',');
            if (strValues.Length != QuestionRepeaterDataSource.Length)
                throw new ApplicationError("Invalid questions sequence length: " + QuestionsSequence.Value);

            var items = new HashSet<int>();
            var result = new int[strValues.Length];

            for (var i = 0; i < result.Length; i++)
            {
                var value = int.Parse(strValues[i]);
                if (value < 0 || value >= result.Length)
                    throw new ApplicationError($"Question index out of range: Value={value}, Length={result.Length}, Input={QuestionsSequence.Value}");

                if (items.Contains(value))
                    throw new ApplicationError($"Question index duplicate found: Value={value}, Input={QuestionsSequence.Value}");

                items.Add(result[i] = value);
            }

            return result;
        }

        #endregion
    }
}
