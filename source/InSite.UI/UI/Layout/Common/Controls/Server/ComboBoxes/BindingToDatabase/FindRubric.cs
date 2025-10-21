using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Common.Web.UI
{
    public class FindRubric : BaseFindEntity<QRubricFilter>
    {
        public QRubricFilter Filter => (QRubricFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QRubricFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier }));

        protected override string GetEntityName() => "Rubric";

        protected override QRubricFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.RubricTitle = keyword;

            return filter;
        }

        protected override int Count(QRubricFilter filter)
        {
            return ServiceLocator.RubricSearch.CountRubrics(filter);
        }

        protected override DataItem[] Select(QRubricFilter filter)
        {
            filter.OrderBy = nameof(QRubric.RubricTitle);

            return ServiceLocator.RubricSearch.GetRubrics(filter)
                .Select(x => new DataItem
                {
                    Value = x.RubricIdentifier,
                    Text = x.RubricTitle
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.RubricSearch.GetRubrics(ids)
                .Select(x => new DataItem
                {
                    Value = x.RubricIdentifier,
                    Text = x.RubricTitle
                });
        }
    }
}