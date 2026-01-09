using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Web.Integration;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class ScormRegistrationGrid : SearchResultsGridViewController<ScormRegistrationFilter>
    {
        protected override bool IsFinder => false;

        protected Guid? GradebookIdentifier => Filter?.GradebookIdentifier;

        public void LoadData(Guid id)
        {
            var filter = new ScormRegistrationFilter
            {
                GradebookIdentifier = id
            };

            Search(filter);
        }

        protected override int SelectCount(ScormRegistrationFilter filter)
        {
            var scormCloud = new ScormIntegrator(Organization, User, Guid.Empty);
            return scormCloud.GetRegistrations(filter.GradebookIdentifier.Value, null).Count();
        }

        protected override IListSource SelectData(ScormRegistrationFilter filter)
        {
            var scormCloud = new ScormIntegrator(Organization, User, Guid.Empty);
            var list = scormCloud.GetRegistrations(filter.GradebookIdentifier.Value, null)
                .OrderByDescending(x => x.ScormAccessedLast)
                .ApplyPaging(filter)
                .ToList();
            return list.ToSearchResult();
        }
    }
}