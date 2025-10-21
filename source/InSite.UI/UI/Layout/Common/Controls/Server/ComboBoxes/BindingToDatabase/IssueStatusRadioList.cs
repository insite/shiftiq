using System;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class IssueStatusRadioList : RadioButtonList
    {
        public string IssueType
        {
            get => (string)ViewState[nameof(IssueType)];
            set => ViewState[nameof(IssueType)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DataTextField = nameof(ListItem.Text);
            DataValueField = nameof(ListItem.Value);
        }

        public override void DataBind()
        {
            DataSource = CreateDataSource();

            base.DataBind();
        }

        private ListItemArray CreateDataSource()
        {
            var org = CurrentSessionState.Identity.Organization;
            var items = ServiceLocator.IssueSearch.GetStatuses(org.Identifier, IssueType);

            var list = new ListItemArray();
            foreach (var i in items)
                list.Add(i.StatusIdentifier.ToString(), i.StatusName);
            return list;
        }
    }
}