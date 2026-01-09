using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class SenderComboBox : ComboBox
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
            var senders = TSenderSearch.Select(SenderType, organization, SenderEnabled);
            var list = new ListItemArray { TotalCount = senders.Length };
            foreach (var sender in senders)
                list.Add(sender.SenderIdentifier.ToString(), sender.SenderNickname);
            return list;
        }
    }
}