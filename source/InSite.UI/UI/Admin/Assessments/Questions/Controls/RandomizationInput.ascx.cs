using System;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class RandomizationInput : BaseUserControl
    {
        #region Properties

        private Randomization InitialValues
        {
            get => (Randomization)ViewState[nameof(InitialValues)];
            set => ViewState[nameof(InitialValues)] = value;
        }

        public bool IsChanged => InitialValues == null || !GetCurrentValue().Equals(InitialValues);

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FooterScript.ContentKey = GetType().FullName;
        }

        #endregion

        #region Setting/getting input values

        public void SetInputValues(Question question)
        {
            EnabledInput.Items.Clear();
            EnabledInput.Items.Add(new ComboBoxOption("List Options", bool.FalseString));
            EnabledInput.Items.Add(new ComboBoxOption("Shuffle Options", bool.TrueString));

            Count.Items.Clear();

            if (question.Options.Count >= 2)
            {
                if (!question.Randomization.Enabled)
                    CountWrapper.Attributes["class"] = "d-none";

                Count.Items.Add(new ComboBoxOption());
                for (var i = 2; i <= question.Options.Count; i++)
                    Count.Items.Add(new ComboBoxOption($"1 .. {i}", i.ToString()));

                SetInputValues(question.Randomization);
            }
            else
            {
                EnabledInput.ValueAsBoolean = false;
                EnabledInput.Enabled = false;
                CountWrapper.Visible = false;
            }

            InitialValues = question.Randomization;
        }

        public void SetInputValues(Set set)
        {
            EnabledInput.Items.Clear();
            EnabledInput.Items.Add(new ComboBoxOption("List Questions", bool.FalseString));
            EnabledInput.Items.Add(new ComboBoxOption("Shuffle Questions", bool.TrueString));

            Count.Items.Clear();

            if (set.Questions.Count >= 2)
            {
                if (!set.Randomization.Enabled)
                    CountWrapper.Attributes["class"] = "d-none";

                Count.Items.Add(new ComboBoxOption());
                for (var i = 2; i <= set.Questions.Count; i++)
                    Count.Items.Add(new ComboBoxOption($"1 .. {i}", i.ToString()));

                SetInputValues(set.Randomization);
            }
            else
            {
                EnabledInput.ValueAsBoolean = false;
                EnabledInput.Enabled = false;
                CountWrapper.Visible = false;
            }

            InitialValues = set.Randomization;
        }

        private void SetInputValues(Randomization rand)
        {
            EnabledInput.ValueAsBoolean = rand.Enabled;

            var countItem = Count.FindOptionByValue(rand.Count.ToString());
            if (countItem != null)
                countItem.Selected = true;
        }

        public void GetInputValues(Randomization rand)
        {
            rand.Enabled = EnabledInput.ValueAsBoolean.Value;
            rand.Count = !rand.Enabled
                ? 0
                : Count.ValueAsInt ?? 0;
        }

        public Randomization GetCurrentValue()
        {
            var rand = new Randomization();

            GetInputValues(rand);

            return rand;
        }

        #endregion
    }
}