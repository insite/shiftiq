using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Sections.Models;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Sections.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        [Serializable]
        private class CriterionInfo
        {
            public Guid CriterionID { get; internal set; }
            public Guid SpecificationID { get; internal set; }
            public Guid[] Sets { get; internal set; }
            public int QuestionLimit { get; internal set; }
            public string Name { get; set; }

            public AddCriterion ToCommand(Guid bankId)
            {
                return new AddCriterion(
                    bankId,
                    SpecificationID,
                    Sets,
                    CriterionID,
                    Name,
                    1,
                    QuestionLimit,
                    null,
                    null);
            }

            public CriterionAdded ToChange()
            {
                return new CriterionAdded(
                    SpecificationID,
                    Sets,
                    CriterionID,
                    Name,
                    1,
                    QuestionLimit,
                    null,
                    null);
            }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : Guid.Empty;

        private bool IsAdvanced
        {
            get => (bool)ViewState[nameof(IsAdvanced)];
            set => ViewState[nameof(IsAdvanced)] = value;
        }

        private Guid? CriterionID
        {
            get => (Guid?)ViewState[nameof(CriterionID)];
            set => ViewState[nameof(CriterionID)] = value;
        }

        private CriterionInfo CriterionData
        {
            get => (CriterionInfo)ViewState[nameof(CriterionData)];
            set => ViewState[nameof(CriterionData)] = value;
        }

        private Guid[] QuestionIDs
        {
            get => (Guid[])ViewState[nameof(QuestionIDs)];
            set => ViewState[nameof(QuestionIDs)] = value;
        }

        #endregion

        #region Fields

        private Form _form = null;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SetRequiredValidator.ServerValidate += SetRequiredValidator_ServerValidate;

            SetAction.AutoPostBack = true;
            SetAction.ValueChanged += SetAction_ValueChanged;

            ShowUsedSets.AutoPostBack = true;
            ShowUsedSets.CheckedChanged += ShowUsedSets_CheckedChanged;

            AddQuestions.AutoPostBack = true;
            AddQuestions.SelectedIndexChanged += AddQuestions_SelectedIndexChanged;

            NextButton.Click += NextButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToFinder();

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void SetRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = SetAction.Value == "New"
                || SetCheckBoxList.Visible && SetCheckBoxList.Items.Cast<System.Web.UI.WebControls.ListItem>().Any(x => x.Selected);
        }

        private void ShowUsedSets_CheckedChanged(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var form = bank.FindForm(FormID);
            BindCriterionSelector(form);
        }

        private void AddQuestions_SelectedIndexChanged(object sender, EventArgs e) => OnAddQuestionsChanged();

        private void OnAddQuestionsChanged()
        {
            var isAddQuestions = bool.Parse(AddQuestions.SelectedValue);

            SaveButton.Visible = !isAddQuestions;
            NextButton.Visible = isAddQuestions;
            RandomlySelectField.Visible = !IsAdvanced && isAddQuestions;
            PreviewSection.Visible = false;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            SaveButton.Visible = true;
            NextButton.Visible = false;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);

            if (!IsAdvanced)
            {
                var form = bank.FindForm(FormID);

                SetupCriterionData(form);

                CriterionID = CriterionData.CriterionID;

                bank.When(CriterionData.ToChange());
            }
            else
            {
                CriterionID = CriterionSelector.ValueAsGuid.Value;
            }

            // Identify the list of questions that satisfies the criteria.

            var criterion = bank.FindCriterion(CriterionID.Value);
            var satisfies = QuestionFilterHelper.GetFilteredQuestions(criterion);

            if (!IsAdvanced && RandomlySelectSelector.Value == "Static")
            {
                satisfies.Shuffle();

                if (CriterionData.QuestionLimit > 0 && CriterionData.QuestionLimit < satisfies.Count)
                    satisfies.RemoveRange(CriterionData.QuestionLimit, satisfies.Count - CriterionData.QuestionLimit);
            }

            QuestionIDs = satisfies.Select(x => x.Identifier).ToArray();

            PreviewSection.Visible = true;
            QuestionCount.Text = string.Format("({0})", satisfies.Count);

            ButtonPanel.Visible = true;

            if (satisfies.Count > 0)
            {
                NoQuestions.Visible = false;
                QuestionList.Visible = true;

                QuestionList.LoadData(criterion, satisfies);
            }
            else
            {
                NoQuestions.Visible = true;
                QuestionList.Visible = false;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Save();
        }

        private void SetAction_ValueChanged(object sender, EventArgs e) => OnSetActionSelected();

        private void OnSetActionSelected()
        {
            var form = GetForm();
            var allowSelectSets = true;
            var rsStaticItem = RandomlySelectSelector.FindOptionByValue("Static");
            var rsDynamicItem = RandomlySelectSelector.FindOptionByValue("Dynamic");

            if (SetAction.Value == "Existing")
            {
                RandomlySelectField.Visible = false;

                rsStaticItem.Enabled = true;
                rsStaticItem.Selected = true;
                rsDynamicItem.Enabled = false;

                BindSetCheckBoxList(form);
            }
            else if (SetAction.Value == "New")
            {
                allowSelectSets = false;

                AddQuestions.SelectedValue = bool.FalseString;
                OnAddQuestionsChanged();

                RandomlySelectField.Visible = true;

                rsStaticItem.Enabled = false;
                rsDynamicItem.Enabled = true;
                rsDynamicItem.Selected = true;
            }
            else
            {
                throw new NotImplementedException("Unexpected action value: " + SetAction.Value);
            }

            SetSelectorContainer.Visible = allowSelectSets;
            AddQuestionsField.Visible = allowSelectSets;
        }

        #endregion

        #region Database operations

        private Form GetForm()
        {
            if (_form == null)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                if (bank == null)
                    RedirectToFinder();

                _form = bank.FindForm(FormID);
                if (_form == null || _form.Specification.Type != SpecificationType.Static)
                    RedirectToBankReader();
            }

            return _form;
        }

        private void Open()
        {
            var form = GetForm();

            SetInputValues(form);
        }

        private void Save()
        {
            var sectionId = UniqueIdentifier.Create();
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);

            if (!IsAdvanced)
            {
                if (!PreviewSection.Visible)
                {
                    var form = bank.FindForm(FormID);

                    SetupCriterionData(form);

                    if (SetAction.Value == "New")
                    {
                        var setId = UniqueIdentifier.Create();
                        var command = new AddSet(bank.Identifier, setId, CriterionData.Name, Guid.Empty);
                        var change = new SetAdded(command.Set, command.Name, command.Standard);

                        ServiceLocator.SendCommand(command);

                        if (RandomlySelectSelector.Value == "Dynamic")
                        {
                            ServiceLocator.SendCommand(new ChangeSetRandomization(
                                bank.Identifier,
                                setId,
                                new Randomization { Enabled = true, Count = 0 }));
                        }

                        bank.When(change);

                        CriterionData.Sets = new[] { setId };
                    }

                    CriterionID = CriterionData.CriterionID;
                }

                ServiceLocator.SendCommand(CriterionData.ToCommand(bank.Identifier));

                if (bank.FindCriterion(CriterionID.Value) == null)
                    bank.When(CriterionData.ToChange());
            }
            else if (!CriterionID.HasValue)
            {
                CriterionID = CriterionSelector.ValueAsGuid.Value;
            }

            // Get the list of questions that satisfies the criteria

            var criterion = bank.FindCriterion(CriterionID.Value);
            var satisfies = criterion.Sets.SelectMany(x => x.EnumerateAllQuestions()).Where(x => QuestionIDs.Contains(x.Identifier));

            // Then, add a new section to the form.

            ServiceLocator.SendCommand(new AddSection(BankID, FormID, sectionId, criterion.Identifier));

            // Finally, add a new field to the section for question in the list.
            if (bool.Parse(AddQuestions.SelectedValue))
            {
                foreach (var question in satisfies)
                    ServiceLocator.SendCommand(new AddField(
                        BankID,
                        UniqueIdentifier.Create(),
                        sectionId,
                        question.Identifier,
                        question.Sequence));
            }

            RedirectToBankReader(FormID, sectionId);
        }

        #endregion

        #region Setting/getting input values

        private void SetInputValues(Form form)
        {
            var bank = form.Specification.Bank;

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            IsAdvanced = bank.IsAdvanced;
            FormDetails.BindForm(form, BankID, IsAdvanced);

            SectionNameField.Visible = !IsAdvanced;
            CriterionField.Visible = IsAdvanced;
            BasicSetField.Visible = !IsAdvanced;
            QuestionLimitField.Visible = !IsAdvanced;
            RandomlySelectField.Visible = !IsAdvanced;
            QuestionLimit.ValueAsInt = 0;

            if (!IsAdvanced)
            {
                SetAction.Value = bank.Sets.Count > 0 ? "Existing" : "New";

                OnSetActionSelected();
            }
            else
            {
                BindCriterionSelector(form);
            }

            CancelButton.NavigateUrl = GetReaderUrl(FormID);
        }

        private void BindCriterionSelector(Form form)
        {
            var excludeSets = form.Sections.SelectMany(x => x.Criterion.SetIdentifiers).ToHashSet();
            var excludeCriteria = new HashSet<Guid>(form.Sections.Count > 0 ? form.Sections.Select(x => x.CriterionIdentifier) : new Guid[0]);

            CriterionSelector.LoadItems(
                form.Specification.Criteria.OrderBy(x => x.Title)
                    .Where(x => !excludeCriteria.Contains(x.Identifier))
                    .Where(x => !x.SetIdentifiers.Any(y => excludeSets.Contains(y)) || ShowUsedSets.Checked),
                "Identifier", "Title"
            );

            if (CriterionSelector.Items.Count == 2)
                ((IComboBoxOption)CriterionSelector.Items[1]).Selected = true;
        }

        private void BindSetCheckBoxList(Form form)
        {
            var message = string.Empty;

            SetCheckBoxList.Items.Clear();

            if (form.Specification.Bank.Sets.Count > 0)
            {
                var attachedSets = form.Sections.SelectMany(x => x.Criterion.Sets).Select(x => x.Identifier).ToHashSet();
                var availableSets = form.Specification.Bank.Sets.Where(x => !attachedSets.Contains(x.Identifier)).ToArray();

                if (availableSets.Length > 0)
                {
                    foreach (var set in availableSets)
                        SetCheckBoxList.Items.Add(new System.Web.UI.WebControls.ListItem($"{set.Sequence}. {set.Name}", set.Identifier.ToString()));
                }
                else
                {
                    message = "All of the question sets in this bank are already being used on this form.";
                }
            }
            else
            {
                message = "The bank has no sets.";
            }

            var hasMessage = message.Length > 0;

            SetCheckBoxList.Visible = !hasMessage;
            SetMessage.Visible = hasMessage;

            if (hasMessage)
                SetMessage.Text = $"<div class='alert alert-warning' style='margin-bottom:0;'>{message}</div>";
        }

        private void SetupCriterionData(Form form)
        {
            CriterionData = new CriterionInfo
            {
                CriterionID = UniqueIdentifier.Create(),
                SpecificationID = form.Specification.Identifier,
                Sets = SetCheckBoxList.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => Guid.Parse(x.Value)).ToArray(),
                QuestionLimit = QuestionLimit.ValueAsInt.Value,
                Name = SectionName.Text
            };
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToBankReader(Guid? formId = null, Guid? sectionId = null)
        {
            var url = GetReaderUrl(formId, sectionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? formId = null, Guid? sectionId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue)
                url += $"&form={formId.Value}";

            if (sectionId.HasValue)
                url += $"&section={sectionId.Value}&tab=section";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}