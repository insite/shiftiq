using InSite.Persistence;

namespace InSite.UI.Admin.Assets.Collections.Controls
{
    public partial class CollectionInfo : System.Web.UI.UserControl
    {
        public void BindCollection(TCollection entity)
        {
            CollectionName.Text = $"<a href=\"/ui/admin/assets/collections/edit?collection={entity.CollectionIdentifier}\">{entity.CollectionName}</a>";
            Package.Text = !string.IsNullOrEmpty(entity.CollectionPackage) ? entity.CollectionPackage : "None";
            Process.Text = entity.CollectionProcess;
            ToolkitName.Text = entity.CollectionTool;
            Type.Text = entity.CollectionType;
            References.Text = !string.IsNullOrEmpty(entity.CollectionReferences) ? entity.CollectionReferences : "None";
        }
    }
}