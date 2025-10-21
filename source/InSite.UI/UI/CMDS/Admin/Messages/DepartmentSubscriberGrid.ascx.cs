using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Custom.CMDS.Admin.Messages
{
    public partial class DepartmentSubscriberGrid : BaseUserControl
    {
        [Serializable]
        private class DummyFilter
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid? DepartmentIdentifier { get; set; }
            public Guid MessageIdentifier { get; set; }
            public string[] EmploymentTypes { get; set; }
        }

        private DummyFilter Filter
        {
            get => (DummyFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCreated += Grid_RowCreated;
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            if (Filter == null)
                return;

            var filter = Filter;
            var paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            var people = MembershipSearch.QueryPeople(
                filter.OrganizationIdentifier,
                filter.DepartmentIdentifier,
                filter.EmploymentTypes,
                paging);

            var followers = ServiceLocator.MessageSearch
                .GetFollowers(filter.MessageIdentifier)
                .ToList();

            var table = new DataTable();
            table.Columns.Add("AggregateIdentifier", typeof(Guid));
            table.Columns.Add("ContactIdentifier", typeof(Guid));
            table.Columns.Add("FullName", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("EmailEnabled", typeof(bool));
            table.Columns.Add("Subscribed", typeof(DateTimeOffset));
            table.Columns.Add("Delivered", typeof(DateTimeOffset));
            table.Columns.Add("IsSubscribed", typeof(bool));
            table.Columns.Add("FollowerEmails", typeof(string));

            foreach (var person in people)
            {
                var row = table.NewRow();
                row["AggregateIdentifier"] = filter.MessageIdentifier;
                row["ContactIdentifier"] = person.UserIdentifier;
                row["FullName"] = person.User.FullName;
                row["Email"] = person.User.Email;
                row["EmailEnabled"] = person.EmailEnabled;
                row["IsSubscribed"] = false;

                var subscriber = ServiceLocator.MessageSearch.GetSubscriberUser(filter.MessageIdentifier, person.UserIdentifier);

                if (subscriber != null)
                {
                    row["IsSubscribed"] = true;
                    row["Subscribed"] = subscriber.Subscribed;
                }

                var deliveryFilter = new DeliveryFilter
                {
                    MessageIdentifier = filter.MessageIdentifier,
                    RecipientAddress = person.User.Email
                };
                var deliveries = ServiceLocator.MessageSearch.GetDeliveries(deliveryFilter);

                if (deliveries.IsNotEmpty())
                {
                    var delivery = deliveries.OrderByDescending(x => x.DeliveryCompleted).First();

                    if (delivery.DeliveryCompleted != null)
                        row["Delivered"] = delivery.DeliveryCompleted;
                }

                var subscriberFollers = followers.Where(x => x.SubscriberIdentifier == person.UserIdentifier).ToList();
                var html = new StringBuilder();
                for (var j = 0; j < subscriberFollers.Count; j++)
                {
                    if (j > 0)
                        html.Append(", ");
                    else
                        html.Append("cc: ");
                    html.Append(subscriberFollers[j].FollowerEmail);
                }
                row["FollowerEmails"] = html.ToString();

                table.Rows.Add(row);
            }

            Grid.DataSource = table;
        }

        private void Grid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var checkbox = (ICheckBoxControl)e.Row.FindControl("Subscribed");
            checkbox.CheckedChanged += Subscribed_CheckedChanged;
        }

        private void Subscribed_CheckedChanged(object sender, EventArgs e)
        {
            var dummyFilter = (DummyFilter)Filter;

            var checkbox = (System.Web.UI.WebControls.CheckBox)sender;

            var row = (GridViewRow)checkbox.NamingContainer;

            var contactIdentifier = Grid.GetDataKey<Guid>(row);

            if (checkbox.Checked)
            {
                var add = new AddSubscriber(dummyFilter.MessageIdentifier, contactIdentifier, "Message Recipient", false, false);
                ServiceLocator.SendCommand(add);
            }
            else
            {
                var remove = new RemoveMessageSubscriber(dummyFilter.MessageIdentifier, contactIdentifier, false);
                ServiceLocator.SendCommand(remove);
            }

            Grid.DataBind();
        }

        public void LoadData(Guid organization, Guid? department, string[] employmentTypes, Guid message)
        {
            var filter = Filter = new DummyFilter
            {
                OrganizationIdentifier = organization,
                DepartmentIdentifier = department,
                MessageIdentifier = message,
                EmploymentTypes = employmentTypes
            };

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = MembershipSearch.CountPeople(
                filter.OrganizationIdentifier,
                filter.DepartmentIdentifier,
                filter.EmploymentTypes);
            Grid.DataBind();
        }

        protected static string GetLocalTime(DateTimeOffset? date)
        {
            return date.Format(User.TimeZone, true);
        }
    }
}