using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Messages.Read;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Messages
{
    public partial class Dashboard : AdminBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
                BindModelToControls();
        }

        private MessageFilter CreateFilter()
        {
            var filter = new MessageFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                HasSender = null
            };
            return filter;
        }

        protected static string GetSearchUrl(string type)
        {
            return "/ui/admin/messages/messages/search?type=" + HttpUtility.UrlEncode(type);
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var insite = CurrentSessionState.Identity.IsOperator;
            var cmds = CurrentSessionState.Identity.IsInRole(CmdsRole.Programmers);

            var widgetItems = new List<Counter>
            {
                new Counter { Name = "Alert", Icon = "far fa-bell" },
                new Counter { Name = "Invitation", Icon = "far fa-envelope-open-text" },
                new Counter { Name = "Newsletter", Icon = "far fa-envelope" },
                new Counter { Name = "Notification", Icon = "far fa-bell" }
            };

            var counts = ServiceLocator.MessageSearch.CountMessagesByType(CreateFilter());
            foreach (var count in counts)
            {
                var widget = widgetItems.FirstOrDefault(x => x.Name == count.Name);
                if (widget == null)
                {
                    count.Icon = "far fa-envelope";
                    widgetItems.Add(count);
                }
                else
                {
                    widget.Value = count.Value;
                }
            }

            WidgetRepeater.DataSource = widgetItems;
            WidgetRepeater.DataBind();

            MailoutPanel.Visible = counts.Count > 0;

            var mailoutCount = ServiceLocator.MessageSearch.CountMailouts(new MailoutFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            });
            MailoutCount.Text = $@"{mailoutCount:n0}";

            var emailCount = EmailSearch.Count(new EmailFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            });
            EmailCount.Text = $@"{emailCount:n0}";
            EmailCountWidget.Visible = Identity.IsGranted("Admin/Messages");

            var clickCount = ServiceLocator.MessageSearch.CountClicks(new VClickFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier
            });
            ClickCount.Text = $@"{clickCount:n0}";

            ScheduledDeliveryGrid.LoadData(Guid.Empty);
            CompletedDeliveryGrid.LoadData(Guid.Empty);
        }
    }
}