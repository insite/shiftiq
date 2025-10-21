using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Options.Controls
{
    public partial class TrueOrFalseOptionRepeater : OptionWriteRepeater
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
            }

            public override Option GetOption()
            {
                var option = base.GetOption();
                option.Points = Points;
                option.CutScore = CutScore;
                return option;
            }
        }

        #endregion

        #region Initialization

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(TrueOrFalseOptionRepeater),
                "init_reorder",
                $"optionWriteRepeater.initReorder('{ClientID}','{ItemsOrder.ClientID}');",
                true);
        }

        protected override ItemCollection CreateItemCollection(Guid questionId) => new OptionCollection(questionId);

        protected override Repeater GetRepeater() => Repeater;

        protected override HiddenField GetItemsOrder() => ItemsOrder;

        protected override IEnumerable<BaseValidator> GetItemValidators(RepeaterItem item)
        {
            yield return (BaseValidator)item.FindControl("TextRequiredValidator");
        }

        #endregion

        #region Event handlers

        protected override void OnOptionItemDataBound(RepeaterItem repeaterItem, ItemCollection.DataItem optionItem) =>
            OnOptionItemDataBound(repeaterItem, (OptionItem)optionItem);

        private void OnOptionItemDataBound(RepeaterItem repeaterItem, OptionItem optionItem)
        {
            var textInput = (EditorTranslation)repeaterItem.FindControl("OptionText");
            var pointsInput = (NumericBox)repeaterItem.FindControl("Points");
            var cutScoreInput = (NumericBox)repeaterItem.FindControl("CutScore");

            optionItem.Text.Set(textInput.Text);
            optionItem.Points = pointsInput.ValueAsDecimal ?? 0;
            optionItem.CutScore = cutScoreInput.ValueAsDecimal;
        }

        #endregion

        #region Methods (data bound)

        protected override void PopulateItems(Question question)
        {
            if (question == null)
            {
                var itemTrue = new OptionItem();
                itemTrue.Text.Default = "True";

                var itemFalse = new OptionItem();
                itemFalse.Text.Default = "False";

                OptionItems.Add(itemTrue);
                OptionItems.Add(itemFalse);
            }
            else
            {
                for (var i = 0; i < 2; i++)
                {
                    var item = i < question.Options.Count
                        ? new OptionItem(question.Options[i])
                        : new OptionItem();

                    OptionItems.Add(item);
                }
            }
        }

        #endregion
    }
}