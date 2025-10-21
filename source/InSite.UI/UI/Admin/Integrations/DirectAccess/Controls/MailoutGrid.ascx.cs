using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls
{
    public partial class MailoutGrid : SearchResultsGridViewController<MailoutFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(string email)
        {
            Search(new MailoutFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                Recipient = email
            });
        }

        protected override int SelectCount(MailoutFilter filter)
        {
            return ServiceLocator.MessageSearch
                .CountMailouts(filter);
        }

        protected override IListSource SelectData(MailoutFilter filter)
        {
            return ServiceLocator.MessageSearch
                .GetMailouts(filter)
                .ToSearchResult();
        }

        protected static string GetLocalTime(DateTimeOffset? value)
        {
            return value.Format(User.TimeZone, true);
        }
    }
}