using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Reports.Queries.Controls
{
    public partial class SqlSearchCriteria : SearchCriteriaController<SqlFilter>
    {
        public override SqlFilter Filter
        {
            get
            {
                if (!Validate(Query.Text))
                    return null;

                var filter = new SqlFilter
                {
                    Query = Query.Text
                };

                return filter;
            }
            set
            {
                Query.Text = value?.Query;
            }
        }

        public override void Clear()
        {
            Query.Text = null;
        }

        private bool Validate(string query)
        {
            if (string.IsNullOrEmpty(query))
                return false;

            if (query.Contains(";"))
            {
                ErrorPanel.InnerHtml = "<strong><i class='fas fa-exclamation-square'></i> Access Denied.</strong> Your query cannot include any semicolon characters.";
                ErrorPanel.Visible = true;
                return false;
            }

            if (!StringHelper.StartsWith(query, "select"))
            {
                ErrorPanel.InnerHtml = "<strong><i class='fas fa-exclamation-square'></i> Access Denied.</strong> Your query must start with the SQL keyword <strong>SELECT</strong>.";
                ErrorPanel.Visible = true;
                return false;
            }

            return true;
        }
    }
}