using System;
using System.Data;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class DivisionSelector : ComboBox
    {
        public Guid OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)] ?? Guid.Empty;
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var data = ContactRepository3.SelectDistrictsForSelector(OrganizationIdentifier);
            foreach (DataRow row in data.Rows)
                list.Add(row["Value"].ToString(), row["Text"].ToString());

            return list;
        }
    }
}
