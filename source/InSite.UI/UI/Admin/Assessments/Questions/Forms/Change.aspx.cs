using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Admin.Assessments.Options.Controls;
using InSite.Admin.Assessments.Questions.Controls;
using InSite.Application.Attempts.Read;
using InSite.Application.Attempts.Write;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Attempts;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Questions.Forms
{
    public partial class Change : AdminBasePage, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region Classes

        private class CompetenyInfo
        {
            public Guid StandardIdentifier { get; set; }
            public Guid ParentStandardIdentifier { get; set; }
            public string StandardType { get; set; }
            public string Code { get; set; }
            public int AssetNumber { get; set; }
            public string Title { get; set; }
            public string ContentTitle { get; set; }

            public CompetenyInfo[] Children { get; set; }

            public string ItemText => (string.IsNullOrEmpty(Code) ? string.Empty : $"{Code}: ")
                + $"{Title} <span class='form-text'>{StandardType} Asset #{AssetNumber}</span>";

            public static readonly Expression<Func<Standard, CompetenyInfo>> Binder = LinqExtensions1.Expr((Standard s) => new CompetenyInfo
            {
                StandardIdentifier = s.StandardIdentifier,
                ParentStandardIdentifier = s.ParentStandardIdentifier.Value,
                StandardType = s.StandardType,
                Code = s.Code,
                AssetNumber = s.AssetNumber,
                Title = (CoreFunctions.GetContentTextEn(s.StandardIdentifier, ContentLabel.Title)
                        ?? CoreFunctions.GetContentTextEn(s.StandardIdentifier, ContentLabel.Summary)).Replace("\r", "").Replace("\n", ""),
                ContentTitle = s.ContentTitle
            });
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid QuestionID => Guid.TryParse(Request.QueryString["question"], out var value) ? value : Guid.Empty;

        private Guid? FormID => Guid.TryParse(Request.QueryString["form"], out var value) ? value : (Guid?)null;

        private Guid? RubricID => Guid.TryParse(Request["rubric"], out var identifier) ? identifier : (Guid?)null;

        private bool IsStandardScenario
        {
            get => (bool)(ViewState[nameof(IsStandardScenario)] ?? false);
            set => ViewState[nameof(IsStandardScenario)] = value;
        }

        #endregion

        #region Fields

        private Question _question;
        private bool _isQuestionLoaded;
        private BankState _bank;
        private bool _isBankLoaded;

        #endregion

        #region UI Event Handling

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AssetVersionIncrement.Click += AssetVersionIncrement_Click;

            Options.ControlAdded += Options_ControlAdded;

            OrderingDetails.Alert += (s, a) => EditorStatus.AddMessage(a.Type, a.Text);

            SetID.AutoPostBack = true;
            SetID.ValueChanged += SetID_ValueChanged;

            StrictCompetencyRepeater.ItemDataBound += StrictCompetencyRepeater_ItemDataBound;

            LooseCompetencyIdentifier.AutoPostBack = true;
            LooseCompetencyIdentifier.ValueChanged += LooseCompetencyIdentifier_ValueChanged;

            ComposedVoiceTimeLimit.MinValue = 0;
            ComposedVoiceTimeLimit.MaxValue = InputAudio.MaximumRecordingTime;

            ComposedVoiceAttemptLimit.MinValue = 0;
            ComposedVoiceAttemptLimit.MaxValue = InputAudio.MaximumAttemptLimit;

            HotspotPinLimit.MinValue = Hotspot.MinPinLimit;

            SaveButton.Click += SaveButton_Click;
        }

        private void Options_ControlAdded(object sender, EventArgs e)
        {
            var repeater = (OptionWriteRepeater)((DynamicControl)sender).GetControl();
            repeater.Alert += (s, a) => EditorStatus.AddMessage(a.Type, a.Text);
        }

        private void AssetVersionIncrement_Click(object sender, EventArgs e)
            => Upgrade();

        private void StrictCompetencyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (CompetenyInfo)e.Item.DataItem;

            var subRepeater = (Repeater)e.Item.FindControl("SubCompetencyRepeater");
            subRepeater.Visible = dataItem.Children.IsNotEmpty();
            subRepeater.DataSource = dataItem.Children;
            subRepeater.DataBind();
        }

        private void SetID_ValueChanged(object sender, EventArgs e)
        {
            if (Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection)
                return;

            var setId = SetID.ValueAsGuid.Value;
            var question = LoadQuestion();
            var set = question.Set.Bank.FindSet(setId);

            var competency = GetStrictCompetencyIdentifier() ?? question.Standard;
            var subCompetencies = GetStrictSubCompetencyIdentifiers() ?? question.SubStandards;

            BindStrictCompetencies(set);
            SetStrictCompetencyIdentifier(competency);
            SetStrictSubCompetencyIdentifiers(subCompetencies);
        }

        private void LooseCompetencyIdentifier_ValueChanged(object sender, FindEntityValueChangedEventArgs e)
        {
            var question = LoadQuestion();
            var subCompetencies = GetLooseSubCompetencyIdentifiers() ?? question.SubStandards;

            BindLooseSubCompetencies(LooseCompetencyIdentifier.Value ?? Guid.Empty);
            SetLooseSubCompetencyIdentifiers(subCompetencies);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                if (!ConfirmRegradeTab.Visible && GetRegradeAttempts(LoadQuestion()) != null)
                {
                    foreach (var navItem in Accordion.GetItems())
                        navItem.Visible = false;

                    ConfirmRegradeTab.Visible = true;
                    ConfirmRegradeTab.IsSelected = true;
                }
                else
                {
                    Save();
                }
            }
            catch (ApplicationError appex)
            {
                EditorStatus.AddMessage(AlertType.Error, appex.Message);
            }
        }

        #endregion

        #region UI Navigation

        private void RedirectToOutline()
            => HttpResponseHelper.Redirect(GetOutlineUrl(), true);

        private void RedirectToSearch()
            => HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToUpgradedQuestion(Guid question)
            => HttpResponseHelper.Redirect(GetEditorUrl(question), true);

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

        public string GetEditorUrl(Guid question)
        {
            var url = new WebUrl(Request.RawUrl);
            url.QueryString["question"] = question.ToString();
            return url.ToString();
        }

        public string GetOutlineUrl()
        {
            var returnUrl = new ReturnUrl();
            return returnUrl.GetReturnUrl()
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}";
        }

        IWebRoute IOverrideWebRouteParent.GetParent() =>
            IsStandardScenario ? GetParent() : null;

        private static bool IsStandardEditorAction(string value) =>
            value != null && value.EndsWith("/standards/edit");

        #endregion

        #region Data Binding

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (IsPostBack)
                return;

            var returnUrl = WebUrl.TryCreate(GetReturnUrl());
            IsStandardScenario = IsStandardEditorAction(returnUrl?.Path);

            Open();
        }

        public Question LoadQuestion()
        {
            if (!_isQuestionLoaded)
            {
                _bank = LoadBank();

                _question = _bank?.FindQuestion(QuestionID);
                _isQuestionLoaded = true;
            }

            return _question;
        }

        public BankState LoadBank()
        {
            if (!_isBankLoaded)
            {
                _bank = ServiceLocator.BankSearch.GetBankState(BankID);

                if (ServiceLocator.BankSearch.GetBank(BankID) == null)
                    _bank = null;

                _isBankLoaded = true;
            }

            return _bank;
        }

        private void Open()
        {
            if (BankID == Guid.Empty)
                RedirectToSearch();

            var question = LoadQuestion();
            if (question == null)
                RedirectToOutline();

            SetInputValues(question);

            var panel = Request.QueryString["panel"];
            if (panel == "comments")
                CommentsTab.IsSelected = true;
        }

        private void SetInputValues(Question question)
        {
            var bank = question.Set.Bank;

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");

            SaveButton.Visible = !bank.IsLocked;

            if (bank.IsLocked)
                EditorStatus.AddMessage(AlertType.Error,
                    "This bank is locked, therefore you cannot save any changes to this question.");

            var attemptCount = ServiceLocator.AttemptSearch.CountAttempts(a => a.Questions.Any(q => q.QuestionIdentifier == question.Identifier));
            if (attemptCount > 0)
                EditorStatus.AddMessage(AlertType.Warning,
                    $"There {(attemptCount == 1 ? "is" : "are")} {"submission".ToQuantity(attemptCount)} to exam forms containing this question, therefore you can make only limited changes to it.");

            BindQuestionTab(question);
            BindSettingsTab(question);
            BindRationaleTab(question);
            BindPrerequisitesTab(question);
            BindCommentsTab(question);
            BindGlossaryTermTab(question);
            BindGradebookTab(question);

            CancelButton.NavigateUrl = GetOutlineUrl();
        }

        private void BindQuestionTab(Question question)
        {
            QuestionTab.Title = $"Question {question.BankIndex + 1}";

            QuestionText.LoadData(question);
            QuestionText.Text = question.Content.Title;

            ExemplarText.Text = question.Content.Exemplar;
            ExemplarCard.Visible = question.Type.IsComposed();

            QuestionType.Text = question.Type.GetDescription();
            PublicationStatusName.Text = question.PublicationStatus.GetDescription();

            BindQuestionAssetVersion(question);

            AttachmentRepeater.LoadData(question);
            AttachmentCard.Visible = AttachmentRepeater.RowCount > 0;

            BindQuestionItems(question);
        }

        private void BindQuestionAssetVersion(Question question)
        {
            var versions = question.EnumerateAllVersions(SortOrder.Descending).Where(x => x != question).Select(x => new
            {
                x.AssetVersion,
                NavigateUrl = GetEditorUrl(x.Identifier)
            }).ToArray();

            AssetVersion.Text = question.AssetVersion.ToString();
            AssetVersionRepeater.DataSource = versions;
            AssetVersionRepeater.DataBind();
            AssetVersionIncrement.Visible = versions.Length == 0 || question.IsLastVersion();
        }

        private void BindQuestionItems(Question question)
        {
            if (question.Type == QuestionItemType.Matching)
            {
                QuestionItemsMultiView.SetActiveView(QuestionMatchingView);

                MatchingDetails.SetInputValues(question);
            }
            else if (question.Type.IsComposed())
            {
                QuestionItemsMultiView.SetActiveView(QuestionRubricView);
                RubricDetail.LoadData(BankID, QuestionID, RubricID);
            }
            else if (question.Type == QuestionItemType.Likert)
            {
                QuestionItemsMultiView.SetActiveView(QuestionLikertView);

                LikertDetails.IsReadOnly = question.Set.Bank.IsLocked;
                LikertDetails.SetCompetencyFramework(question.Set.Bank.Standard);
                LikertDetails.LoadData(question);
            }
            else if (question.Type.IsHotspot())
            {
                QuestionItemsMultiView.SetActiveView(QuestionHotspotView);

                HotspotDetails.LoadData(question);
                HotspotDetails.IsReadOnly = question.Set.Bank.IsLocked;
            }
            else if (question.Type == QuestionItemType.Ordering)
            {
                QuestionItemsMultiView.SetActiveView(QuestionOrderingView);

                OrderingDetails.IsReadOnly = question.Set.Bank.IsLocked;
                OrderingDetails.LoadData(question);
            }
            else
            {
                QuestionItemsMultiView.SetActiveView(QuestionOptionsView);

                var optionsRepeater = OptionWriteRepeater.LoadRepeater(Options, question.Type);
                optionsRepeater.ValidationGroup = "Assessment";
                optionsRepeater.LoadData(question);
                optionsRepeater.IsReadOnly = question.Set.Bank.IsLocked;
            }
        }

        private void BindSettingsTab(Question question)
        {
            var isCheckList = question.Type.IsCheckList();
            var isMatching = question.Type == QuestionItemType.Matching;
            var isComposed = question.Type.IsComposed();
            var isComposedVoice = question.Type == QuestionItemType.ComposedVoice;
            var isHotspotCustom = question.Type == QuestionItemType.HotspotCustom;

            QuestionCondition.EnsureDataBound();
            QuestionCondition.Value = question.Condition;

            QuestionFlag.EnsureDataBound();
            QuestionFlag.FlagValue = question.Flag;
            QuestionFlagIcon.Text = question.Flag.ToIconHtml();

            ComposedVoiceSection.Visible = isComposedVoice;
            ComposedVoiceTimeLimit.ValueAsInt = question.ComposedVoice.TimeLimit;
            ComposedVoiceAttemptLimit.ValueAsInt = question.ComposedVoice.AttemptLimit;

            HotspotCustomSection.Visible = isHotspotCustom;
            HotspotPinLimit.ValueAsInt = question.Hotspot.PinLimit;
            HotspotShowShapes.ValueAsBoolean = question.Hotspot.ShowShapes;

            MaximumPoints.ReadOnly = !isCheckList && !isMatching;
            MaximumPoints.ValueAsDecimal = question.Points;
            CutScore.ValueAsDecimal = question.CutScore;

            CalculationMethodField.Visible = isCheckList || isMatching;
            CalculationMethod.EnumValue = question.CalculationMethod == QuestionCalculationMethod.Default
                ? (QuestionCalculationMethod?)null
                : question.CalculationMethod;

            RandomizationContainer.Visible = !isMatching && !isComposed;
            RandomizationInput.SetInputValues(question);
            LayoutInput.SetInputValue(question.Layout);

            QuestionCode.Text = question.Classification.Code;
            QuestionTag.Text = question.Classification.Tag;
            TaxonomySelector.ValueAsInt = question.Classification.Taxonomy;
            DifficultySelector.ValueAsInt = question.Classification.Difficulty;
            LikeItemGroup.Text = question.Classification.LikeItemGroup;
            SourceDescriptor.Text = question.Classification.Reference;

            BindQuestionSetSection(question);

            var organization = OrganizationSearch.Select(question.Set.Bank.Tenant);
            NoOrganizationTags.Visible = !QuestionTags.LoadData(organization, question.Classification.Tags);
        }

        private void BindQuestionSetSection(Question question)
        {
            SetID.LoadItems(question.Set.Bank.Sets, "Identifier", "Name");
            SetID.ValueAsGuid = question.Set.Identifier;

            var disableStrictCompetency = Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection;

            LooseCompetencyContainer.Visible = disableStrictCompetency;
            StrictCompetencyRepeater.Visible = !disableStrictCompetency;

            if (disableStrictCompetency)
            {
                LooseCompetencyIdentifier.Filter.RootStandardIdentifier = question.Set.Bank.Standard;
                LooseCompetencyIdentifier.Value = question.Standard;

                BindLooseSubCompetencies(question.Standard);
                SetLooseSubCompetencyIdentifiers(question.SubStandards);
            }
            else
            {
                BindStrictCompetencies(question.Set);
                SetStrictCompetencyIdentifier(question.Standard);
                SetStrictSubCompetencyIdentifiers(question.SubStandards);
            }
        }

        private void BindRationaleTab(Question question)
        {
            RationaleTab.Visible = question.Type != QuestionItemType.Matching;

            Rationale.LoadData(question);
            Rationale.Text = question.Content.Rationale;

            RationaleOnCorrectAnswer.LoadData(question);
            RationaleOnCorrectAnswer.Text = question.Content.RationaleOnCorrectAnswer;

            RationaleOnIncorrectAnswer.LoadData(question);
            RationaleOnIncorrectAnswer.Text = question.Content.RationaleOnIncorrectAnswer;

            ContentDescription.LoadData(question);
            ContentDescription.Text = question.Content.Description;
        }

        private void BindPrerequisitesTab(Question question)
        {
            PrerequisitesTab.Visible = !IsStandardScenario;
            if (!IsStandardScenario)
                Prerequisites.LoadData(question);
        }

        private void BindCommentsTab(Question question)
        {
            var returnUrl = "bank&question&panel=comments";

            var returnData = Request.QueryString["return"];
            if (returnData.IsNotEmpty())
                returnUrl += $"&return={returnData}";

            CommentsTab.SetTitle("Comments", question.Comments.Count);
            CommentRepeater.LoadData(BankID, question.Identifier, question.Comments, new ReturnUrl(returnUrl));
        }

        private void BindGlossaryTermTab(Question question)
        {
            GlossaryTermGrid.LoadData(
                question.Identifier,
                "Exam Question",
                question.Options.Where(x => x.Content.Title != null).Select(x => x.Content.Title).Append(question.Content.Title),
                ContentLabel.Title,
                true);

            GlossaryTermTab.SetTitle("Glossary Terms", GlossaryTermGrid.RowCount);
        }

        private void BindGradebookTab(Question question)
        {
            GradebookTab.Visible = !IsStandardScenario && GradeItems.BindModelToControls(question);
        }

        #endregion

        #region Command Sending

        private void Upgrade()
        {
            var upgrade = new UpgradeQuestion(BankID, QuestionID, UniqueIdentifier.Create());
            ServiceLocator.SendCommand(upgrade);
            RedirectToUpgradedQuestion(upgrade.UpgradedQuestion);
        }

        private void Save()
        {
            try
            {
                var question = LoadQuestion();
                var commands = new List<Command>();

                GetCommands(question, commands);

                foreach (var cmd in commands)
                    ServiceLocator.SendCommand(cmd);

                SaveRubric(question);

                RedirectToOutline();
            }
            catch (ApplicationError apperr)
            {
                EditorStatus.AddMessage(AlertType.Error, apperr.Message);
                return;
            }
        }

        private void SaveRubric(Question question)
        {
            if (!question.Type.IsComposed())
                return;

            if (RubricDetail.RubricID.HasValue)
                ConnectRubric(RubricDetail.RubricID.Value, question);
            else
                ServiceLocator.SendCommand(new DisconnectQuestionRubric(BankID, question.Identifier));
        }

        private static void ConnectRubric(Guid rubricId, Question question)
        {
            ServiceLocator.SendCommand(new ConnectQuestionRubric(question.Set.Bank.Identifier, question.Identifier, rubricId));

            var attemptIds = ServiceLocator.AttemptSearch
                .BindAttemptQuestions(
                    x => x.AttemptIdentifier,
                    new QAttemptFilter { QuestionIdentifier = question.Identifier })
                .Distinct()
                .ToArray();
            if (attemptIds.IsEmpty())
                return;

            var rubric = ServiceLocator.RubricSearch.GetRubric(rubricId);
            if (rubric == null || rubric.RubricPoints <= 0)
                return;

            foreach (var attemptId in attemptIds)
                ServiceLocator.SendCommand(new ChangeAttemptQuestionRubric(attemptId, question.Identifier, new AttemptQuestionRubric
                {
                    Identifier = rubricId,
                    Points = rubric.RubricPoints
                }));
        }

        private void GetCommands(Question question, List<Command> commands)
        {
            if (question.Type == QuestionItemType.Likert)
            {
                var error = LikertDetails.GetError();
                if (error.IsNotEmpty())
                    throw ApplicationError.Create(error);
            }
            else if (question.Type.IsHotspot())
            {
                var error = HotspotDetails.GetError();
                if (error.IsNotEmpty())
                    throw ApplicationError.Create(error);
            }
            else if (question.Type == QuestionItemType.Ordering)
            {
                var error = OrderingDetails.GetError();
                if (error.IsNotEmpty())
                    throw ApplicationError.Create(error);
            }

            var points = MaximumPoints.ValueAsDecimal;
            if ((question.Type.IsCheckList() || question.Type == QuestionItemType.Matching) && (!points.HasValue || points.Value == 0))
                throw ApplicationError.Create("The question has no points.");

            var aggregate = question.Set.Bank.Identifier;

            GetQuestionCommands(question, commands);
            GetClassificationCommands(question, commands);
            GetRandomizationCommands(question, commands);
            GetContentCommands(question, commands);

            var optionsCommands = ((OptionWriteRepeater)Options.GetControl())?.GetCommands(question);
            if (optionsCommands != null)
                commands.AddRange(optionsCommands);

            if (!IsStandardScenario)
            {
                var prerequisitesCommands = Prerequisites.GetCommands(question);
                if (prerequisitesCommands != null)
                    commands.AddRange(prerequisitesCommands);
            }

            var matches = MatchingDetails.Visible ? MatchingDetails.GetMatches() : null;
            if (matches != null && !matches.Equals(question.Matches))
                commands.Add(new ChangeQuestionMatches(aggregate, question.Identifier, matches));

            var likertCommands = LikertDetails.GetCommands(question);
            if (likertCommands != null)
                commands.AddRange(likertCommands);

            var hotspotCommands = HotspotDetails.GetCommands(question);
            if (hotspotCommands != null)
                commands.AddRange(hotspotCommands);

            var orderingCommands = OrderingDetails.GetCommands(question);
            if (orderingCommands != null)
                commands.AddRange(orderingCommands);

            GetRegradeCommands(question, commands);
            GetGradeItemCommands(commands);
        }

        private void GetQuestionCommands(Question question, List<Command> commands)
        {
            var bankId = question.Set.Bank.Identifier;

            var set = SetID.ValueAsGuid.Value;
            var isSetChanges = set != question.Set.Identifier;

            if (isSetChanges)
                commands.Add(new ChangeQuestionSet(bankId, question.Identifier, set));

            var condition = QuestionCondition.Value.NullIfEmpty();
            if (condition != question.Condition)
                commands.Add(new ChangeQuestionCondition(bankId, question.Identifier, condition));

            var flag = QuestionFlag.FlagValue ?? FlagType.None;
            if (flag != question.Flag)
                commands.Add(new ChangeQuestionFlag(bankId, question.Identifier, flag));

            if (question.Type == QuestionItemType.ComposedVoice)
            {
                var composedVoice = new ComposedVoice
                {
                    TimeLimit = ComposedVoiceTimeLimit.ValueAsInt ?? 0,
                    AttemptLimit = ComposedVoiceAttemptLimit.ValueAsInt ?? 0
                };

                if (!composedVoice.IsEqual(question.ComposedVoice))
                    commands.Add(new ChangeQuestionComposedVoice(bankId, question.Identifier, composedVoice));
            }

            if (question.Type == QuestionItemType.HotspotCustom)
            {
                var pinLimit = HotspotPinLimit.ValueAsInt ?? Hotspot.MinPinLimit;
                if (question.Hotspot.PinLimit != pinLimit)
                    commands.Add(new ChangeQuestionHotspotPinLimit(bankId, question.Identifier, pinLimit));

                var showShapes = HotspotShowShapes.ValueAsBoolean.Value;
                if (question.Hotspot.ShowShapes != showShapes)
                    commands.Add(new ChangeQuestionHotspotShowShapes(bankId, question.Identifier, showShapes));
            }

            var competency = Guid.Empty;
            var subCompetencies = new Guid[0];

            if (!Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection)
            {
                competency = GetStrictCompetencyIdentifier() ?? Guid.Empty;
                subCompetencies = GetStrictSubCompetencyIdentifiers() ?? new Guid[0];
            }
            else if (LooseCompetencyIdentifier.Value.HasValue)
            {
                competency = LooseCompetencyIdentifier.Value.Value;
                subCompetencies = GetLooseSubCompetencyIdentifiers() ?? new Guid[0];
            }

            var isStandardChanged = isSetChanges
                || competency != question.Standard
                || subCompetencies.Length != question.SubStandards.EmptyIfNull().Length
                || subCompetencies.Length > 0 && subCompetencies.Any(x => !question.SubStandards.Contains(x));

            if (isStandardChanged)
                commands.Add(new ChangeQuestionStandard(bankId, question.Identifier, competency, subCompetencies));

            var points = MaximumPoints.ValueAsDecimal;
            var cutScore = CutScore.ValueAsDecimal;
            var calculationMethod = CalculationMethod.EnumValue ?? QuestionCalculationMethod.Default;

            if (points != question.Points || cutScore != question.CutScore || calculationMethod != question.CalculationMethod)
                commands.Add(new ChangeQuestionScoring(bankId, question.Identifier, points, cutScore, calculationMethod));
        }

        private void GetClassificationCommands(Question question, List<Command> commands)
        {
            var classification = new QuestionClassification
            {
                Code = QuestionCode.Text,
                Difficulty = DifficultySelector.ValueAsInt,
                LikeItemGroup = LikeItemGroup.Text,
                Reference = SourceDescriptor.Text,
                Tag = QuestionTag.Text,
                Taxonomy = TaxonomySelector.ValueAsInt,
                Tags = QuestionTags.SaveData()
            };

            if (!classification.Equals(question.Classification))
                commands.Add(new ChangeQuestionClassification(question.Set.Bank.Identifier, question.Identifier, classification));
        }

        private void GetRandomizationCommands(Question question, List<Command> commands)
        {
            if (!RandomizationContainer.Visible)
                return;

            var layout = LayoutInput.GetCurrentValue();
            if (!layout.Equals(question.Layout))
            {
                if ((question.Layout.Type != OptionLayoutType.None && question.Layout.Type != OptionLayoutType.List) || (layout.Type != OptionLayoutType.None && layout.Type != OptionLayoutType.List))
                    commands.Add(new ChangeQuestionLayout(question.Set.Bank.Identifier, question.Identifier, layout));
            }

            var randomization = RandomizationInput.GetCurrentValue();
            if (!randomization.Equals(question.Randomization))
                commands.Add(new ChangeQuestionRandomization(question.Set.Bank.Identifier, question.Identifier, randomization));
        }

        private void GetContentCommands(Question question, List<Command> commands)
        {
            var content = new ContentExamQuestion
            {
                Title = QuestionText.Text,
                Description = ContentDescription.Text,
                Rationale = GetRationale(
                    question.Type,
                    Rationale,
                    MatchingDetails.GetRationale(),
                    question.Content.Rationale),
                RationaleOnCorrectAnswer = GetRationale(
                    question.Type,
                    RationaleOnCorrectAnswer,
                    MatchingDetails.GetRationaleOnCorrectAnswer(),
                    question.Content.RationaleOnCorrectAnswer),
                RationaleOnIncorrectAnswer = GetRationale(
                    question.Type,
                    RationaleOnIncorrectAnswer,
                    MatchingDetails.GetRationaleOnIncorrectAnswer(),
                    question.Content.RationaleOnIncorrectAnswer)
            };

            if (question.Type.IsComposed())
                content.Exemplar = ExemplarText.Text;

            if (!content.IsEqual(question.Content))
                commands.Add(new ChangeQuestionContent(question.Set.Bank.Identifier, question.Identifier, content));
        }

        private MultilingualString GetRationale(QuestionItemType questionType, QuestionTextEditor editor, MultilingualString matching, MultilingualString @default)
        {
            if (questionType != QuestionItemType.Matching)
                return editor.Text;
            else if (MatchingDetails.Visible)
                return matching;
            else
                return @default?.Clone();
        }

        private void GetRegradeCommands(Question question, List<Command> commands)
        {
            var regradeAttempts = GetRegradeAttempts(question);
            if (regradeAttempts == null)
                return;

            RegradeOption regradeOption;
            if (RegradeOptionSelector.SelectedValue == "AwardPointsForCorrectedAndPrevious")
                regradeOption = RegradeOption.AwardPointsForCorrectedAndPrevious;
            else if (RegradeOptionSelector.SelectedValue == "AwardPointsForCorrectedOnly")
                regradeOption = RegradeOption.AwardPointsForCorrectedOnly;
            else if (RegradeOptionSelector.SelectedValue == "FullCreditForEveryone")
                regradeOption = RegradeOption.FullCreditForEveryone;
            else
                return;

            var oldOptions = question.Options.Select(x => new Domain.Attempts.OldOption
            {
                Key = x.Number,
                IsTrue = x.IsTrue,
                Points = x.Points
            }).ToList();
            var regradedForms = new Dictionary<Guid, QBankForm>();

            foreach (var regradeAttempt in regradeAttempts)
            {
                commands.Add(new RegradeQuestion(
                    regradeAttempt.AttemptIdentifier,
                    regradeAttempt.FormIdentifier,
                    question.Identifier,
                    oldOptions,
                    regradeOption
                ));

                if (!regradedForms.ContainsKey(regradeAttempt.FormIdentifier))
                    regradedForms.Add(regradeAttempt.FormIdentifier, regradeAttempt.Form);
            }

            foreach (var regradedForm in regradedForms.Values)
                commands.Add(new AnalyzeForm(regradedForm.BankIdentifier, regradedForm.FormIdentifier));
        }

        private List<QAttempt> GetRegradeAttempts(Question question)
        {
            if (question.Set.Bank.IsLocked)
                return null;

            var hasChangedPoints = false;

            var optionCommands = ((OptionWriteRepeater)Options.GetControl())?.GetCommands(question);
            if (optionCommands != null)
                foreach (var optionCommand in optionCommands)
                {
                    var changeOption = optionCommand as ChangeOption;
                    if (changeOption != null)
                    {
                        var option = question.Options.Find(x => x.Number == changeOption.Number);
                        if (option.IsTrue != changeOption.IsTrue || option.Points != changeOption.Points)
                        {
                            hasChangedPoints = true;
                            break;
                        }
                    }
                }

            if (!hasChangedPoints)
                return null;

            var attemptFilter = new QAttemptFilter
            {
                QuestionIdentifier = question.Identifier
            };

            var attempts = ServiceLocator.AttemptSearch.GetAttempts(attemptFilter, x => x.Form);

            return attempts.Count > 0 ? attempts : null;
        }

        private void GetGradeItemCommands(List<Command> commands)
        {
            var forms = GradeItems.BindControlsToModel();

            foreach (var (formId, likertRowId, gradeItemId) in forms)
            {
                if (likertRowId == null)
                    commands.Add(new ChangeQuestionGradeItem2(BankID, formId, QuestionID, gradeItemId));
                else
                    commands.Add(new ChangeQuestionLikertRowGradeItem(BankID, formId, QuestionID, likertRowId.Value, gradeItemId));
            }
        }

        #endregion

        #region Loose Competency Selector

        private void BindLooseSubCompetencies(Guid parentId)
        {
            if (!Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection)
                return;

            CompetenyInfo[] items = null;

            if (parentId != Guid.Empty)
                items = StandardSearch.Bind(
                    CompetenyInfo.Binder,
                    x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.ParentStandardIdentifier == parentId,
                    "AssetNumber,ContentTitle");

            LooseSubCompetencyRepeater.Visible = items.IsNotEmpty();
            LooseSubCompetencyRepeater.DataSource = items;
            LooseSubCompetencyRepeater.DataBind();
        }

        private Guid[] GetLooseSubCompetencyIdentifiers()
        {
            if (!Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection)
                return null;

            var result = new List<Guid>();

            foreach (RepeaterItem subItem in LooseSubCompetencyRepeater.Items)
            {
                var isSubSelected = (ICheckBox)subItem.FindControl("IsSelected");
                if (isSubSelected.Checked)
                    result.Add(Guid.Parse(isSubSelected.Value));
            }

            return result.NullIfEmpty()?.ToArray();
        }

        private void SetLooseSubCompetencyIdentifiers(Guid[] values)
        {
            if (!Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection)
                return;

            var selected = !values.IsEmpty()
                ? values.Select(x => x.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : null;

            foreach (RepeaterItem subItem in LooseSubCompetencyRepeater.Items)
            {
                var isSubSelected = (ICheckBox)subItem.FindControl("IsSelected");
                isSubSelected.Checked = selected != null && selected.Contains(isSubSelected.Value);
            }
        }

        #endregion

        #region Strict Competency Selector

        private void BindStrictCompetencies(Set set)
        {
            var items = StandardSearch.Bind(
                CompetenyInfo.Binder,
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Parent.StandardIdentifier == set.Standard,
                "AssetNumber,ContentTitle");

            if (items.Length > 0 && Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection)
            {
                var subItems = StandardSearch
                    .Bind(
                        CompetenyInfo.Binder,
                        x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.Parent.Parent.StandardIdentifier == set.Standard,
                        "AssetNumber,ContentTitle");

                foreach (var item in items)
                    item.Children = subItems.Where(x => x.ParentStandardIdentifier == item.StandardIdentifier).ToArray();
            }

            CompetencyPanel.Visible = items.Length > 0;
            NoCompetencyPanel.Visible = !CompetencyPanel.Visible;

            StrictCompetencyRepeater.DataSource = items;
            StrictCompetencyRepeater.DataBind();
        }

        private Guid? GetStrictCompetencyIdentifier()
        {
            foreach (RepeaterItem item in StrictCompetencyRepeater.Items)
            {
                var isSelected = (IRadioButton)item.FindControl("IsSelected");
                if (isSelected.Checked)
                    return Guid.Parse(isSelected.Value);
            }

            return null;
        }

        private void SetStrictCompetencyIdentifier(Guid? value)
        {
            var strValue = value?.ToString();

            foreach (RepeaterItem item in StrictCompetencyRepeater.Items)
            {
                var isSelected = (IRadioButton)item.FindControl("IsSelected");
                isSelected.Checked = strValue != null && string.Equals(strValue, isSelected.Value, StringComparison.OrdinalIgnoreCase);
            }
        }

        private Guid[] GetStrictSubCompetencyIdentifiers()
        {
            if (!Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection)
                return null;

            var result = new List<Guid>();

            foreach (RepeaterItem item in StrictCompetencyRepeater.Items)
            {
                var isSelected = (IRadioButton)item.FindControl("IsSelected");
                if (!isSelected.Checked)
                    continue;

                var subRepeater = (Repeater)item.FindControl("SubCompetencyRepeater");
                foreach (RepeaterItem subItem in subRepeater.Items)
                {
                    var isSubSelected = (ICheckBox)subItem.FindControl("IsSelected");
                    if (isSubSelected.Checked)
                        result.Add(Guid.Parse(isSubSelected.Value));
                }

                break;
            }

            return result.NullIfEmpty()?.ToArray();
        }

        private void SetStrictSubCompetencyIdentifiers(Guid[] values)
        {
            if (Organization.Toolkits.Assessments.DisableStrictQuestionCompetencySelection)
                return;

            var selected = Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection && !values.IsEmpty()
                ? values.Select(x => x.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : null;

            foreach (RepeaterItem item in StrictCompetencyRepeater.Items)
            {
                var isSelected = (IRadioButton)item.FindControl("IsSelected");

                var subRepeater = (Repeater)item.FindControl("SubCompetencyRepeater");
                foreach (RepeaterItem subItem in subRepeater.Items)
                {
                    var isSubSelected = (ICheckBox)subItem.FindControl("IsSelected");
                    isSubSelected.Checked = isSelected.Checked
                        && selected != null
                        && selected.Contains(isSubSelected.Value);
                }
            }
        }

        #endregion
    }
}