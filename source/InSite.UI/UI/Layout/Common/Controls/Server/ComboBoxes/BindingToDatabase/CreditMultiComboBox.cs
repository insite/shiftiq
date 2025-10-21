using System;
using System.Collections.Generic;

using InSite.Persistence;

using Shift.Common;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Common.Web.UI
{
    public class CreditMultiComboBox : MultiComboBox
    {
        public List<Guid> OrganizationIdentifiers
        {
            get => (List<Guid>)ViewState[nameof(OrganizationIdentifiers)];
            set => ViewState[nameof(OrganizationIdentifiers)] = value;
        }

        public string SubType
        {
            get => (string)ViewState[nameof(SubType)];
            set => ViewState[nameof(SubType)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;

            var list = new ListItemArray();

            var data = StandardSearch.Bind(
                x => new { x.StandardIdentifier, Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title), x.ContentTitle },
                x => x.StandardType == SubType && x.OrganizationIdentifier == organization,
                "ContentTitle");

            foreach (var item in data)
                list.Add(item.StandardIdentifier.ToString(), item.Title);

            return list;
        }
    }
}