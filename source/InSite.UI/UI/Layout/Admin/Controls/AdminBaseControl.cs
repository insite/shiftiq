using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Foundations;
using InSite.Domain.Organizations;
using InSite.Persistence;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.UI.Layout.Admin
{
    public class AdminBaseControl : UserControl
    {
        public string CurrentLanguage => CookieTokenModule.Current.Language;
        
        protected CultureInfo CurrentLanguageCulture => CultureInfo.GetCultureInfo(Identity.Language);

        protected static ISecurityFramework Identity => CurrentSessionState.Identity;

        public Common.Controls.Navigation.Navigator Navigator { get; private set; }

        protected static OrganizationState Organization => Identity.Organization;

        protected static UserModel User => Identity.User;

        protected string GetDisplayText(string attribute) =>
            LabelSearch.GetTranslation(attribute, CurrentSessionState.Identity.Language, Organization.OrganizationIdentifier);

        protected static bool IsContentItem(RepeaterItemEventArgs e) => IsContentItem(e.Item);

        protected static bool IsContentItem(RepeaterItem item)
        {
            return item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Navigator = new Common.Controls.Navigation.Navigator(Request);
        }

        public string Translate(string text) => text;
    }
}