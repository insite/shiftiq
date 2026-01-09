using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Organizations.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common.Linq;

namespace InSite.Admin.Accounts.Senders.Controls
{
    public partial class MessageGrid : SearchResultsGridViewController<MessageFilter>
    {
        #region Properties and Fields

        protected override bool IsFinder => false;

        private Dictionary<Guid, QOrganization> _organizations;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _organizations = new Dictionary<Guid, QOrganization>();
        }

        public void LoadData(Guid senderIdentifier)
        {
            var filter = new MessageFilter
            {
                SenderIdentifier = senderIdentifier
            };

            Search(filter);
        }

        #endregion

        #region Select

        protected override int SelectCount(MessageFilter filter)
        {
            return ServiceLocator.MessageSearch.CountMessages(filter);
        }

        protected override IListSource SelectData(MessageFilter filter)
        {
            return ServiceLocator.MessageSearch
                .GetMessagesWithCount(filter)
                .Select(x => new
                {
                    x.MessageIdentifier,
                    x.MessageName,
                    x.ContentSubject,
                    x.OrganizationIdentifier,
                    x.MailoutCount,
                    x.RecipientCount
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Helpers

        protected string GetOrganizationCode(Guid id) => GetOrganization(id)?.OrganizationCode;

        protected string GetOrganizationName(Guid id) => GetOrganization(id)?.CompanyName;

        private QOrganization GetOrganization(Guid id)
        {
            if (!_organizations.TryGetValue(id, out var result))
                _organizations.Add(id, result = ServiceLocator.OrganizationSearch.Get(id));

            return result;
        }

        #endregion
    }
}