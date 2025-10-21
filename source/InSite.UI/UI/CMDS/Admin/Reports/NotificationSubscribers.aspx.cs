using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;

using InSite.Application.Messages.Read;
using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web.UI;
using InSite.Custom.CMDS.Admin.Reports.Controls;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

using Color = System.Drawing.Color;

namespace InSite.Cmds.Admin.Reports.Forms
{
    public partial class NotificationSubscribers : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        public class Subscriber
        {
            public EmailAddress Recipient { get; set; }
            public EmailAddressList Followers { get; set; }

            public Subscriber()
            {
                Followers = new EmailAddressList();
            }
        }

        public class Notification
        {
            public Guid MessageIdentifier { get; set; }
            public string MessageTitle { get; set; }
            public string MessageName { get; set; }
            public List<Subscriber> Subscribers { get; set; }

            public Notification()
            {
                Subscribers = new List<Subscriber>();
            }
        }

        public class Department
        {
            public string OrganizationName { get; set; }
            public string DepartmentName { get; set; }
            public List<Notification> Notifications { get; set; }

            public Department()
            {
                Notifications = new List<Notification>();
            }
        }

        #endregion

        #region Methods (initialization and loading)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButton_Click;

            DownloadButton.Click += (s, a) => ExportToXLSX();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            LoadDepartmentSelector();
        }

        private void LoadDepartmentSelector()
        {
            DepartmentIdentifier.Value = null;
            DepartmentIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            DepartmentIdentifier.Value = null;
        }

        #endregion

        #region Methods (event handling)

        private void SearchButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DepartmentsNav.ClearItems();

            var departments = GetDataSource();
            foreach (var department in departments)
            {
                AddDepartmentsNavItem(out var navItem, out var grid);

                navItem.Title = department.DepartmentName;

                grid.LoadData(department.Notifications);
            }
        }

        #endregion

        #region Methods (searching)

        private Expression<Func<Membership, bool>> CreateMembershipFilter()
        {
            var departmentIds = DepartmentIdentifier.Values;
            var membershipTypes = Membership.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();

            return departmentIds.Length > 0
                ? LinqExtensions1.Expr(
                    (Membership u) => u.Group.GroupType == GroupTypes.Department
                                   && departmentIds.Contains(u.GroupIdentifier)
                                   && membershipTypes.Contains(u.MembershipType))
                : LinqExtensions1.Expr(
                    (Membership u) => u.Group.GroupType == GroupTypes.Department
                                   && u.Group.OrganizationIdentifier == CurrentIdentityFactory.ActiveOrganizationIdentifier
                                   && membershipTypes.Contains(u.MembershipType));
        }

        private IEnumerable<Department> GetDataSource()
        {
            var membershipFilter = CreateMembershipFilter();

            var memberships = MembershipSearch.Bind(
                x => new
                {
                    OrganizationName = x.Group.Organization.CompanyName,
                    DepartmentIdentifier = x.Group.GroupIdentifier,
                    DepartmentName = x.Group.GroupName,
                    UserIdentifier = x.UserIdentifier
                },
                membershipFilter
            );

            var subscriberFilter = new QSubscriberUserFilter
            {
                // MessageName = "Cmds",
                SubscriberIdentifiers = memberships.Select(x => x.UserIdentifier).Distinct().ToArray()
            };

            var recipients = ServiceLocator.MessageSearch.GetSubscriberUsers(subscriberFilter);

            var messages = recipients.Select(x => new { x.MessageIdentifier, x.MessageName, x.MessageTitle })
                .Distinct()
                .OrderBy(x => x.MessageTitle)
                .ToList();

            var followers = GetFollowers(recipients);

            var departments = new List<Department>();

            foreach (var group in memberships.GroupBy(x => x.DepartmentIdentifier))
            {
                var firstItem = group.First();

                var department = new Department
                {
                    OrganizationName = firstItem.OrganizationName,
                    DepartmentName = firstItem.DepartmentName
                };

                foreach (var message in messages)
                {
                    var notification = new Notification
                    {
                        MessageIdentifier = message.MessageIdentifier,
                        MessageName = message.MessageName,
                        MessageTitle = message.MessageTitle,

                        Subscribers = recipients
                            .Where(
                                x => x.MessageIdentifier == message.MessageIdentifier
                                  && group.Any(y => y.UserIdentifier == x.UserIdentifier)
                            )
                            .Select(x => new Subscriber { Recipient = new EmailAddress(x.UserEmail, x.UserFullName) })
                            .OrderBy(x => x.Recipient.DisplayName)
                            .ToList()
                    };

                    var messageFollowers = followers[message.MessageIdentifier];

                    foreach (var subscriber in notification.Subscribers)
                    {
                        subscriber.Followers = new EmailAddressList(messageFollowers
                            .Where(x => x.SubscriberEmail.Equals(subscriber.Recipient.Address, StringComparison.CurrentCultureIgnoreCase))
                            .Select(x => new EmailAddress(x.FollowerEmail, x.FollowerFullName))
                            .ToList());
                    }

                    department.Notifications.Add(notification);
                }

                departments.Add(department);
            }

            return departments.OrderBy(x => x.DepartmentName);
        }

        private static Dictionary<Guid, List<VFollower>> GetFollowers(List<ISubscriberPerson> recipients)
        {
            var messageIds = recipients.Select(x => x.MessageIdentifier).Distinct().ToArray();
            var followers = new Dictionary<Guid, List<VFollower>>();

            foreach (var messageId in messageIds)
            {
                var messageFollowers = ServiceLocator.MessageSearch.GetFollowers(messageId);
                followers.Add(messageId, messageFollowers);
            }

            return followers;
        }

        #endregion

        #region Helper methods

        private void AddDepartmentsNavItem(out NavItem navItem, out NotificationsGrid grid)
        {
            DepartmentsNav.AddItem(navItem = new NavItem());
            navItem.Controls.Add(grid = (NotificationsGrid)LoadControl("./NotificationSubscribersGrid.ascx"));
        }

        #endregion

        #region Methods (exporting)

        private void ExportToXLSX()
        {
            var captionStyle = new XlsxCellStyle { IsBold = true };
            captionStyle.Border.BorderAround(XlsxBorderStyle.None);

            var headerStyle = new XlsxCellStyle { IsBold = true };
            headerStyle.Border.BorderAround(XlsxBorderStyle.Thin, Color.Black);

            var columnStyle = new XlsxCellStyle();
            columnStyle.Border.BorderAround(XlsxBorderStyle.None);

            var leftColumnStyle = new XlsxCellStyle();
            leftColumnStyle.Border.BorderAround(XlsxBorderStyle.Thin, Color.Black);

            var rightColumnStyle = new XlsxCellStyle();
            rightColumnStyle.Border.BorderAround(XlsxBorderStyle.None);
            rightColumnStyle.Border.Right.Style = XlsxBorderStyle.Thin;
            rightColumnStyle.Border.Right.Color = Color.Black;
            rightColumnStyle.Border.Left.Style = XlsxBorderStyle.Thin;
            rightColumnStyle.Border.Left.Color = Color.Black;

            var rightBottomColumnStyle = new XlsxCellStyle();
            rightBottomColumnStyle.Border.BorderAround(XlsxBorderStyle.None);
            rightBottomColumnStyle.Border.Right.Style = XlsxBorderStyle.Thin;
            rightBottomColumnStyle.Border.Right.Color = Color.Black;
            rightBottomColumnStyle.Border.Left.Style = XlsxBorderStyle.Thin;
            rightBottomColumnStyle.Border.Left.Color = Color.Black;
            rightBottomColumnStyle.Border.Bottom.Style = XlsxBorderStyle.Thin;
            rightBottomColumnStyle.Border.Bottom.Color = Color.Black;

            var bottomColumnStyle = new XlsxCellStyle();
            bottomColumnStyle.Border.BorderAround(XlsxBorderStyle.None);
            bottomColumnStyle.Border.Bottom.Style = XlsxBorderStyle.Thin;
            bottomColumnStyle.Border.Bottom.Color = Color.Black;

            var xlsxSheet = new XlsxWorksheet("Notification Subscribers");
            xlsxSheet.Columns[0].Width = 40;
            xlsxSheet.Columns[1].Width = 30;
            xlsxSheet.Columns[2].Width = 30;

            var departments = GetDataSource();

            int rowNumber = 0;

            foreach (var department in departments)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Organization", Style = captionStyle });
                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = department.OrganizationName, Style = columnStyle });

                rowNumber++;

                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Department", Style = captionStyle });
                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = department.DepartmentName, Style = columnStyle });

                rowNumber++;

                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = "Notification", Style = headerStyle });
                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = "Subscribers", Style = headerStyle });
                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = "Followers", Style = headerStyle });

                rowNumber++;

                foreach (var notification in department.Notifications)
                {
                    var rowSpan = notification.Subscribers.Sum(x => x.Followers.Count == 0 ? 1 : x.Followers.Count);

                    if (rowSpan == 0)
                        rowSpan = 1;

                    xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber, 1, rowSpan) { Value = notification.MessageTitle, Style = leftColumnStyle });

                    if (notification.Subscribers.Count > 0)
                    {
                        for (int subscriberIndex = 0; subscriberIndex < notification.Subscribers.Count; subscriberIndex++)
                        {
                            var subscriber = notification.Subscribers[subscriberIndex];
                            var subscriberStyle = subscriberIndex == notification.Subscribers.Count - 1 ? bottomColumnStyle : columnStyle;
                            var subscriberRowSpan = subscriber.Followers.Count > 0 ? subscriber.Followers.Count : 1;

                            xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber, 1, subscriberRowSpan) { Value = subscriber.Recipient.DisplayName, Style = subscriberStyle });

                            if (subscriber.Followers.Count > 0)
                            {
                                for (int followerIndex = 0; followerIndex < subscriber.Followers.Count; followerIndex++)
                                {
                                    var follower = subscriber.Followers[followerIndex];

                                    var followerStyle = subscriberIndex == notification.Subscribers.Count - 1 && followerIndex == subscriber.Followers.Count - 1
                                        ? rightBottomColumnStyle
                                        : rightColumnStyle;

                                    xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber++) { Value = follower.DisplayName, Style = followerStyle });
                                }
                            }
                            else
                            {
                                var followerStyle = subscriberIndex == notification.Subscribers.Count - 1 ? rightBottomColumnStyle : rightColumnStyle;
                                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber++) { Style = followerStyle });
                            }
                        }
                    }
                    else
                    {
                        xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Style = bottomColumnStyle });
                        xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Style = rightBottomColumnStyle });
                        rowNumber++;
                    }
                }

                rowNumber++;
            }

            ReportXlsxHelper.Export(xlsxSheet);
        }

        #endregion
    }
}