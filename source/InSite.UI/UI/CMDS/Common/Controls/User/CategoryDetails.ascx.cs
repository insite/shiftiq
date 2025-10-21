using InSite.Persistence;

namespace InSite.Cmds.Controls.Contacts.Categories
{
    public partial class CategoryDetails : System.Web.UI.UserControl
    {
        public void SetInputValues(VAchievementCategory category)
        {
            if (category == null)
            {
                Visible = false;
                return;
            }

            CategoryName.Text = category.CategoryName;
            AchievementType.Value = category.AchievementLabel;
        }

        public void GetInputValues(VAchievementCategory category)
        {
            category.CategoryName = CategoryName.Text.Trim();
            category.AchievementLabel = AchievementType.Value;
        }
    }
}