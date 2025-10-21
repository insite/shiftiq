using System.ComponentModel;
using System.Linq;

using InSite.Admin.Assessments.Attempts.Models;
using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class AdHocResults : SearchResultsGridViewController<QAttemptFilter>
    {
        #region Properties

        public AdHocAttemptDataItem[] DataItems
        {
            get => (AdHocAttemptDataItem[])ViewState[nameof(DataItems)];
            set => ViewState[nameof(DataItems)] = value;
        }

        #endregion

        #region Event handlers

        protected override void Search(QAttemptFilter filter, int pageIndex)
        {
            if (!IsPostBack)
            {
                Filter = filter;
                SetGridVisibility(false, false);
                OnSearched();
            }
            else
            {
                base.Search(filter, pageIndex);
            }
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(QAttemptFilter filter)
        {
            return DataItems?.Length ?? 0;
        }

        protected override IListSource SelectData(QAttemptFilter filter)
        {
            if (DataItems == null)
                return null;

            return DataItems
                .ApplyPaging(filter)
                .AsQueryable()
                .ToSearchResult();
        }

        protected string GetFormAssetVersion()
        {
            var attempt = (AdHocAttemptDataItem)Page.GetDataItem();
            if (!attempt.FormAsset.HasValue)
                return "(n/a)";

            var version = attempt.FormAssetVersion;
            if (attempt.FormFirstPublished.HasValue && attempt.AttemptStartedValue.HasValue && attempt.AttemptStartedValue.Value < attempt.FormFirstPublished.Value)
                version = 0;

            return version.ToString();
        }

        #endregion
    }
}