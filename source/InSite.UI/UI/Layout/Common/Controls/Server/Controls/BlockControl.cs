using System;
using System.Web.UI;

using InSite.Domain.Organizations;
using InSite.UI.Layout.Lobby;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public abstract class BlockControl : UserControl
    {
        public System.Globalization.CultureInfo CurrentCulture => new System.Globalization.CultureInfo(CurrentLanguage);
        public string CurrentLanguage => CookieTokenModule.Current.Language;
        public OrganizationState Organization => CurrentSessionState.Identity.Organization;
        public Domain.Foundations.User User => CurrentSessionState.Identity.User;

        public abstract void BindContent(ContentContainer block, string hook = null);

        public abstract string[] GetContentLabels();

        public string GetText(ContentContainer block, string label, string language = "en")
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.GetText(label, language);
        }

        public string GetHtml(ContentContainer block, string label, string language = "en")
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.GetHtml(label, language);
        }

        public string Translate(string text)
        {
            return ((LobbyBasePage)Page).Translator.Translate(text);
        }
    }
}