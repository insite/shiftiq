using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Messages.Clicks.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VClickFilter>
    {
        public override VClickFilter Filter
        {
            get
            {
                var filter = new VClickFilter
                {
                    OrganizationIdentifier = Organization.Identifier,

                    ClickedSince = ClickedSince.Value,
                    ClickedBefore = ClickedBefore.Value,

                    UserName = UserName.Text,
                    UserEmail = UserEmail.Text,
                    MessageTitle = MessageTitle.Text,
                    LinkText = LinkText.Text,
                    LinkUrl = LinkUrl.Text,
                    UserBrowser = UserBrowser.Text,
                    UserHostAddress = UserHostAddress.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ClickedSince.Value = value.ClickedSince;
                ClickedBefore.Value = value.ClickedBefore;

                UserName.Text = value.UserName;
                UserEmail.Text = value.UserEmail;
                MessageTitle.Text = value.MessageTitle;
                LinkText.Text = value.LinkText;
                LinkUrl.Text = value.LinkUrl;
                UserBrowser.Text = value.UserBrowser;
                UserHostAddress.Text = value.UserHostAddress;
            }
        }

        public override void Clear()
        {
            ClickedSince.Value = null;
            ClickedBefore.Value = null;

            UserName.Text = null;
            UserEmail.Text = null;
            MessageTitle.Text = null;
            LinkText.Text = null;
            LinkUrl.Text = null;
            UserBrowser.Text = null;
            UserHostAddress.Text = null;
        }
    }
}