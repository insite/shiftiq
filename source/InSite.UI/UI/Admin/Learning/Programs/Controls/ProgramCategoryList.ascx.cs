using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Application.Utility.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Learning.Programs.Controls
{
    public partial class ProgramCategoryList : BaseUserControl
    {
        private Guid? ProgramId
        {
            get => (Guid?)ViewState[nameof(ProgramId)];
            set => ViewState[nameof(ProgramId)] = value;
        }

        public void SetProgram(Guid programId, bool bind = true)
        {
            ProgramId = programId;

            if (bind)
                LoadData();
        }

        private void LoadData()
        {
            List<TCollectionItem> items = null;
            HashSet<Guid> selections = null;

            if (ProgramId.HasValue)
            {
                items = TCollectionItemSearch.Select(new TCollectionItemFilter
                {
                    CollectionName = CollectionName.Learning_Catalogs_Category_Name,
                    OrganizationIdentifier = Organization.Identifier,
                    OrderBy = nameof(TCollectionItem.ItemFolder) + "," + nameof(TCollectionItem.ItemName)
                });
                selections = CourseSearch.BindProgramCategories(x => x.ItemIdentifier, x => x.ProgramIdentifier == ProgramId.Value).ToHashSet();
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
            if (!ProgramId.HasValue)
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

            var existCategories = CourseSearch.SelectProgramCategories(x => x.ProgramIdentifier == ProgramId.Value);
            var deleteList = new List<TProgramCategory>();
            var insertList = new List<TProgramCategory>();

            foreach (var category in existCategories)
            {
                if (!selectedCategories.Contains(category.ItemIdentifier))
                    deleteList.Add(category);
                else
                    selectedCategories.Remove(category.ItemIdentifier);
            }

            foreach (var categoryId in selectedCategories)
            {
                insertList.Add(new TProgramCategory
                {
                    ProgramIdentifier = ProgramId.Value,
                    ItemIdentifier = categoryId,
                    OrganizationIdentifier = Organization.Identifier
                });
            }

            Course2Store.InsertProgramCategory(insertList);
            Course2Store.DeleteProgramCategory(deleteList);
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