using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class OccupationComboBox : ComboBox
    {
        #region Properties
        public StandardFilter ListFilter => (StandardFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new StandardFilter()));

        #endregion

        #region Data binding

        protected override ListItemArray CreateDataSource()
        {
            var data = StandardSearch
                .Select(ListFilter).ToArray()
                    .Select(x => new ListItem
                    {
                        Value = x.StandardIdentifier.ToString(),
                        Text = x.ContentName ?? x.ContentTitle
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

            return new ListItemArray(data);
        }

        #endregion
    }

    public class OccupationParentComboBox : ComboBox
    {
        public StandardFilter ListFilter => (StandardFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new StandardFilter()));

        protected override ListItemArray CreateDataSource()
        {
            var parents = StandardSearch
                .Bind(x => x.Parent, ListFilter)
                .Where(x => x != null)
                .Select(x => new ListItem
                {
                    Value = x.StandardIdentifier.ToString(),
                    Text = x.ContentName ?? x.ContentTitle
                })
                .OrderBy(x => x.Text)
                .ToArray();

            var ids = parents.Select(x => x.Value).Distinct();
            var list = new ListItemArray();
            foreach (var id in ids)
            {
                if (!list.Items.Any(i => i.Value == id))
                    list.Add(new ListItem { Value = id, Text = parents.FirstOrDefault(x => x.Value == id).Text });
            }
            return new ListItemArray(list);
        }
    }

    public class OccupationListComboBox : ComboBox
    {
        public StandardFilter ListFilter => (StandardFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new StandardFilter()));

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.StandardLabel = "Occupation List";
            ListFilter.DepartmentGroupIdentifier = null;

            var array = StandardSearch
                .Bind(x => new { Identifier = x.StandardIdentifier, Name = x.ContentName, Title = x.ContentTitle } , ListFilter)
                .Select(x => new ListItem
                {
                    Value = x.Identifier.ToString(),
                    Text = x.Title ?? x.Name
                })
                .OrderBy(x => x.Text)
                .ToArray();

            return new ListItemArray(array);
        }
    }
}