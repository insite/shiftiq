using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class SenderTypeComboBox : ComboBox
    {
        public bool? SenderEnabled
        {
            get { return (bool?)ViewState[nameof(SenderEnabled)]; }
            set { ViewState[nameof(SenderEnabled)] = value; }
        }

        public string SenderType
        {
            get { return (string)ViewState[nameof(SenderType)]; }
            set { ViewState[nameof(SenderType)] = value; }
        }

        protected override ListItemArray CreateDataSource()
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var senders = TSenderSearch
                .Select(SenderType, organization, SenderEnabled)
                .Select(x => new { SenderType = x.SenderType }).Distinct().ToArray();

            var list = new ListItemArray();

            foreach (var sender in senders)
                list.Add(sender.SenderType, sender.SenderType);

            return list;
        }
    }
}