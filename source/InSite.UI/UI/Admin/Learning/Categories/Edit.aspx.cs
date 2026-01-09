using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Learning.Categories
{
    public partial class Edit : AdminBasePage
    {
        public const string CollectionName = Shift.Constant.CollectionName.Learning_Catalogs_Category_Name;

        public static Guid? GetCollectionId()
        {
            return TCollectionSearch.BindFirst(
                x => (Guid?)x.CollectionIdentifier,
                new TCollectionFilter { CollectionName = CollectionName });
        }

        public const string NavigateUrl = "/ui/admin/learning/categories/edit";

        public static string GetNavigateUrl(Guid itemId) => NavigateUrl + "?category=" + itemId;

        public static void Redirect(Guid itemId) => HttpResponseHelper.Redirect(GetNavigateUrl(itemId));

        private Guid CategoryIdentifier
        {
            get => (Guid)ViewState[nameof(CategoryIdentifier)];
            set => ViewState[nameof(CategoryIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!Guid.TryParse(Request.QueryString["category"], out var categoryId))
                Search.Redirect();

            CategoryIdentifier = categoryId;

            AchievementsEditor.InitDelegates(
                Organization.Identifier,
                GetAssignedAchievements,
                DeleteAchievements,
                InsertAchievements,
                "category");

            CancelButton.NavigateUrl = Search.NavigateUrl;

            SaveButton.Click += SaveButton_Click;
        }

        private List<AchievementListGridItem> GetAssignedAchievements(List<AchievementListGridItem> list)
        {
            var categoryId = CategoryIdentifier;

            var categoryAchievementIds = VCmdsAchievementSearch.SelectCategoryAchievements(categoryId);

            var assignedAchievementIds = categoryAchievementIds.Select(x => x.AchievementIdentifier).ToList();

            return list
                .Where(x => assignedAchievementIds.Contains(x.AchievementIdentifier))
                .ToList();
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var entity = TCollectionItemSearch.Select(CategoryIdentifier);
            if (entity == null || entity.OrganizationIdentifier != Organization.OrganizationIdentifier)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, entity.ItemName);

            CategoryDetail.SetInputValues(entity);

            var display = $"## {entity.ItemName}" + Environment.NewLine + (entity.ItemDescription ?? "(No description)");

            DescriptionDisplay.Text = Markdown.ToHtml(display);

            var courses = CourseSearch.SelectCoursesByCategory(CategoryIdentifier);

            CourseRepeater.DataSource = courses;
            CourseRepeater.DataBind();

            var programs = ProgramSearch.SelectProgramsByCategory(CategoryIdentifier);

            ProgramRepeater.DataSource = programs;
            ProgramRepeater.DataBind();

            AchievementsEditor.SetEditable(true, true);

            AchievementsEditor.LoadAchievements();

            DeleteButton.NavigateUrl = Delete.GetNavigateUrl(CategoryIdentifier);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var item = TCollectionItemSearch.Select(CategoryIdentifier);

            CategoryDetail.GetInputValues(item);

            TCollectionItemStore.Update(item);

            TCollectionItemCache.Refresh();

            Search.Redirect();
        }

        private void DeleteAchievements(IEnumerable<Guid> achievements)
        {
            var achievementIdentifiers = VCmdsAchievementSearch.Bind(x => x.AchievementIdentifier, x => achievements.Contains(x.AchievementIdentifier));

            var list = TAchievementClassificationSearch.Select(x => x.CategoryIdentifier == CategoryIdentifier && achievementIdentifiers.Contains(x.AchievementIdentifier));

            TAchievementClassificationStore.Delete(list);
        }

        private Int32 InsertAchievements(IEnumerable<Guid> achievements)
        {
            var achievementIdentifiers = VCmdsAchievementSearch.Bind(x => x.AchievementIdentifier, x => achievements.Contains(x.AchievementIdentifier));

            var existenList = TAchievementClassificationSearch.Select(x => x.CategoryIdentifier == CategoryIdentifier);

            var insertList = new List<VAchievementClassification>();

            foreach (var achievementIdentifier in achievementIdentifiers)
            {
                if (existenList.FirstOrDefault(i => i.AchievementIdentifier == achievementIdentifier) == null)
                    insertList.Add(new VAchievementClassification { AchievementIdentifier = achievementIdentifier, CategoryIdentifier = CategoryIdentifier });
            }

            TAchievementClassificationStore.Insert(insertList);

            return insertList.Count;
        }
    }
}