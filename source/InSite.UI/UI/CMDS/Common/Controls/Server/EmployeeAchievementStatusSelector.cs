using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class EmployeeAchievementStatusSelector : ComboBox
    {
        /// <remarks>
        /// Submitted is not in the status collection, because it is normally set by the learner
        /// uploading evidence. It is added after Pending so the form always shows the real status
        /// and an administrator can move a credential into or out of Submitted from any state.
        /// </remarks>
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = OrganizationIdentifiers.CMDS,
                CollectionName = CollectionName.Achievements_Credentials_Validity_Status
            });

            var submitted = CredentialStatus.Submitted.ToString();

            foreach (var item in data)
            {
                list.Add(item.ItemName, item.ItemName);

                if (item.ItemName == CredentialStatus.Pending.ToString())
                    list.Add(submitted, submitted);
            }

            if (!list.ContainsValue(submitted))
                list.Add(submitted, submitted);

            return list;
        }
    }
}
