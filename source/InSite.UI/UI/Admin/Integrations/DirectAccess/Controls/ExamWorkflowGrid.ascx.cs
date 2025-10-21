using System;
using System.ComponentModel;

using InSite.Application.Attempts.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls
{
    public partial class ExamWorkflowGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        private Guid ContactIdentifier
        {
            get { return (Guid?)ViewState[nameof(ContactIdentifier)] ?? Guid.Empty; }
            set { ViewState[nameof(ContactIdentifier)] = value; }
        }

        public void LoadData(Guid contactIdentifier)
        {
            ContactIdentifier = contactIdentifier;

            Search(new NullFilter());
        }

        protected override int SelectCount(NullFilter filter)
        {
            return ServiceLocator.AttemptSearch.CountAttempts(new QAttemptFilter { LearnerUserIdentifier = ContactIdentifier });
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            return ServiceLocator.AttemptSearch
                .GetAttempts(
                    new QAttemptFilter
                    {
                        LearnerUserIdentifier = ContactIdentifier,
                        Paging = filter.Paging,
                        OrderBy = string.Join(",", new[]
                        {
                            $"{nameof(QAttempt.Registration)}.{nameof(QAttempt.Registration.Event.EventScheduledStart)}",
                            $"{nameof(QAttempt.Form)}.{nameof(QAttempt.Form.FormTitle)}",
                            $"{nameof(QAttempt.LearnerPerson)}.{nameof(QAttempt.LearnerPerson.UserFullName)}"
                        })
                    },
                    x => x.Form, x => x.Registration.Event)
                .ToSearchResult();
        }
    }
}