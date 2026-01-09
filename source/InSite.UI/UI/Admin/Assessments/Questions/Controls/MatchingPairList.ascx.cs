using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class MatchingPairList : BaseUserControl
    {
        #region OptionItem class

        [Serializable]
        public class AnswerItem
        {
            public int Sequence { get; set; }
            public MultilingualString LeftText { get; set; }
            public MultilingualString RightText { get; set; }

            public AnswerItem(int sequence)
            {
                Sequence = sequence;
            }
        }

        #endregion

        #region Events

        public event EventHandler Refreshed;
        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        public List<AnswerItem> AnswerItems => (List<AnswerItem>)ViewState[nameof(AnswerItems)];

        #endregion

        #region Fields

        private AnswerItem _newItem;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AnswerRepeater.DataBinding += AnswerRepeater_DataBinding;
            AnswerRepeater.ItemCreated += AnswerRepeater_ItemCreated;
            AnswerRepeater.ItemDataBound += AnswerRepeater_ItemDataBound;
            AnswerRepeater.ItemCommand += AnswerRepeater_ItemCommand;

            AddAnswerButton.Click += AddAnswerButton_Click;
        }

        protected override void SetupValidationGroup(string groupName)
        {
            if (!IsBaseControlLoaded)
                return;

            for (var i = 0; i < AnswerRepeater.Items.Count; i++)
                SetupAnswerValidationGroup(AnswerRepeater.Items[i], groupName);
        }

        private void SetupAnswerValidationGroup(RepeaterItem item, string groupName)
        {
            var leftTextRequiredValidator = (BaseValidator)item.FindControl("LeftTextRequiredValidator");
            leftTextRequiredValidator.ValidationGroup = groupName;

            var rightTextRequiredValidator = (BaseValidator)item.FindControl("RightTextRequiredValidator");
            rightTextRequiredValidator.ValidationGroup = groupName;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (AnswerItems != null)
            {
                for (var i = 0; i < AnswerItems.Count; i++)
                {
                    var repeaterItem = AnswerRepeater.Items[i];
                    var answerSequence = (ITextControl)repeaterItem.FindControl("AnswerSequence");
                    var leftText = (ITextControl)repeaterItem.FindControl("LeftText");
                    var rightText = (ITextControl)repeaterItem.FindControl("RightText");

                    var leftOptionText = (EditorTranslation)repeaterItem.FindControl("LeftOptionText");
                    var rightOptionText = (EditorTranslation)repeaterItem.FindControl("RightOptionText");

                    var sequence = int.Parse(answerSequence.Text);
                    var item = AnswerItems.FirstOrDefault(x => x.Sequence == sequence);
                    if (item != null)
                    {
                        item.LeftText = leftOptionText.Text;
                        item.RightText = rightOptionText.Text;
                    }
                }
            }
        }

        #endregion

        #region Event handlers

        private void AnswerRepeater_DataBinding(object sender, EventArgs e)
        {
            AnswerRepeater.DataSource = AnswerItems;
        }

        private void AnswerRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            SetupAnswerValidationGroup(e.Item, ValidationGroup);
        }

        private void AnswerRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = (AnswerItem)e.Item.DataItem;

            var leftOptionText = (EditorTranslation)e.Item.FindControl("LeftOptionText");
            leftOptionText.Text = item.LeftText;

            var rightOptionText = (EditorTranslation)e.Item.FindControl("RightOptionText");
            rightOptionText.Text = item.RightText;
        }

        private void AnswerRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var sequence = int.Parse((string)e.CommandArgument);
                var item = AnswerItems.FirstOrDefault(x => x.Sequence == sequence);
                if (item == null)
                    return;

                AnswerItems.Remove(item);

                AnswerRepeater.DataBind();

                OnRefreshed();
            }
        }

        private void AddAnswerButton_Click(object sender, EventArgs e)
        {
            AnswerItems.Add(_newItem = new AnswerItem(GetNewSequence()));

            AnswerRepeater.DataBind();

            OnRefreshed();
        }

        #endregion

        #region Public methods

        public void LoadData(MatchingList list)
        {
            ClearItems();

            foreach (var pair in list.Pairs)
            {
                var item = new AnswerItem(GetNewSequence())
                {
                    LeftText = pair.Left.Title,
                    RightText = pair.Right.Title
                };

                AnswerItems.Add(item);
            }

            AnswerRepeater.DataBind();
        }

        public void Update(MatchingList list)
        {
            if (AnswerItems == null)
                return;

            for (var i = 0; i < AnswerItems.Count; i++)
            {
                var item = AnswerItems[i];

                MatchingPair pair;

                if (item.Sequence >= list.Pairs.Count)
                {
                    list.Pairs.Add(pair = new MatchingPair());
                }
                else
                    pair = list.Pairs[item.Sequence];

                pair.Left.Title = item.LeftText;
                pair.Right.Title = item.RightText;
                pair.Points = 1;
            }

            for (var i = list.Pairs.Count - 1; i >= 0; i--)
            {
                if (AnswerItems.Find(x => x.Sequence == i) == null)
                    list.Pairs.RemoveAt(i);
            }
        }

        #endregion

        #region Helper methods

        public int GetNewSequence()
        {
            var value = (int)(ViewState[nameof(GetNewSequence)] ?? -1);

            ViewState[nameof(GetNewSequence)] = ++value;

            return value;
        }

        public void ClearItems()
        {
            ViewState[nameof(GetNewSequence)] = null;
            ViewState[nameof(AnswerItems)] = new List<AnswerItem>();
        }

        #endregion
    }
}