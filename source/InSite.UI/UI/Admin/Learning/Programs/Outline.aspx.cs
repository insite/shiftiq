using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Records.Programs
{
    public partial class Outline : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/outline";

        public static string GetNavigateUrl(Guid programId, string status = null, string tab = null, string subtab = null, string panel = null)
        {
            var url = NavigateUrl + "?id=" + programId;

            if (status.IsNotEmpty())
                url += "&status=" + HttpUtility.UrlEncode(status);

            if (tab.IsNotEmpty())
                url += "&tab=" + HttpUtility.UrlEncode(tab);

            if (subtab.IsNotEmpty())
                url += "&subtab=" + HttpUtility.UrlEncode(subtab);

            if (panel.IsNotEmpty())
                url += "&panel=" + HttpUtility.UrlEncode(panel);

            return url;
        }

        public static void Redirect(Guid programId, string status = null, string tab = null, string subtab = null, string panel = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(programId, status, tab, subtab, panel));

        private Guid? ProgramID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementListEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "program");

            AchievementListEditor.Refreshed += AchievementEditor_Refreshed;

            InitTab();
        }

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var programId = ProgramID.Value;

            var programTaskIds = TaskSearch.SelectByProgram(programId);

            var assignedAchievementIds = programTaskIds.Select(x => x.AchievementIdentifier).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        private void InitTab()
        {
            var url = new WebUrl(Request.RawUrl);
            var tab = url.QueryString["tab"];
            var isPublication = tab == "publication";

            SelectTab(CatalogTab, tab == "catalog");
            SelectTab(ContentTab, tab == "content");
            SelectTab(LearnerTab, tab == "enrollments");
            SelectTab(PublicationTab, isPublication);
            SelectTab(NotificationTab, tab == "notification");
            SelectTab(PrivacyTab, tab == "privacy");
            SelectTab(SettingsSection, tab == "settings");

            if (isPublication)
                PublicationSetup.SetTab(url.QueryString["subtab"]);

            void SelectTab(NavItem nav, bool selected)
            {
                if (selected)
                    nav.IsSelected = true;
            }
        }

        private void SelectTab(string tab)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ValidateQueryString();
            BindModelToControls(InitModel());
        }

        private void Task_Updated(object sender, EventArgs e)
        {
            PublicationSetup.Refresh();
        }

        private void BindModelToControls(TProgram model)
        {
            PageHelper.AutoBindHeader(this, qualifier: model.ProgramName);

            ProgramCode.Text = model.ProgramCode;
            ProgramCodeField.Visible = !string.IsNullOrEmpty(model.ProgramCode);

            if (model.GroupIdentifier != null)
            {
                var group = ServiceLocator.GroupSearch.GetGroup(model.GroupIdentifier.Value);
                if (group != null)
                {
                    GroupLabel.Text = group.GroupType;
                    GroupName.Text = group.GroupName;
                    GroupField.Visible = true;
                }
            }

            CatalogName.Text = "None";

            if (model.CatalogIdentifier != null)
            {
                var catalog = CourseSearch.GetCatalog(model.OrganizationIdentifier, model.CatalogIdentifier.Value);

                if (catalog != null)
                {
                    CatalogName.Text = catalog.CatalogName;

                    if (catalog.IsHidden)
                        CatalogName.Text += " <span class='badge bg-danger fs-sm ms-2'>Hidden</span>";
                }
            }

            ProgramName.Text = model.ProgramName;
            ProgramDescription.Text = model.ProgramDescription;
            ProgramIdentifier.Text = model.ProgramIdentifier.ToString();

            if (model.ProgramType != null)
            {
                ProgramType.Text = $"<span class='badge bg-primary fs-sm'>{model.ProgramType}</span>";
            }

            ProgramTag.Text = model.ProgramTag.HasValue() ? model.ProgramTag : "None";
            ProgramTagField.Visible = model.ProgramTag.HasValue();

            PrivacySettingRepeater.LoadData(model.ProgramIdentifier, "Program");
            ProgramContent.LoadData(model.ProgramIdentifier);
            NotificationSetup.LoadData(model);
            PublicationSetup.LoadData(model);

            RecodeLink.NavigateUrl =
                RetagLink.NavigateUrl =
                RenameLink.NavigateUrl =
                DescribeLink.NavigateUrl =
                ModifyGroupLink.NavigateUrl =
                $"/ui/admin/learning/programs/describe?id={model.ProgramIdentifier}";

            ModifyCatalogLink.NavigateUrl = ModifyCatalog.GetNavigateUrl(model.ProgramIdentifier);
            ModifySettingsLink.NavigateUrl = ModifySettings.GetNavigateUrl(model.ProgramIdentifier);

            BindModelToControlsForAchievement(model.AchievementIdentifier);
            BindModelToControlsForCategories(model.ProgramIdentifier);

            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(model.ProgramIdentifier, Outline.GetNavigateUrl(model.ProgramIdentifier));
            DeleteLink.NavigateUrl = $"/ui/admin/learning/programs/delete?id={model.ProgramIdentifier}";
            EditButton.NavigateUrl = $"/ui/admin/learning/programs/tasks/assign?id={model.ProgramIdentifier}";
            DuplicateLink.NavigateUrl = $"/ui/admin/learning/programs/duplicate?id={model.ProgramIdentifier}";
            EditAchievementLink.NavigateUrl = ModifyAchievement.GetNavigateUrl(model.ProgramIdentifier);

            ContactTabControl.LoadData(model.ProgramIdentifier, model.AchievementIdentifier);

            if (model.ProgramType == "Achievements Only")
            {
                AchievementTaskRepeater.BindModelToControls(model.ProgramIdentifier, true);

                AchievementTaskRepeater.Visible = true;
            }
            else
            {
                AssessmentTaskRepeater.BindModelToControls(model.ProgramIdentifier);
                LogbookTaskRepeater.BindModelToControls(model.ProgramIdentifier);
                SurveyTaskRepeater.BindModelToControls(model.ProgramIdentifier);
                CourseTaskRepeater.BindModelToControls(model.ProgramIdentifier);

                AssessmentTaskRepeater.Visible = LogbookTaskRepeater.Visible = SurveyTaskRepeater.Visible = CourseTaskRepeater.Visible = true;
            }

            CurrentDepartmentIdentifier = model.GroupIdentifier ?? Guid.Empty;

            if (model.ProgramType == "Achievements Only")
            {
                AchievementSection.Visible = true;
                SettingsSection.Visible = true;
                AchievementListEditor.SetEditable(true, true);
                AchievementListEditor.LoadAchievements(GroupByEnum.TypeAndCategory, model.GroupIdentifier);
                EditButton.Visible = false;
                TaskGrid.BindModelToControls(model.ProgramIdentifier);
            }
            else
            {
                CredentialGrid.LoadData(model.ProgramIdentifier);
                CredentialPanel.Visible = true;
            }
        }

        private void ValidateQueryString()
        {
            if (ProgramID == null)
                Search.Redirect();
        }

        private TProgram InitModel()
        {
            var model = ProgramSearch.GetProgram(ProgramID.Value);
            if (model == null)
                Search.Redirect();
            return model;
        }

        #region Program Categories

        public void BindModelToControlsForCategories(Guid programId)
        {
            List<TCollectionItem> items = null;

            var selections = CourseSearch.BindProgramCategories(x => x.ItemIdentifier, x => x.ProgramIdentifier == programId).ToHashSet();
            if (selections.Count > 0)
            {
                items = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionName = CollectionName.Learning_Catalogs_Category_Name,
                    OrganizationIdentifier = Organization.Identifier,
                    OrderBy = nameof(TCollectionItem.ItemFolder) + "," + nameof(TCollectionItem.ItemName)
                });
                items = items.Where(x => selections.Contains(x.ItemIdentifier)).ToList();
            }

            NoCatalogCategories.Visible = items.IsEmpty();

            CategoriesFolderRepeater.DataSource = items?.GroupBy(x => x.ItemFolder.IfNullOrEmpty("None")).Select(x => new
            {
                FolderName = x.Key,
                Items = x
            });
            CategoriesFolderRepeater.ItemDataBound += CategoriesFolderRepeater_ItemDataBound;
            CategoriesFolderRepeater.DataBind();
        }

        private void CategoriesFolderRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Items");
            itemRepeater.DataBind();
        }

        #endregion

        #region Program Achievement

        public void BindModelToControlsForAchievement(Guid? achievementId)
        {
            var achievement = achievementId.HasValue
                ? ServiceLocator.AchievementSearch.GetAchievement(achievementId.Value)
                : null;
            var hasAchievement = achievement != null;

            AchievementFields.Visible = hasAchievement;
            NoAchievementMessage.Visible = !hasAchievement;

            if (!hasAchievement)
                return;

            AchievementOutlineLink.NavigateUrl = $"/ui/admin/records/achievements/outline?id={achievementId}";
            AchievementTitle.Text = achievement.AchievementTitle;
            AchievementLabel.Text = achievement.AchievementLabel;
            AchievementLayout.Text = achievement.CertificateLayoutCode;
            AchievementLayoutField.Visible = achievement.CertificateLayoutCode.IsNotEmpty();

            var expirationType = achievement.ExpirationType.ToEnum(ExpirationType.None);
            if (expirationType == ExpirationType.Fixed && achievement.ExpirationFixedDate.HasValue)
                AchievementExpiration.Text = achievement.ExpirationFixedDate.Value.Format(User.TimeZone);
            else if (expirationType == ExpirationType.Relative && achievement.ExpirationLifetimeUnit.IsNotEmpty() && achievement.ExpirationLifetimeQuantity > 0)
                AchievementExpiration.Text = achievement.ExpirationLifetimeUnit.ToQuantity(achievement.ExpirationLifetimeQuantity.Value).ToLower();
            else
                AchievementExpiration.Text = "No Expiry";
        }

        #endregion

        #region Achievements Only

        private Guid CurrentDepartmentIdentifier
        {
            get => (Guid)ViewState[nameof(CurrentDepartmentIdentifier)];
            set => ViewState[nameof(CurrentDepartmentIdentifier)] = value;
        }

        private void AchievementEditor_Refreshed(object sender, EventArgs e)
        {
            TaskGrid.BindModelToControls(ProgramID.Value);
        }

        private List<AchievementListGridItem> SelectAchievements(Guid enterpriseId, Guid organizationId, string scope, string keyword)
        {
            return VCmdsAchievementSearch.SelectDepartmentTemplateAchievements(
                enterpriseId, organizationId, scope, keyword,
                ProgramID.Value);
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            TaskStore.Delete(ProgramID.Value, achievements);

            TaskGrid.BindModelToControls(ProgramID.Value);
        }

        private int InsertAchievements(IEnumerable<Guid> achievements)
        {
            var items = new List<TTask>();

            foreach (var id in achievements)
            {
                var entity = new TTask
                {
                    ProgramIdentifier = ProgramID.Value,
                    ObjectIdentifier = id,
                    ObjectType = "Achievement",
                    OrganizationIdentifier = Organization.Identifier,
                    TaskCompletionRequirement = "Credential Granted",
                    TaskIdentifier = UniqueIdentifier.Create()
                };

                items.Add(entity);
            }

            TaskStore.Insert(items);

            TaskGrid.BindModelToControls(ProgramID.Value);

            return items.Count;
        }

        #endregion
    }
}