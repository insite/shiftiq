using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Admin.Assessments.Options.Controls
{
    public partial class SingleCorrectOptionRepeater : OptionWriteRepeater
    {
        #region Classes

        [Serializable]
        private class OptionCollection : ItemCollection
        {
            public OptionCollection(Guid questionId)
                : base(questionId)
            {

            }
        }

        [Serializable]
        private class OptionItem : ItemCollection.DataItem
        {
            public decimal Points { get; set; }
            public decimal? CutScore { get; set; }
            public bool? IsTrue { get; set; }
            public Guid? Competency { get; set; }

            public OptionItem()
                : base()
            {

            }

            public OptionItem(Option option)
                : base(option.Number)
            {
                Text.Set(option.Content.Title);
                Points = option.Points;
                CutScore = option.CutScore;
                IsTrue = option.IsTrue;
                Competency = option.Standard.NullIfEmpty();
            }

            public override Option GetOption()
            {
                var option = base.GetOption();
                option.Points = Points;
                option.CutScore = CutScore;
                option.IsTrue = IsTrue;
                option.Standard = Competency ?? Guid.Empty;
                return option;
            }
        }

        #endregion

        #region Properties

        private Guid? FrameworkIdentifier
        {
            get { return (Guid?)ViewState[nameof(FrameworkIdentifier)]; }
            set { ViewState[nameof(FrameworkIdentifier)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddOptionButton.Click += AddOptionButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            AddOptionButton.Visible = !IsReadOnly;

            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(SingleCorrectOptionRepeater),
                "init_reorder",
                $"optionWriteRepeater.initReorder('{ClientID}','{ItemsOrder.ClientID}');",
                true);
        }

        protected override ItemCollection CreateItemCollection(Guid questionId)
        {
            if (questionId == Guid.Empty)
                return new OptionCollection(questionId);

            var question = ServiceLocator.BankSearch.GetQuestionData(questionId);

            FrameworkIdentifier = question.Set.Bank.Standard;
            return new OptionCollection(questionId);
        }

        protected override Repeater GetRepeater() => Repeater;

        protected override HiddenField GetItemsOrder() => ItemsOrder;

        protected override IEnumerable<BaseValidator> GetItemValidators(RepeaterItem item)
        {
            yield return (BaseValidator)item.FindControl("TextRequiredValidator");
        }

        #endregion

        #region Event handlers

        private void AddOptionButton_Click(object sender, EventArgs e)
        {
            OptionItems.Add(new OptionItem());

            Repeater.DataBind();

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(SingleCorrectOptionRepeater),
                "focus_last",
                $"$('#{ClientID} > tbody > tr:last > td input[type=\"text\"]:first').focus();",
                true);
        }

        protected override void OnRepeaterItemCreated(RepeaterItem item)
        {
            base.OnRepeaterItemCreated(item);

            var competencyInput = (FindStandard)item.FindControl("OptionCompetency");
            competencyInput.Filter.RootStandardIdentifier = FrameworkIdentifier;
            competencyInput.Visible = Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection;
        }

        protected override void OnOptionItemDataBound(RepeaterItem repeaterItem, ItemCollection.DataItem optionItem) =>
            OnOptionItemDataBound(repeaterItem, (OptionItem)optionItem);

        private void OnOptionItemDataBound(RepeaterItem repeaterItem, OptionItem optionItem)
        {
            var textInput = (EditorTranslation)repeaterItem.FindControl("OptionText");
            var pointsInput = (NumericBox)repeaterItem.FindControl("Points");
            var cutScoreInput = (NumericBox)repeaterItem.FindControl("CutScore");
            var competencyInput = (FindStandard)repeaterItem.FindControl("OptionCompetency");

            optionItem.Text.Set(textInput.Text);
            optionItem.Points = pointsInput.ValueAsDecimal ?? 0;
            optionItem.CutScore = cutScoreInput.ValueAsDecimal;
            optionItem.Competency = competencyInput.Value;
        }

        #endregion

        #region Methods (data bound)

        protected override void PopulateItems(Question question)
        {
            if (question == null)
            {
                OptionItems.Add(new OptionItem());
                OptionItems.Add(new OptionItem());
                OptionItems.Add(new OptionItem());
                OptionItems.Add(new OptionItem());
            }
            else
            {
                foreach (var option in question.Options)
                    OptionItems.Add(new OptionItem(option));
            }
        }

        #endregion
    }
}