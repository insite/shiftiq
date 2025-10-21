using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Layout.Lobby
{
    public class LobbyBaseControl : UserControl
    {
        private IHasTranslator _translator;

        public string CurrentLanguage => CookieTokenModule.Current.Language;

        protected CultureInfo CurrentLanguageCulture => CultureInfo.GetCultureInfo(CurrentLanguage);

        protected static Domain.Organizations.OrganizationState Organization => OrganizationSearch.Select(CookieTokenModule.Current.OrganizationCode);

        protected static bool IsContentItem(RepeaterItemEventArgs e) => IsContentItem(e.Item);

        protected static bool IsContentItem(RepeaterItem item)
        {
            return item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem;
        }

        public string Translate(string text)
        {
            if (_translator == null)
                _translator = Page as IHasTranslator;

            if (_translator == null)
                return text;

            return _translator.Translator.Translate(text);
        }

        protected string GetDisplayText(string attribute) 
            => LabelSearch.GetTranslation(attribute, CurrentLanguage, Organization.OrganizationIdentifier);
    }
}