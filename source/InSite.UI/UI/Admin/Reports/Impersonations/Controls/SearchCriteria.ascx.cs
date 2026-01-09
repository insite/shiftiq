using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Reports.Impersonations.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<ImpersonationFilter>
    {
        #region Properties

        public override ImpersonationFilter Filter
        {
            get
            {
                var filter = new ImpersonationFilter()
                {
                    SinceDate = SinceDate.Value?.UtcDateTime,
                    BeforeDate = BeforeDate.Value?.UtcDateTime
                };

                return filter;
            }
            set
            {
                SinceDate.Value = value.SinceDate;
                BeforeDate.Value = value.BeforeDate;
            }
        }

        #endregion

        #region Operations

        public override void Clear()
        {
            SinceDate.Value = null;
            BeforeDate.Value = null;
        }

        #endregion
    }
}