using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class CaseCommentCategoryComboBox : ComboBox
    {
        public TCollectionItemFilter ListFilter => (TCollectionItemFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new TCollectionItemFilter()));

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            ListFilter.CollectionName = CollectionName.Cases_Comments_Categorization_Name;

            var items = TCollectionItemCache.Select(ListFilter)
                .Select(x => new ListItem
                {
                    Text = x.ItemName.ToString(),
                    Value = x.ItemName.ToString(),
                })
                .ToList();

            return new ListItemArray(items);
        }
    }
}