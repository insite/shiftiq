using System;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class AchievementCategorySelector : ComboBox
    {
        public Guid? OrganizationIdentifier
        {
            get => ViewState[nameof(OrganizationIdentifier)] as Guid?;
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var organization = OrganizationIdentifier ?? CurrentSessionState.Identity.Organization.Identifier;
            var categories = VAchievementCategorySearch
                .Select(x => x.OrganizationIdentifier == organization)
                .OrderBy(x => x.CategoryName)
                .ToList();

            foreach (var category in categories)
                list.Add(category.CategoryIdentifier.ToString(), category.CategoryName);

            return list;
        }
    }
}
