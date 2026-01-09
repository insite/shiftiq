using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Banks.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Records.Rurbics.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QRubricFilter>
    {
        private Dictionary<Guid, int> _connections;

        protected override int SelectCount(QRubricFilter filter)
        {
            return ServiceLocator.RubricSearch.CountRubrics(filter);
        }

        protected override IListSource SelectData(QRubricFilter filter)
        {
            _connections = new Dictionary<Guid, int>();

            var result = ServiceLocator.RubricSearch.GetRubrics(filter);
            foreach (var item in result)
            {
                var questions = ServiceLocator.BankSearch.CountQuestions(new QBankQuestionFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    RubricIdentifier = item.RubricIdentifier
                });
                _connections.Add(item.RubricIdentifier, questions);
            }

            return result.ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.Format(User.TimeZone, true);
        }

        protected string GetStatus()
        {
            var rubric = (RubricSearchItem)Page.GetDataItem();
            var questions = _connections[rubric.RubricIdentifier];

            return questions == 0 ? "Not Connected" : "Connected";
        }
    }
}