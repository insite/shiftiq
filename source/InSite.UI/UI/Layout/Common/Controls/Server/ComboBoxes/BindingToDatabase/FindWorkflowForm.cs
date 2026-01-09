using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Surveys.Read;

namespace InSite.Common.Web.UI
{
    public class FindWorkflowForm : BaseFindEntity<QSurveyFormFilter>
    {
        #region Properties

        public QSurveyFormFilter Filter => (QSurveyFormFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QSurveyFormFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier }));

        #endregion

        protected override string GetEntityName() => "Form";

        protected override QSurveyFormFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Name = keyword;

            return filter;
        }

        protected override int Count(QSurveyFormFilter filter)
        {
            return ServiceLocator.SurveySearch.CountSurveyForms(filter);
        }

        protected override DataItem[] Select(QSurveyFormFilter filter)
        {
            return ServiceLocator.SurveySearch
                .GetSurveyForms(filter)
                .Select(x => new DataItem
                {
                    Value = x.SurveyFormIdentifier,
                    Text = x.SurveyFormName
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return Select(
                new QSurveyFormFilter
                {
                    OrganizationIdentifier = Filter.OrganizationIdentifier,
                    Identifiers = ids
                });
        }
    }
}