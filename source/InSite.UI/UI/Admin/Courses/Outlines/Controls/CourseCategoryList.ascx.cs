using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Courses.Outlines.Controls
{
    public partial class CourseCategoryList : BaseUserControl
    {
        private Guid? CourseId
        {
            get => (Guid?)ViewState[nameof(CourseId)];
            set => ViewState[nameof(CourseId)] = value;
        }

        private string AchievementLabel
        {
            get => (string)ViewState[nameof(AchievementLabel)];
            set => ViewState[nameof(AchievementLabel)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            CompanyName.Text = Organization.CompanyName;
        }

        public void SetCourse(Guid courseId, bool bind = true)
        {
            CourseId = courseId;

            if (bind)
                LoadData();
        }

        public void SetAchievement(string achievementLabel, bool bind = true)
        {
            AchievementLabel = achievementLabel;

            if (bind)
                LoadData();
        }

        private void LoadData()
        {
            List<TCollectionItem> items = null;
            HashSet<Guid> selections = null;

            if (CourseId.HasValue && AchievementLabel.IsNotEmpty())
            {
                items = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionName = CollectionName.Learning_Catalogs_Category_Name,
                    OrganizationIdentifier = Organization.Identifier,
                    OrderBy = nameof(TCollectionItem.ItemFolder) + "," + nameof(TCollectionItem.ItemName)
                });
                selections = CourseSearch.BindCourseCategories(x => x.ItemIdentifier, x => x.CourseIdentifier == CourseId.Value).ToHashSet();
            }

            FolderRepeater.DataSource = items?.GroupBy(x => x.ItemFolder.IfNullOrEmpty("None")).Select(x => new
            {
                FolderName = x.Key,
                Items = x,
                Selections = selections
            });
            FolderRepeater.ItemDataBound += FolderRepeater_ItemDataBound;
            FolderRepeater.DataBind();

            NoneLiteral.Visible = items.IsEmpty();
        }

        public void SaveData()
        {
            if (!CourseId.HasValue || AchievementLabel.IsEmpty())
                return;

            var selectedCategories = new HashSet<Guid>();
            foreach (RepeaterItem folderItem in FolderRepeater.Items)
            {
                var list = (Common.Web.UI.CheckBoxList)folderItem.FindControl("CategoryList");
                foreach (System.Web.UI.WebControls.ListItem checkBoxItem in list.Items)
                {
                    if (checkBoxItem.Selected)
                        selectedCategories.Add(Guid.Parse(checkBoxItem.Value));
                }
            }

            var existCategories = CourseSearch.SelectCourseCategories(x => x.CourseIdentifier == CourseId.Value);
            var deleteList = new List<TCourseCategory>();
            var insertList = new List<TCourseCategory>();

            foreach (var category in existCategories)
            {
                if (!selectedCategories.Contains(category.ItemIdentifier))
                    deleteList.Add(category);
                else
                    selectedCategories.Remove(category.ItemIdentifier);
            }

            foreach (var categoryId in selectedCategories)
            {
                insertList.Add(new TCourseCategory
                {
                    CourseIdentifier = CourseId.Value,
                    ItemIdentifier = categoryId,
                    OrganizationIdentifier = Organization.Identifier
                });
            }

            Course2Store.InsertCategory(insertList);
            Course2Store.DeleteCategory(deleteList);
        }

        private void FolderRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var items = (IEnumerable<TCollectionItem>)DataBinder.Eval(e.Item.DataItem, "Items");
            var selections = (ICollection<Guid>)DataBinder.Eval(e.Item.DataItem, "Selections");

            var categoryList = (Common.Web.UI.CheckBoxList)e.Item.FindControl("CategoryList");

            categoryList.Items.Clear();

            foreach (var item in items)
            {
                categoryList.Items.Add(new System.Web.UI.WebControls.ListItem()
                {
                    Text = item.ItemName,
                    Value = item.ItemIdentifier.ToString(),
                    Selected = selections.Contains(item.ItemIdentifier)
                });
            }
        }
    }
}