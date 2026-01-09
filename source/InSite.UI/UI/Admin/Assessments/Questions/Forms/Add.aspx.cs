using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assessments.Options.Controls;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region Constants

        private const string ComposedTypeValue = "-1";
        private const string HotspotSingleSelectTypeValue = "-2";
        private const string HotspotMultiSelectTypeValue = "-3";

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.TryParse(Request.QueryString["set"], out var value) ? value : Guid.Empty;

        private Guid? SectionID => Guid.TryParse(Request["section"], out var identifier) ? identifier : (Guid?)null;

        private Guid? CompetencyID => Guid.TryParse(Request["competency"], out var identifier) ? identifier : (Guid?)null;

        private Guid? RubricID => Guid.TryParse(Request["rubric"], out var identifier) ? identifier : (Guid?)null;

        private bool IsStandardScenario
        {
            get => (bool)(ViewState[nameof(IsStandardScenario)] ?? false);
            set => ViewState[nameof(IsStandardScenario)] = value;
        }

        #endregion

        #region Methods (Initialization)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (s, a) => OnCreationTypeSelected();

            DuplicateBankId.AutoPostBack = true;
            DuplicateBankId.ValueChanged += (s, a) => OnDuplicateBankChanged();

            QuestionType.AutoPostBack = true;
            QuestionType.ValueChanged += (s, a) => OnQuestionTypeChanged();

            QuestionSubtype.AutoPostBack = true;
            QuestionSubtype.ValueChanged += (s, a) => OnQuestionSubtypeChanged();

            if (!CompetencyID.HasValue)
            {
                QuestionSetSelector.AutoPostBack = true;
                QuestionSetSelector.ValueChanged += QuestionSetSelector_ValueChanged;
            }

            SaveButton.Click += SaveClicked;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToFinder();

            if (IsPostBack)
                return;

            var returnUrl = WebUrl.TryCreate(GetReturnUrl());
            IsStandardScenario = IsStandardEditorAction(returnUrl?.Path);

            InitQuestionType();
            InitQuestionSubtype();

            Open();
        }

        #endregion

        #region Methods (Binding)

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToFinder();

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            BankDetails.BindBank(bank);
            QuestionText.LoadData(bank);
            OrderingDetails.InitData(bank);

            QuestionOutputStandardField.Visible = CompetencyID.HasValue;
            QuestionOutputStandard.AssetID = CompetencyID ?? Guid.Empty;

            SetOutputField.Visible = bank.IsAdvanced;
            SetInputField.Visible = !bank.IsAdvanced;
            CreationTypePanel.Visible = bank.IsAdvanced;

            if (bank.IsAdvanced)
            {
                CreationType.EnsureDataBound();
                CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate);
                OnCreationTypeSelected();
                OnDuplicateBankChanged();

                OpenAdvanced(bank);
            }
            else
            {
                OpenBasic(bank);
            }

            CancelButton.NavigateUrl = GetReaderUrl();

            SetQuestionType(QuestionItemType.SingleCorrect);

            LikertDetails.SetCompetencyFramework(bank.Standard);

            InitRubric();
        }

        private void OpenBasic(BankState bank)
        {
            var selectedSetId = SetID;
            HashSet<Guid> allowedSetId = null;

            if (SectionID.HasValue)
            {
                var section = bank.FindSection(SectionID.Value);
                if (section != null)
                {
                    if (selectedSetId == Guid.Empty && section.Criterion.Sets.Count == 1)
                        selectedSetId = section.Criterion.Sets[0].Identifier;

                    if (section.Criterion.Sets.Count > 0)
                        allowedSetId = section.Criterion.Sets.Select(x => x.Identifier).ToHashSet();
                }
            }

            if (selectedSetId == Guid.Empty && bank.Sets.Count == 1)
            {
                selectedSetId = bank.Sets[0].Identifier;
            }

            QuestionSetSelector.LoadItems(
                bank.Sets.Where(x => allowedSetId == null || allowedSetId.Contains(x.Identifier)),
                "Identifier", "Name"
            );
            QuestionSetSelector.ValueAsGuid = selectedSetId;

            OnSetChanged(null);
        }

        private void OpenAdvanced(BankState bank)
        {
            var set = bank.FindSet(SetID);
            if (set == null)
                RedirectToReader();

            SetDetails.BindSet(set);

            OnSetChanged(set);
        }

        private void InitRubric()
        {
            if (Request.QueryString["returnFromRubric"] != "1")
                return;

            SetQuestionType(Request.QueryString["type"].ToEnum(QuestionItemType.ComposedEssay));

            RubricDetail.RubricID = RubricID;
        }

        private void Save()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var setId = bank.IsAdvanced ? SetID : QuestionSetSelector.ValueAsGuid.Value;
            var set = bank.FindSet(setId);
            var creationType = CreationType.ValueAsEnum;
            var isDuplicate = bank.IsAdvanced && creationType == CreationTypeEnum.Duplicate;

            Question question;

            if (isDuplicate)
                question = CreateQuestionDuplicate(set);
            else
                question = CreateQuestionOne(set);

            if (question == null)
                return;

            question.Identifier = UniqueIdentifier.Create();
            question.Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);

            ServiceLocator.SendCommand(new AddQuestion(BankID, setId, question));

            var section = SectionID.HasValue ? bank.FindSection(SectionID.Value) : null;
            if (section != null && section.Criterion.Sets.Any(x => x.Identifier == setId))
                ServiceLocator.SendCommand(new AddField(BankID, UniqueIdentifier.Create(), section.Identifier, question.Identifier, -1));

            if (!isDuplicate && question.Type.IsComposed() && RubricDetail.RubricID.HasValue)
                ServiceLocator.SendCommand(new ConnectQuestionRubric(BankID, question.Identifier, RubricDetail.RubricID.Value));

            RedirectToReader(question.Identifier);
        }

        private Question CreateQuestionDuplicate(Set set)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(DuplicateBankId.Value.Value);
            var question = bank?.FindQuestion(DuplicateQuestionId.Value.Value);

            if (question == null)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Question not found.");
                return null;
            }

            var duplicate = question.Clone(cloneIdentifiers: false);
            duplicate.Set = set;
            duplicate.Identifier = Guid.Empty;
            duplicate.Standard = Guid.Empty;
            duplicate.SubStandards = null;
            duplicate.Source = question.Identifier;
            duplicate.Asset = 0;
            duplicate.PublicationStatus = PublicationStatus.Drafted;
            duplicate.FirstPublished = null;
            duplicate.AttachmentIdentifiers = new HashSet<Guid>();

            if (question.Type == QuestionItemType.Likert)
            {
                foreach (var row in question.Likert.Rows)
                {
                    row.Standard = Guid.Empty;
                    row.SubStandards = null;
                }
            }
            else if (question.Options.IsNotEmpty())
            {
                foreach (var option in question.Options)
                    option.Standard = Guid.Empty;
            }

            return duplicate;
        }

        private Question CreateQuestionOne(Set set)
        {
            var question = new Question();
            question.Set = set;
            question.Type = GetQuestionType();
            question.CalculationMethod = CalculationMethod.EnumValue ?? QuestionCalculationMethod.Default;
            question.Points = MaximumPoints.ValueAsDecimal;
            question.Content.Title = QuestionText.Text;

            if (question.Type.IsComposed())
                question.Content.Exemplar = ExemplarText.Text;

            string error;

            if (question.Type == QuestionItemType.Matching)
                error = SetupMatching(question);
            else if (question.Type == QuestionItemType.Likert)
                error = SetupLikert(question);
            else if (question.Type.IsHotspot())
                error = SetupHotspot(question);
            else if (question.Type.IsComposed())
                error = null;
            else if (question.Type == QuestionItemType.Ordering)
                error = SetupOrdering(question);
            else
                error = SetupOptions(question);

            if (error.IsNotEmpty())
            {
                CreatorStatus.AddMessage(AlertType.Error, error);
                return null;
            }

            SetupCompetency(question);

            return question;
        }

        private void SetupCompetency(Question question)
        {
            var competencyId = CompetencyID ?? QuestionInputStandard.ValueAsGuid;
            if (!competencyId.HasValue)
                return;

            var set = question.Set;
            var isCompetencyValid = set.Standard != Guid.Empty
                && StandardSearch.Exists(x => x.StandardIdentifier == competencyId.Value && x.Parent.StandardIdentifier == set.Standard);

            if (!isCompetencyValid)
                return;

            question.Standard = competencyId.Value;
            question.SubStandards = null;
        }

        private string SetupMatching(Question question)
        {
            if (question.Points.Value == 0)
                return "The question has no points.";

            question.Content.Rationale = MatchingDetails.GetRationale();
            question.Content.RationaleOnCorrectAnswer = MatchingDetails.GetRationaleOnCorrectAnswer();
            question.Content.RationaleOnIncorrectAnswer = MatchingDetails.GetRationaleOnIncorrectAnswer();
            question.Matches = MatchingDetails.GetMatches();

            return null;
        }

        private string SetupLikert(Question question)
        {
            var error = LikertDetails.GetError();
            if (error.IsNotEmpty())
                return error;

            question.Likert = LikertDetails.GetMatrix();

            return null;
        }

        private string SetupHotspot(Question question)
        {
            var error = HotspotDetails.GetError();
            if (error.IsNotEmpty())
                return error;

            question.Hotspot = HotspotDetails.GetHotspot(question.Set.Bank.Asset);

            return null;
        }

        private string SetupOrdering(Question question)
        {
            var error = OrderingDetails.GetError();
            if (error.IsNotEmpty())
                return error;

            question.Ordering = OrderingDetails.GetOrdering();

            return null;
        }

        private string SetupOptions(Question question)
        {
            var hasPoints = question.Type.IsRadioList();
            var hasCorrect = question.Type.IsCheckList();

            if (hasCorrect && question.Points.Value == 0)
                return "The question has no points.";

            question.Options = new List<Option>();
            ((OptionWriteRepeater)Options.GetControl()).GetInputValues(question);

            if (hasPoints && question.Options.All(x => !x.HasPoints) || hasCorrect && question.Options.All(x => x.IsTrue != true))
                return "The question contains no correct option.";

            return null;
        }

        #endregion

        #region Methods (question type)

        private void InitQuestionType()
        {
            QuestionType.EnsureDataBound();

            var lastComposedIndex = -1;
            var lastSingleSelectHotspotIndex = -1;
            var lastMultiSelectHotspotIndex = -1;

            for (var i = 0; i < QuestionType.Items.Count; i++)
            {
                var option = (ComboBoxOption)QuestionType.Items[i];
                var type = option.Value.ToEnum<QuestionItemType>();

                if (type == QuestionItemType.ComposedEssay || type == QuestionItemType.ComposedVoice)
                {
                    option.Visible = false;
                    lastComposedIndex = i;
                }
                else if (type == QuestionItemType.HotspotStandard || type == QuestionItemType.HotspotImageCaptcha)
                {
                    option.Visible = false;
                    lastSingleSelectHotspotIndex = i;
                }
                else if (type == QuestionItemType.HotspotMultipleChoice || type == QuestionItemType.HotspotMultipleAnswer)
                {
                    option.Visible = false;
                    lastMultiSelectHotspotIndex = i;
                }
            }

            if (lastComposedIndex >= 0)
                InsertTypeItem(lastComposedIndex, ComposedTypeValue, "comment", "Composed");

            if (lastSingleSelectHotspotIndex >= 0)
                InsertTypeItem(lastSingleSelectHotspotIndex, HotspotSingleSelectTypeValue, "bullseye-pointer", "Single-Select Hotspot").Visible = false;

            if (lastMultiSelectHotspotIndex >= 0)
                InsertTypeItem(lastMultiSelectHotspotIndex, HotspotMultiSelectTypeValue, "bullseye-pointer", "Multi-Select Hotspot").Visible = false;
        }

        private ComboBoxOption InsertTypeItem(int index, string value, string icon, string text)
        {
            var item = new ComboBoxOption
            {
                Value = value,
                Icon = "fas fa-" + icon,
                Text = text
            };

            QuestionType.Items.Insert(index, item);

            return item;
        }

        private void InitQuestionSubtype()
        {
            QuestionSubtype.EnsureDataBound();

            foreach (ComboBoxOption option in QuestionSubtype.Items)
            {
                var type = option.Value.ToEnum<QuestionItemType>();

                if (type.IsComposed())
                {
                    if (option.Text.StartsWith("Composed "))
                        option.Text = option.Text.Substring(9);
                }
                else if (type.IsHotspot() && type != QuestionItemType.HotspotCustom)
                {
                    if (option.Text.StartsWith("Hotspot "))
                        option.Text = option.Text.Substring(8);
                }
            }
        }

        private void LoadQuestionSubtype()
        {
            if (QuestionType.Value == ComposedTypeValue)
            {
                QuestionSubtypeField.Visible = true;
                SetSubtypeItemsVisibility(QuestionItemType.ComposedEssay, QuestionItemType.ComposedVoice);
                QuestionSubtype.ValueAsEnum = QuestionItemType.ComposedEssay;
            }
            else if (QuestionType.Value == HotspotSingleSelectTypeValue)
            {
                QuestionSubtypeField.Visible = true;
                SetSubtypeItemsVisibility(QuestionItemType.HotspotStandard, QuestionItemType.HotspotImageCaptcha);
                QuestionSubtype.ValueAsEnum = QuestionItemType.HotspotStandard;
            }
            else if (QuestionType.Value == HotspotMultiSelectTypeValue)
            {
                QuestionSubtypeField.Visible = true;
                SetSubtypeItemsVisibility(QuestionItemType.HotspotMultipleChoice, QuestionItemType.HotspotMultipleAnswer);
                QuestionSubtype.ValueAsEnum = QuestionItemType.HotspotMultipleChoice;
            }
            else
            {
                QuestionSubtypeField.Visible = false;
            }
        }

        private void SetSubtypeItemsVisibility(params QuestionItemType[] values)
        {
            foreach (ComboBoxOption option in QuestionSubtype.Items)
            {
                var value = option.Value.ToEnum<QuestionItemType>();

                option.Visible = values.Contains(value);
            }
        }

        private void SetQuestionType(QuestionItemType value)
        {
            var isComposed = value.IsComposed();
            var isSingleSelectHotspot = value == QuestionItemType.HotspotStandard || value == QuestionItemType.HotspotImageCaptcha;
            var isMultiSelectHotspot = value == QuestionItemType.HotspotMultipleChoice || value == QuestionItemType.HotspotMultipleAnswer;

            if (isComposed)
                QuestionType.Value = ComposedTypeValue;
            else if (isSingleSelectHotspot)
                QuestionType.Value = HotspotSingleSelectTypeValue;
            else if (isMultiSelectHotspot)
                QuestionType.Value = HotspotMultiSelectTypeValue;
            else
                QuestionType.ValueAsEnum = value;

            OnQuestionTypeChanged();

            if (isComposed || isSingleSelectHotspot || isMultiSelectHotspot)
            {
                QuestionSubtype.ValueAsEnum = value;
                ApplyQuestionType();
            }
        }

        private QuestionItemType GetQuestionType()
        {
            if (QuestionType.Value == ComposedTypeValue || QuestionType.Value == HotspotSingleSelectTypeValue || QuestionType.Value == HotspotMultiSelectTypeValue)
                return QuestionSubtype.ValueAsEnum.Value;
            else
                return QuestionType.ValueAsEnum.Value;
        }

        private void ApplyQuestionType()
        {
            var questionType = GetQuestionType();

            var isMatching = questionType == QuestionItemType.Matching;
            var isCheckList = questionType.IsCheckList();
            var isComposed = questionType.IsComposed();
            var isLikert = questionType == QuestionItemType.Likert;
            var isHotspot = questionType.IsHotspot();
            var isOrdering = questionType == QuestionItemType.Ordering;

            ExemplarCard.Visible = isComposed;
            OptionsCard.Visible = !isMatching && !isComposed && !isLikert && !isHotspot && !isOrdering;
            MatchingCard.Visible = isMatching;
            RubricCard.Visible = isComposed && !IsStandardScenario;
            LikertContainer.Visible = isLikert;
            HotspotContainer.Visible = isHotspot;
            OrderingContainer.Visible = isOrdering;

            if (isMatching)
            {
                MatchingDetails.ValidationGroup = SaveButton.ValidationGroup;
                MatchingDetails.InitData();
            }
            else if (isComposed)
            {
                RubricDetail.InitData(BankID, questionType);
            }
            else if (isLikert)
            {
                LikertDetails.ValidationGroup = SaveButton.ValidationGroup;
                LikertDetails.LoadData();
            }
            else if (isHotspot)
            {
                HotspotDetails.ValidationGroup = SaveButton.ValidationGroup;
                HotspotDetails.LoadData();
            }
            else if (isOrdering)
            {
                OrderingDetails.ValidationGroup = SaveButton.ValidationGroup;
                OrderingDetails.LoadData();
            }
            else
            {
                var optionsRepeater = OptionWriteRepeater.LoadRepeater(Options, questionType);
                optionsRepeater.ValidationGroup = SaveButton.ValidationGroup;
                optionsRepeater.LoadData();
            }

            CalculationMethodField.Visible = isCheckList || isMatching;
            MaximumPointsField.Visible = isCheckList || isMatching;

            CalculationMethod.EnumValue = null;
            MaximumPoints.ValueAsDecimal = null;
        }

        #endregion

        #region Methods (Event Handling)

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;
            var isOne = value == CreationTypeEnum.One;
            var isDuplicate = value == CreationTypeEnum.Duplicate;

            OneQuestionPanel1.Visible = isOne;
            OneQuestionPanel2.Visible = isOne;
            DuplicateQuestionContainer.Visible = isDuplicate;

            SetOutputField.Attributes["class"] = isDuplicate
                ? ControlHelper.AddCssClass(SetOutputField.Attributes["class"], "h-100")
                : ControlHelper.RemoveCssClass(SetOutputField.Attributes["class"], "h-100");
        }

        private void OnDuplicateBankChanged()
        {
            DuplicateQuestionId.Filter.BankIdentifier = DuplicateBankId.Value ?? Guid.Empty;
            DuplicateQuestionId.Enabled = DuplicateBankId.Value.HasValue;
        }

        private void OnQuestionTypeChanged()
        {
            LoadQuestionSubtype();
            ApplyQuestionType();
        }

        private void OnQuestionSubtypeChanged()
        {
            ApplyQuestionType();
        }

        private void QuestionSetSelector_ValueChanged(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var set = bank.FindSet(SetID);

            OnSetChanged(set);
        }

        private void OnSetChanged(Set set)
        {
            var isVisible = !CompetencyID.HasValue && set != null && set.Standard != Guid.Empty;

            if (isVisible)
            {
                QuestionInputStandard.ValueAsGuid = null;
                QuestionInputStandard.ListFilter.ParentStandardIdentifiers = new[] { set.Standard };
            }

            QuestionInputStandardField.Visible = isVisible;
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? questionId = null)
        {
            var url = GetReaderUrl(questionId);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? questionId = null)
        {
            var returnUrl = new ReturnUrl();
            var questionIdQuery = questionId.HasValue ? $"question={questionId}" : null;
            var result = IsStandardScenario
                ? returnUrl.GetReturnUrl(append: questionIdQuery)
                : returnUrl.GetReturnUrl(questionIdQuery);

            return result
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/banks/outline"))
                return $"bank={BankID}";

            if (IsStandardEditorAction(parent.Name))
            {
                var webUrl = new WebUrl(GetReturnUrl());
                var removeKeys = webUrl.QueryString.AllKeys.Where(x => !x.Equals("id", StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (var key in removeKeys)
                    webUrl.QueryString.Remove(key);
                return webUrl.QueryString.ToString();
            }

            return null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            IsStandardScenario ? GetParent() : null;

        private static bool IsStandardEditorAction(string value) =>
            value != null && value.EndsWith("/standards/edit");

        #endregion
    }
}
