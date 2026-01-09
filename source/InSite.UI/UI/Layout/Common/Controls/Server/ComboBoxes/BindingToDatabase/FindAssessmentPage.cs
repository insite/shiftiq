using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Sites.Read;

namespace InSite.Common.Web.UI
{
    public class FindAssessmentPage : BaseFindEntity<VAssessmentPageFilter>
    {
        public VAssessmentPageFilter Filter => (VAssessmentPageFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new VAssessmentPageFilter 
                { 
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                    PageIsHidden = false,
                }));

        protected override string GetEntityName() => "Assessment";

        protected override VAssessmentPageFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.FormName = keyword;

            return filter;
        }

        protected override int Count(VAssessmentPageFilter filter)
        {
            return ServiceLocator.PageSearch.Count(filter);
        }

        protected override DataItem[] Select(VAssessmentPageFilter filter)
        {
            filter.OrderBy = nameof(VAssessmentPage.FormName);

            return ServiceLocator.PageSearch.Select(filter).Select(GetDataItem).ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.PageSearch.GetAssessmentPages(ids).Select(GetDataItem).ToArray();
        }

        private static DataItem GetDataItem(VAssessmentPage x) => new DataItem
        {
            Value = x.FormIdentifier,
            Text = x.FormName,
        };
    }
}
