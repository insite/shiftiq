using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Sections.Controls;
using InSite.Admin.Assessments.Sections.Models;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Admin.Assessments.Forms.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        [Serializable]
        private class FormInfo
        {
            public string Name { get; set; }
            public int? TimeLimit { get; set; }

            public List<Tuple<Guid, List<Guid>>> Criteria { get; set; }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private List<Guid> CriterionRepeaterDataKeys => (List<Guid>)(ViewState[nameof(CriterionRepeaterDataKeys)]
            ?? (ViewState[nameof(CriterionRepeaterDataKeys)] = new List<Guid>()));

        private List<FormInfo> PreviewData
        {
            get => (List<FormInfo>)ViewState[nameof(PreviewData)];
            set => ViewState[nameof(PreviewData)] = value;
        }

        private Dictionary<Guid, string[]> QuestionStatusFilterDataSource
        {
            get => (Dictionary<Guid, string[]>)ViewState[nameof(QuestionStatusFilterDataSource)];
            set => ViewState[nameof(QuestionStatusFilterDataSource)] = value;
        }

        private Guid SpecificationID => Guid.TryParse(Request.QueryString["spec"], out var value) ? value : Guid.Empty;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriterionRepeater.ItemDataBound += CriterionRepeater_ItemDataBound;

            QuestionStatusValidator.ServerValidate += QuestionStatusValidator_ServerValidate;
            QuestionStatusUpdatePanel.Request += QuestionStatusUpdatePanel_Request;

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

        protected override void CreateChildControls()
        {
            if (PreviewFormsNav.ItemsCount == 0 && PreviewData.IsNotEmpty())
            {
                for (var i = 0; i < PreviewData.Count; i++)
                    AddPreviewFormsNavItem(out _, out _, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Event handlers

        private void QuestionStatusValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = QuestionStatusList.Items.Cast<System.Web.UI.WebControls.ListItem>().Any(x => x.Selected);
        }

        private void QuestionStatusUpdatePanel_Request(object sender, StringValueArgs e)
        {
            if (e.Value == "refresh")
                BindQuestionStatusList();
        }

        private void CriterionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var setWeightSum = 0m;
                var questionLimitSum = 0;
                var availableSum = 0;
                var requiredSum = 0;

                foreach (var dataObj in (IEnumerable)CriterionRepeater.DataSource)
                {
                    setWeightSum += (decimal)DataBinder.Eval(dataObj, "SetWeight");
                    questionLimitSum += (int)DataBinder.Eval(dataObj, "QuestionLimit");
                    availableSum += (int)DataBinder.Eval(dataObj, "QuestionCount");
                    requiredSum += (int)DataBinder.Eval(dataObj, "RequiredCount");
                }

                var setWeightSumLiteral = (ITextControl)e.Item.FindControl("SetWeightSum");
                setWeightSumLiteral.Text = setWeightSum.ToString("p0");

                var questionLimitSumLiteral = (ITextControl)e.Item.FindControl("QuestionLimitSum");
                questionLimitSumLiteral.Text = questionLimitSum.ToString("n0");

                var availableSumLiteral = (ITextControl)e.Item.FindControl("AvailableSum");
                availableSumLiteral.Text = availableSum.ToString("n0");

                var requiredSumLiteral = (ITextControl)e.Item.FindControl("RequiredSum");
                requiredSumLiteral.Text = requiredSum.ToString("n0");
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            PreviewQuestionsSection.Visible = false;
            PreviewFormsSection.Visible = false;
            SaveButton.Visible = false;

            if (!Page.IsValid)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var criteria = GetSelectedCriteria().Select(id => bank.FindCriterion(id)).ToArray();
            var statusFilter = new HashSet<string>(QuestionStatusList.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value));
            var questionFilter = new QuestionFilterHelper(criteria, statusFilter, LikeItemGroups.SelectedValue == "MutuallyExclusive");
            var formCount = FormsCount.ValueAsInt.Value;

            PreviewData = new List<FormInfo>();

            var questionListData = new List<List<Tuple<Criterion, List<Question>>>>();

            for (var i = 1; i <= formCount; i++)
            {
                var list = questionFilter.GetResult();

                questionListData.Add(list);

                PreviewData.Add(new FormInfo
                {
                    Name = formCount == 1 ? FormName.FormNameInput : $"{FormName.FormNameInput} {i}",
                    TimeLimit = TimeLimit.ValueAsInt,
                    Criteria = list.Select(x => new Tuple<Guid, List<Guid>>(
                        x.Item1.Identifier,
                        x.Item2.Select(y => y.Identifier).ToList()
                    )).ToList()
                });
            }

            if (PreviewData.Count > 1)
            {
                PreviewFormsSection.Visible = true;
                PreviewFormsSectionHeader.Text = $"Preview Forms ({PreviewData.Count})";

                PreviewFormsNav.ClearItems();

                for (var i = 0; i < PreviewData.Count; i++)
                {
                    var formData = PreviewData[i];

                    AddPreviewFormsNavItem(out var navItem, out var questionList, out var alert);

                    navItem.SetTitle(formData.Name, formData.Criteria.Sum(x => x.Item2.Count));

                    BindQuestionList(questionListData[i], questionList, alert);
                }
            }
            else
            {
                PreviewQuestionsSection.Visible = true;
                PreviewQuestionsSectionHeader.Text = $"Preview Questions ({PreviewData[0].Criteria.Sum(x => x.Item2.Count)})";

                BindQuestionList(questionListData[0], PreviewQuestionsList, PreviewQuestionsAlert);
            }

            SaveButton.Visible = true;
            NextButton.Visible = false;
        }

        private void BindQuestionList(List<Tuple<Criterion, List<Question>>> data, QuestionList list, Alert alert)
        {
            if (data.Count == 0)
            {
                list.Visible = false;

                alert.Visible = true;
                alert.AddMessage(AlertType.Warning, "No Questions");
            }
            else if (data.SelectMany(x => x.Item2).GroupBy(x => x).Any(x => x.Count() > 1))
            {
                list.Visible = false;

                alert.Visible = true;
                alert.AddMessage(AlertType.Error, "Unable display questions: the form can't contain more than one reference to a question.");
            }
            else
            {
                alert.Visible = false;

                list.Visible = true;
                list.LoadData(data);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToFinder();

            var title =
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            SpecificationOutputField.Visible = bank.IsAdvanced;
            CodeField.Visible = bank.IsAdvanced;

            if (bank.IsAdvanced)
            {
                var spec = bank.FindSpecification(SpecificationID);
                if (spec == null)
                    RedirectBack();

                SetInputValues(spec);
            }
            else
            {
                SetInputValues(bank);
            }
        }

        private void Save()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToFinder();

            Guid specId, formId;

            if (bank.IsAdvanced)
            {
                var spec = bank.FindSpecification(SpecificationID);
                if (spec == null)
                    RedirectBack();

                specId = spec.Identifier;

                if (spec.Type == SpecificationType.Static)
                {
                    formId = Guid.Empty;

                    foreach (var formData in PreviewData)
                    {
                        var newFormId = UniqueIdentifier.Create();

                        ServiceLocator.SendCommand(new AddForm(
                            BankID,
                            spec.Identifier,
                            newFormId,
                            formData.Name,
                            Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset),
                            formData.TimeLimit));

                        SetFormProperties(newFormId);

                        var sequence = 1;

                        foreach (var sieveAndQuestions in formData.Criteria)
                        {
                            var sieveIdentifier = sieveAndQuestions.Item1;
                            var sectionId = UniqueIdentifier.Create();

                            ServiceLocator.SendCommand(new AddSection(
                                BankID,
                                newFormId,
                                sectionId,
                                sieveIdentifier));

                            foreach (var questionIdentifier in sieveAndQuestions.Item2)
                            {
                                ServiceLocator.SendCommand(new AddField(
                                    BankID,
                                    UniqueIdentifier.Create(),
                                    sectionId,
                                    questionIdentifier,
                                    sequence++));
                            }
                        }

                        if (formId == Guid.Empty)
                            formId = newFormId;
                    }
                }
                else
                {
                    ServiceLocator.SendCommand(new AddForm(
                        BankID,
                        spec.Identifier,
                        formId = UniqueIdentifier.Create(),
                        FormName.FormNameInput,
                        Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset),
                        TimeLimit.ValueAsInt));

                    SetFormProperties(formId);
                }
            }
            else
            {
                ServiceLocator.SendCommand(new AddSpecification(
                    bank.Identifier,
                    SpecificationType.Dynamic,
                    ConsequenceType.Medium,
                    specId = UniqueIdentifier.Create(),
                    "Default",
                    Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset),
                    0,
                    0,
                    new ScoreCalculation
                    {
                        Disclosure = DisclosureType.None,
                        PassingScore = 0.5M,
                        SuccessWeight = 1M,
                        FailureWeight = 1M
                    }));

                ServiceLocator.SendCommand(new AddForm(
                    BankID,
                    specId,
                    formId = UniqueIdentifier.Create(),
                    FormName.FormNameInput,
                    Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset),
                    TimeLimit.ValueAsInt));
            }

            HttpResponseHelper.Redirect(GetReaderUrl(specId, formId));
        }

        private void SetFormProperties(Guid formId)
        {
            if (!string.IsNullOrEmpty(CodeField.FormCode)
                || !string.IsNullOrEmpty(CodeField.FormSource)
                || !string.IsNullOrEmpty(CodeField.FormOrigin)
                )
            {
                ServiceLocator.SendCommand(new ChangeFormCode(BankID, formId, CodeField.FormCode, CodeField.FormSource, CodeField.FormOrigin));
            }

            if (!string.IsNullOrEmpty(CodeField.FormHook))
                ServiceLocator.SendCommand(new ChangeAssessmentHook(BankID, formId, CodeField.FormHook));
        }

        #endregion

        #region Setting/getting input values

        private void SetInputValues(BankState bank)
        {
            SaveButton.Visible = true;

            var backUrl = GetBackUrl();
            CancelButton.NavigateUrl = backUrl;
        }

        private void SetInputValues(Specification spec)
        {
            CriteriaColumn.Visible = spec.Type != SpecificationType.Dynamic;
            NextButton.Visible = spec.Type != SpecificationType.Dynamic;
            SaveButton.Visible = spec.Type == SpecificationType.Dynamic;

            FormsCount.ValueAsInt = 1;

            SpecificationOutputType.Text = spec.Type.GetName();
            SpecificationOutputName.Text = spec.Name;

            if (spec.Type != SpecificationType.Dynamic)
            {
                var hasCriteria = spec.Criteria.Count > 0;

                CriteriaPanel.Visible = hasCriteria;
                NoCriteriaMessage.Visible = !hasCriteria;

                BindCriterionRepeater(spec);

                QuestionStatusFilterDataSource = spec.Criteria.ToDictionary(
                    x => x.Identifier,
                    x => x.Sets.SelectMany(y => y.Questions.Select(z => z.Condition)).Distinct().ToArray());

                BindQuestionStatusList();
            }

            var backUrl = GetBackUrl();
            CancelButton.NavigateUrl = backUrl;
        }

        private void BindCriterionRepeater(Specification spec)
        {
            CriterionRepeaterDataKeys.Clear();
            CriterionRepeaterDataKeys.AddRange(spec.Criteria.Select(x => x.Identifier));

            CriterionRepeater.DataSource = spec.Criteria.Select(criterion =>
            {
                string filterType;
                int requiredCount;

                if (criterion.FilterType == CriterionFilterType.Tag)
                {
                    filterType = "Question Tag";

                    var filter = QuestionDisplayFilter.Parse(criterion.TagFilter);
                    requiredCount = filter.Sum(x => x.Maximum);
                }
                else if (criterion.FilterType == CriterionFilterType.Pivot)
                {
                    filterType = "Pivot Table";
                    requiredCount = criterion.PivotFilter.CellValueSum;
                }
                else
                {
                    filterType = "None";
                    requiredCount = criterion.QuestionCount;
                }

                var availableQuestions = QuestionFilterHelper.GetAvailableQuestions(criterion);

                return new
                {
                    criterion.Title,
                    criterion.SetWeight,
                    criterion.QuestionLimit,
                    QuestionCount = availableQuestions.Count,
                    FilterType = filterType,
                    RequiredCount = requiredCount,
                };
            }).ToArray();

            CriterionRepeater.DataBind();
        }

        private void BindQuestionStatusList()
        {
            var data = GetSelectedCriteria().SelectMany(id => QuestionStatusFilterDataSource[id]).Distinct().OrderBy(x => x).ToArray();

            QuestionStatusList.Items.Clear();

            foreach (var status in data)
                QuestionStatusList.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Value = status ?? string.Empty,
                    Text = string.IsNullOrEmpty(status) ? "<i>(Undefined)</i>" : WebUtility.HtmlEncode(status),
                    Selected = true
                });

            NoQuestionsMessage.Visible = data.Length == 0;
        }

        #endregion

        #region Methods (helpers)

        private Guid[] GetSelectedCriteria()
        {
            var result = new List<Guid>();

            foreach (RepeaterItem item in CriterionRepeater.Items)
            {
                var isSelected = (ICheckBoxControl)item.FindControl("IsSelected");
                if (isSelected.Checked)
                    result.Add(CriterionRepeaterDataKeys[item.ItemIndex]);
            }

            return result.ToArray();
        }

        private void AddPreviewFormsNavItem(out NavItem navItem, out QuestionList questionList, out Alert alert)
        {
            PreviewFormsNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(questionList = (QuestionList)LoadControl("~/UI/Admin/Assessments/Sections/Controls/QuestionList.ascx"));
            navItem.Controls.Add(alert = new Alert { ShowClose = false });
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectBack() =>
            HttpResponseHelper.Redirect(GetBackUrl(), true);

        private string GetBackUrl() =>
            new ReturnUrl().GetReturnUrl() ?? GetReaderUrl();

        private string GetReaderUrl(Guid? specificationId = null, Guid? formId = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (formId.HasValue && formId != Guid.Empty)
                url += $"&form={formId.Value}";
            else if (specificationId.HasValue)
                url += $"&spec={specificationId.Value}";
            else
                url += "&panel=specifications";

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