using InSite.Persistence;

using Shift.Common;

using ListItem = Shift.Common.ListItem;

namespace InSite.Common.Web.UI
{
    public class FileTypeMultiComboBox : MultiComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var fileTypes = UploadSearch.SelectFileTypes(CurrentSessionState.Identity.Organization.OrganizationIdentifier);

            var list = new ListItemArray();

            foreach (var fileType in fileTypes)
            {
                list.Add(new ListItem
                {
                    Value = fileType,
                    Text = fileType
                });
            }

            return list;
        }
    }
}