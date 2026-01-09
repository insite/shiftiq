using System;
using System.Collections.Generic;
using System.Linq;

using Humanizer;

using InSite.Application.Standards.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class Detail : BaseUserControl
    {
        #region Classes

        public class RegistrationFile
        {
            public string[][] Lines { get; set; }
            public int StartIndex { get; set; }
        }

        public class RegionInfo
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public bool IsApplicable { get; set; }
        }

        private class AssetTreeInfo
        {
            public int ToAssetID { get; set; }
        }

        #endregion

        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        private int ContentItemsCount
        {
            get => (int)(ViewState[nameof(ContentItemsCount)] ?? 0);
            set => ViewState[nameof(ContentItemsCount)] = value;
        }

        public int OutgoingContainmentEdgeCount => ContainmentChildren.EdgeCount;

        public int OutgoingContainmentNotEdgeCount => ContainmentChildren.NotEdgeCount;

        #endregion

        #region Initialization and PreRender

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CompetenciesPanel.Updated += CompetenciesPanel_Updated;
            ContainmentChildren.Refreshed += ContainmentChildren_Refreshed;
            ContainmentParents.Refreshed += ContainmentParents_Refreshed;
            ConnectionTo.Refreshed += ConnectionTo_Refreshed;
            ConnectionFrom.Refreshed += ConnectionFrom_Refreshed;

            CompetencyScoreCalculationMethod.AutoPostBack = true;
            CompetencyScoreCalculationMethod.ValueChanged += CompetencyScoreCalculationMethod_ValueChanged;

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += DepartmentIdentifierMethod_ValueChanged;

            ScenarioQuestions.Alert += (x, y) => OnAlert(y.Type, y.Text);
        }

        protected override void CreateChildControls()
        {
            if (ContentNavigation.ItemsCount == 0 && ContentItemsCount > 0)
            {
                for (var i = 0; i < ContentItemsCount; i++)
                    AddContentNavigationNavItem(out _, out _);
            }

            base.CreateChildControls();
        }

        #endregion

        #region Event handlers

        private void CompetenciesPanel_Updated(object sender, EventArgs e)
        {
            ContainmentChildren.Refresh();
            SetRelationshipTitles();
        }

        private void ContainmentChildren_Refreshed(object sender, EventArgs e)
        {
            SetRelationshipTitles();
            CompetenciesPanel.Refresh();
        }

        private void ContainmentParents_Refreshed(object sender, EventArgs e)
        {
            SetRelationshipTitles();
        }

        private void ConnectionTo_Refreshed(object sender, EventArgs e)
        {
            SetRelationshipTitles();
        }

        private void ConnectionFrom_Refreshed(object sender, EventArgs e)
        {
            SetRelationshipTitles();
        }

        private void CompetencyScoreCalculationMethod_ValueChanged(object sender, EventArgs e)
        {
            CalculationArgumentField.Visible = StringHelper.EqualsAny(CompetencyScoreCalculationMethod.Value, new[] { "NumberOfTimes", "DecayingAverage" });
        }

        private void DepartmentIdentifierMethod_ValueChanged(object sender, EventArgs e)
        {
            RefreshIndustryIdentifier(DepartmentIdentifier.ValueAsGuid);
        }

        private void RefreshIndustryIdentifier(Guid? department)
        {
            IndustryIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            IndustryIdentifier.ListFilter.GroupIdentifier = department;
            IndustryIdentifier.ListFilter.CollectionName = CollectionName.Contacts_Settings_Industries_Name;
            IndustryIdentifier.RefreshData();
        }

        #endregion

        #region Methods (data binding)

        public void GetInputValues(QStandard asset)
        {
            asset.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            // Standard Details

            asset.ParentStandardIdentifier = ParentAssetID.Value;
            asset.StandardType = StandardType.Value;
            asset.StandardLabel = AssetLabel.Text;
            asset.StandardTier = AssetTier.Text;
            asset.SourceDescriptor = SourceDescriptor.Text;
            asset.Code = Code.Text;
            asset.StandardHook = Hook.Text;
            asset.StandardAlias = Alias.Text;
            asset.DepartmentGroupIdentifier = DepartmentIdentifier.ValueAsGuid;
            asset.IndustryItemIdentifier = IndustryIdentifier.ValueAsGuid;
            asset.ContentName = ContentName.Text;
            asset.ContentTitle = ContentTitleEn.Text;
            asset.AuthorName = AuthorName.Text;
            asset.AuthorDate = AuthorDate.Value;
            asset.IsPractical = IsPractical.Checked;
            asset.IsTheory = IsTheory.Checked;
            asset.CompetencyScoreSummarizationMethod = CompetencyScoreSummarizationMethod.Value.NullIfEmpty();
            asset.CompetencyScoreCalculationMethod = CompetencyScoreCalculationMethod.Value.NullIfEmpty();
            asset.CalculationArgument = CalculationArgument.ValueAsInt;
            asset.PointsPossible = PointsPossible.ValueAsDecimal;
            asset.MasteryPoints = MasteryPoints.ValueAsDecimal;

            if (!asset.DatePosted.HasValue)
                asset.DatePosted = DateTimeOffset.UtcNow;

            // Standard Settings

            asset.Sequence = Sequence.ValueAsInt.Value;
            asset.LevelType = LevelType.Text;
            asset.LevelCode = LevelCode.Text;
            asset.MajorVersion = MajorVersion.Text;
            asset.MinorVersion = MinorVersion.Text;
            asset.StandardIdentifier = Guid.Parse(Thumbprint.Text);
            asset.ContentDescription = DescriptionEn.Text;
            asset.CreditIdentifier = CreditIdentifier.Text;
            asset.CreditHours = CreditHours.ValueAsDecimal;
            asset.PassingScore = PassingScore.ValueAsDecimal / 100m;

            var tags = OrganizationTags.SaveData();
            asset.Tags = tags.Count > 0 ? JsonConvert.SerializeObject(tags) : null;
        }

        public void SetInputValues(Standard standard, string tab, string panel, Guid? outlineRootId, bool canEdit, bool canDelete)
        {
            if (standard.Parent != null)
            {
                ShowParentInfoButton.Visible = true;
                ShowParentInfoButton.OnClientClick = $"assetDetail.showAssetInfo({standard.Parent.AssetNumber}); return false;";
            }
            else
            {
                ShowParentInfoButton.Visible = false;
            }

            UpdateStatusLink.NavigateUrl = $"/ui/admin/standards/edit/status?standard={standard.StandardIdentifier}";
            UpdateStatusLink.Visible = TCollectionItemCache.Exists(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                CollectionName = CollectionName.Standards_Classification_Status
            });

            OutlineLink.Visible = outlineRootId.HasValue;
            OutlineLink.NavigateUrl = $"/ui/admin/standards/manage?standard={outlineRootId}";

            ClassifyLink.NavigateUrl = $"/ui/admin/standards/classify?asset={standard.StandardIdentifier}";
            ClassifyLink.Visible = canEdit;

            DownloadLink.NavigateUrl = $"/ui/admin/standards/download?asset={standard.StandardIdentifier}";

            DeleteLink.NavigateUrl = $"/admin/standards/delete?asset={standard.StandardIdentifier}";
            DeleteLink.Visible = canDelete;

            SetDetails(standard);

            SetSettings(standard);

            SetCategories(standard);

            var content = SetContent(standard, panel, tab);

            SetCompetencies(standard, canEdit);

            SetRelationships(standard, panel, canEdit);

            SetGlossaryTerms(standard, content, canEdit);

            SetScenarioQuestions(standard, panel, canEdit, canDelete);
        }

        private void SetDetails(Standard standard)
        {
            ParentAssetID.Value = standard.ParentStandardIdentifier;

            StandardType.Value = standard.StandardType;
            AssetLabel.Text = standard.StandardLabel;
            AssetTier.Text = standard.StandardTier;
            SourceDescriptor.Text = standard.SourceDescriptor;
            Code.Text = standard.Code;
            Hook.Text = standard.StandardHook;
            Alias.Text = standard.StandardAlias;

            DepartmentIdentifier.ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            DepartmentIdentifier.ListFilter.GroupType = "Department";
            DepartmentIdentifier.EnsureDataBound();
            DepartmentIdentifier.ValueAsGuid = standard.DepartmentGroupIdentifier;
            if (standard.DepartmentGroupIdentifier.HasValue)
            {
                RefreshIndustryIdentifier(standard.DepartmentGroupIdentifier);
                IndustryIdentifier.ValueAsGuid = standard.IndustryItemIdentifier;
            }

            ContentName.Text = standard.ContentName;
            ContentTitleEn.Text = standard.ContentTitle;

            StandardIdentifier.Text = standard.StandardIdentifier.ToString();

            AuthorName.Text = standard.AuthorName;
            AuthorDate.Value = standard.AuthorDate ?? DateTime.Today;
            DatePosted.Value = standard.DatePosted ?? DateTime.Today;

            IsPractical.Checked = standard.IsPractical;
            IsTheory.Checked = standard.IsTheory;

            CompetencyScoreSummarizationMethod.Value = standard.CompetencyScoreSummarizationMethod;
            CompetencyScoreCalculationMethod.Value = standard.CompetencyScoreCalculationMethod;
            CalculationArgument.ValueAsInt = standard.CalculationArgument;
            CalculationArgumentField.Visible = StringHelper.EqualsAny(CompetencyScoreCalculationMethod.Value, new[] { "NumberOfTimes", "DecayingAverage" });
            PointsPossible.ValueAsDecimal = standard.PointsPossible;
            MasteryPoints.ValueAsDecimal = standard.MasteryPoints;
        }

        private void SetSettings(Standard standard)
        {
            Sequence.ValueAsInt = standard.Sequence;
            LevelType.Text = standard.LevelType;
            LevelCode.Text = standard.LevelCode;
            MajorVersion.Text = standard.MajorVersion;
            MinorVersion.Text = standard.MinorVersion;
            Thumbprint.Text = standard.StandardIdentifier.ToString();

            var hasCanvasAccess = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Integrations_Canvas);
            CanvasIdentifierField.Visible = hasCanvasAccess;
            CanvasIdentifier.Text = string.IsNullOrEmpty(standard.CanvasIdentifier) ? "N/A" : standard.CanvasIdentifier;

            DescriptionEn.Text = standard.ContentDescription;
            CreditIdentifier.Text = standard.CreditIdentifier;
            CreditHours.ValueAsDecimal = standard.CreditHours;
            PassingScore.ValueAsDecimal = standard.PassingScore * 100m;

            var organization = OrganizationSearch.Select(standard.OrganizationIdentifier);
            var tags = !string.IsNullOrEmpty(standard.Tags) ? JsonConvert.DeserializeObject<List<Tuple<string, List<string>>>>(standard.Tags) : null;

            NoOrganizationTags.Visible = !OrganizationTags.LoadData(organization, tags);
        }

        private void SetCategories(Standard standard)
        {
            CategoriesTab.Visible = CurrentSessionState.Identity.Organization.Toolkits?.Standards?.ShowStandardCategories ?? false;
            AssetCategories.LoadData(standard);
        }

        private ContentContainer SetContent(Standard asset, string panel, string tab)
        {
            ContentItemsCount = 0;
            ContentNavigation.ClearItems();
            ContentTab.Visible = false;

            if (asset.DocumentType != null)
                return null;

            var fields = CurrentSessionState.Identity.Organization.GetStandardContentLabels();
            if (fields.Length == 0)
                return null;

            ContentTab.Visible = true;

            if (panel == "content")
                ContentTab.IsSelected = true;
            else
                tab = null;

            var content = ServiceLocator.ContentSearch.GetBlock(asset.StandardIdentifier, labels: fields);
            var canWrite = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards, PermissionOperation.Write);

            foreach (var name in fields)
            {
                AddContentNavigationNavItem(out var navItem, out var ctrl);

                navItem.Title = name;

                if (string.Equals(tab, name, StringComparison.OrdinalIgnoreCase))
                    navItem.IsSelected = true;

                var contentOutput = (ContentOutput)ctrl.LoadControl("~/UI/Admin/Standards/Standards/Controls/ContentOutput.ascx");
                contentOutput.Setup(asset.StandardIdentifier, name, content[name], canWrite);

                ContentItemsCount++;
            }

            return content;
        }

        private void SetCompetencies(Standard standard, bool canEdit)
        {
            var showCompetenciesPanel = standard.StandardType == Shift.Constant.StandardType.Profile
                || standard.StandardType == Shift.Constant.StandardType.Document;

            CompetenciesTab.Visible = showCompetenciesPanel;

            if (showCompetenciesPanel)
            {
                CompetenciesPanel.SetInputValues(standard);
                CompetenciesPanel.SetAssetType(standard.StandardType);
            }

            ContainmentParents.LoadData(standard, true, ConnectionDirection.Incoming, canEdit);
            ContainmentParentsField.Visible = ContainmentParents.ItemCount > 0;

            ContainmentChildren.LoadData(standard, true, ConnectionDirection.Outgoing, canEdit);
        }

        private void SetRelationships(Standard standard, string panel, bool canEdit)
        {
            ConnectionFrom.LoadData(standard, false, ConnectionDirection.Incoming, canEdit);
            ConnectionFromField.Visible = ConnectionFrom.ItemCount > 0;

            ConnectionTo.LoadData(standard, false, ConnectionDirection.Outgoing, canEdit);

            SetRelationshipTitles();

            if (panel == "relationships")
            {
                RelationshipsTab.IsSelected = true;
                ContainmentsTab.IsSelected = true;
            }
        }

        private void SetRelationshipTitles()
        {
            ContainmentsTab.SetTitle(
                "Containments",
                "Parent".ToQuantity(ContainmentParents.ItemCount) + ", " + "Child".ToQuantity(ContainmentChildren.ItemCount));
            ConnectionsTab.SetTitle(
                "Connections",
                "Upstream".ToQuantity(ConnectionFrom.ItemCount) + ", " + "Downstream".ToQuantity(ConnectionTo.ItemCount));
        }

        private void SetGlossaryTerms(Standard standard, ContentContainer content, bool canEdit)
        {
            var hasContent = content != null;

            GlossaryTermTab.Visible = hasContent;

            if (!hasContent)
                return;

            GlossaryTermGrid.LoadData(
                standard.StandardIdentifier,
                "Standard",
                content.GetItems().SelectMany(x => x.GetStrings()),
                ContentLabel.Title,
                canEdit);
        }

        private void SetScenarioQuestions(Standard standard, string panel, bool canEdit, bool canDelete)
        {
            var enabled = standard.StandardType == Shift.Constant.StandardType.Scenario
                && standard.Parent?.StandardType == Shift.Constant.StandardType.Blueprint;

            ScenarioQuesionsTab.Visible = enabled;

            if (!enabled)
                return;

            Guid? selectedQuestionId = null;

            if (panel == "questions")
            {
                ScenarioQuesionsTab.IsSelected = true;

                if (Guid.TryParse(Page.Request.QueryString["question"], out var questionId))
                    selectedQuestionId = questionId;
            }

            ScenarioQuestions.SetInputValues(standard, canEdit && canDelete, "id&panel=questions", selectedQuestionId);
            ScenarioQuesionsTab.SetTitle("Questions", ScenarioQuestions.QuestionsCount);
        }

        #endregion

        #region Helpers

        private void AddContentNavigationNavItem(out NavItem navItem, out DynamicControl ctrl)
        {
            ContentNavigation.AddItem(navItem = new NavItem());
            navItem.Controls.Add(ctrl = new DynamicControl());
        }

        #endregion
    }
}