using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Sdk.UI;

using Literal = System.Web.UI.WebControls.Literal;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class GradeItemList : BaseUserControl
    {
        [Serializable]
        private class LikertRow
        {
            public Guid LikertRowId { get; set; }
            public Guid? GradeItemId { get; set; }
            public string QuestionText { get; set; }
            public string GradeItemCode { get; set; }
            public string GradeItemName { get; set; }
            public int LikertRowIndex { get; set; }
        }

        [Serializable]
        private class FormRecord
        {
            public Guid GradebookId { get; set; }
            public Guid? GradeItemId { get; set; }
            public Guid FormId { get; set; }
            public string FormName { get; set; }
            public int FormSequence { get; set; }
            public LikertRow[] LikertRows { get; set; }
        }

        private Guid BankId
        {
            get => (Guid)ViewState[nameof(BankId)];
            set => ViewState[nameof(BankId)] = value;
        }

        private Guid QuestionId
        {
            get => (Guid)ViewState[nameof(QuestionId)];
            set => ViewState[nameof(QuestionId)] = value;
        }

        private List<FormRecord> Forms
        {
            get => (List<FormRecord>)ViewState[nameof(Forms)];
            set => ViewState[nameof(Forms)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            List.ItemCreated += List_ItemCreated;
        }

        public bool BindModelToControls(Question question)
        {
            var bank = question.Set.Bank;

            QuestionId = question.Identifier;
            BankId = bank.Identifier;

            Forms = bank.Specifications
                .SelectMany(spec =>
                    spec.Forms
                        .Where(form => form.Gradebook.HasValue && form.ContainsQuestion(question.Identifier))
                        .Select(form => new FormRecord
                        {
                            GradebookId = form.Gradebook.Value,
                            GradeItemId = question.GradeItems.TryGetValue(form.Identifier, out var gradeItemId) ? gradeItemId : (Guid?)null,
                            FormId = form.Identifier,
                            FormName = form.Name,
                            FormSequence = form.Sequence,
                            LikertRows = GetLikertRows(form.Identifier, question)
                        })
                )
                .OrderBy(x => x.FormSequence)
                .ToList();

            if (Forms.Count == 0)
                return false;

            List.ItemDataBound += List_ItemDataBound;
            List.DataSource = Forms;
            List.DataBind();

            return true;
        }

        public List<(Guid FormId, Guid? LikertRowId, Guid? GradeItemId)> BindControlsToModel()
        {
            var result = new List<(Guid, Guid?, Guid?)>();

            for (int i = 0; i < List.Items.Count; i++)
            {
                var item = List.Items[i];
                var formId = Guid.Parse(((Literal)item.FindControl("FormId")).Text);
                var gradeItemId = ((GradebookItemComboBox)item.FindControl("GradeItem")).ValueAsGuid;
                var likertRows = GetLikertRowsFromItemChildren(formId, gradeItemId);

                result.Add((formId, null, gradeItemId));

                if (likertRows == null)
                    continue;

                foreach (var likertRow in likertRows)
                    result.Add((formId, likertRow.LikertRowId, likertRow.GradeItemId));
            }

            return result;
        }

        private void List_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var formRecord = (FormRecord)e.Item.DataItem;

            var gradeItem = (GradebookItemComboBox)e.Item.FindControl("GradeItem");
            gradeItem.GradebookIdentifier = formRecord.GradebookId;
            gradeItem.ValueAsGuid = formRecord.GradeItemId;

            var likertRowRepeater = (Repeater)e.Item.FindControl("LikertRowRepeater");
            likertRowRepeater.Visible = formRecord.GradeItemId.HasValue;
            likertRowRepeater.DataSource = formRecord.LikertRows;
            likertRowRepeater.DataBind();
        }

        private void List_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var gradeItemComboBox = (GradebookItemComboBox)e.Item.FindControl("GradeItem");
            gradeItemComboBox.AutoPostBack = true;
            gradeItemComboBox.ValueChanged += GradeItemComboBox_ValueChanged;
        }

        private void GradeItemComboBox_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            var comboBox = (GradebookItemComboBox)sender;
            var item = (RepeaterItem)comboBox.NamingContainer;
            var formRecord = Forms[item.ItemIndex];

            var likertRows = comboBox.ValueAsGuid.HasValue
                ? GetLikertRowsFromItemChildren(formRecord.FormId, comboBox.ValueAsGuid.Value)
                : null;

            var likertRowRepeater = (Repeater)item.FindControl("LikertRowRepeater");
            likertRowRepeater.Visible = comboBox.ValueAsGuid.HasValue;
            likertRowRepeater.DataSource = likertRows;
            likertRowRepeater.DataBind();
        }

        private LikertRow[] GetLikertRows(Guid formId, Question question)
        {
            if (question.Likert == null || !question.Likert.Rows.Any())
                return null;

            var likertRows = question.Likert.Rows
                .Select(x => new LikertRow
                {
                    LikertRowId = x.Identifier,
                    GradeItemId = x.GradeItems.TryGetValue(formId, out var gradeItemId) ? gradeItemId : (Guid?)null,
                    QuestionText = x.Content.Title.Default.MaxLength(50, true),
                    LikertRowIndex = x.Index
                })
                .OrderBy(x => x.LikertRowIndex)
                .ToArray();

            foreach (var likertRow in likertRows)
            {
                var gradeItem = likertRow.GradeItemId.HasValue
                    ? ServiceLocator.RecordSearch.GetGradeItem(likertRow.GradeItemId.Value)
                    : null;

                likertRow.GradeItemCode = gradeItem?.GradeItemCode;
                likertRow.GradeItemName = gradeItem?.GradeItemName ?? "N/A";
            }

            return likertRows;
        }

        private LikertRow[] GetLikertRowsFromItemChildren(Guid formId, Guid? parentGradeItemId)
        {
            var existing = Forms.Find(x => x.FormId == formId);
            if (existing.GradeItemId == parentGradeItemId)
                return existing.LikertRows;

            var bank = ServiceLocator.BankSearch.GetBankState(BankId);
            var form = bank.FindForm(formId);
            if (form.Gradebook == null)
                return null;

            var childItems = parentGradeItemId.HasValue
                ? ServiceLocator.RecordSearch
                    .GetGradeItems(new QGradeItemFilter
                    {
                        GradebookIdentifier = form.Gradebook,
                        ParentGradeItemIdentifier = parentGradeItemId
                    })
                    .OrderBy(x => x.GradeItemSequence)
                    .ToList()
                : null;

            var question = bank.FindQuestion(QuestionId);

            if (question.Likert == null || !question.Likert.Rows.Any())
                return null;

            var likertRows = question.Likert.Rows
                .Select(x => new LikertRow
                {
                    LikertRowId = x.Identifier,
                    QuestionText = x.Content.Title.Default.MaxLength(50, true),
                    LikertRowIndex = x.Index
                })
                .OrderBy(x => x.LikertRowIndex)
                .ToArray();

            for (int i = 0; i < likertRows.Length; i++)
            {
                var likertRow = likertRows[i];
                var gradeItem = childItems != null && i < childItems.Count ? childItems[i] : null;

                likertRow.GradeItemId = gradeItem?.GradeItemIdentifier;
                likertRow.GradeItemCode = gradeItem?.GradeItemCode;
                likertRow.GradeItemName = gradeItem?.GradeItemName ?? "N/A";
            }

            return likertRows;
        }
    }
}