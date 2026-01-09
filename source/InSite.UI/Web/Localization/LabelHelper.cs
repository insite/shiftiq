using System.Globalization;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common
{
    public static class LabelHelper
    {
        public static string GetLabelContentText(string attribute)
            => GetTranslation(attribute, CookieTokenModule.Current.Language, false);

        public static string GetTranslation(string attribute)
            => GetTranslation(attribute, CookieTokenModule.Current.Language);

        public static string GetTranslation(string attribute, bool isMarkdown = false, params object[] args)
            => GetTranslation(attribute, CookieTokenModule.Current.Language, isMarkdown, args);

        public static string GetTranslation(string attribute, string language, bool isMarkdown = false, params object[] args)
        {
            if (attribute.IsEmpty())
                return attribute;

            var organization = CurrentSessionState.Identity.Organization.Identifier;

            var text = LabelSearch.GetTranslation(attribute, language, organization, true, true);
            if (text.IsEmpty())
            {
                text = LabelSearch.GetTranslation(attribute, language, OrganizationIdentifiers.Global, true, true);
                if (text.IsEmpty())
                    return attribute;
            }

            if (args.IsNotEmpty())
                text = string.Format(CultureInfo.CurrentCulture, text, args);

            if (isMarkdown)
                text = Markdown.ToHtml(text);

            return text;
        }
    }
}