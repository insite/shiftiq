using System.Linq;
using System.Web;

namespace Shift.Common
{
    public static class LocationHelper
    {
        public static string GetGMapsAddressLink(string street1, string street2, string city, string province, string country, string postalCode)
        {
            if (street1.IsEmpty() && street2.IsEmpty() && city.IsEmpty() && province.IsEmpty() && postalCode.IsEmpty())
                return null;

            var parts = new[] { street2, street1, city, province, country, postalCode };
            var query = string.Join(" ", parts.Where(x => x.IsNotEmpty()));

            return $"https://www.google.com/maps/search/{HttpUtility.UrlEncode(query)}";
        }

        public static string ToHtml(string street1, string street2, string city, string province, string postalCode, string country, string phone, string fax) =>
            ToString(street1, street2, city, province, postalCode, country, phone, fax, "<br />");

        public static string ToString(string street1, string street2, string city, string province, string postalCode, string country, string phone, string fax, string separator = ", ")
        {
            var line1 = AddressBuilder.BuildLine(street2, " - ", street1);

            string line2, line3 = null;

            if (country.IsNotEmpty())
            {
                line2 = AddressBuilder.BuildLine(city, ", ", province);
                line3 = AddressBuilder.BuildLine(postalCode, " ", country);
            }
            else
            {
                line2 = AddressBuilder.BuildLine(city, ", ", province, " ", postalCode);
            }

            var line4 = AddressBuilder.BuildLineWithPrefix("Phone ", phone, ", ", "Fax ", fax);

            var lines = new[] { line1, line2, line3, line4 };

            return string.Join(separator, lines.Where(x => x.IsNotEmpty()));
        }

        #region Helpers

        private static class AddressBuilder
        {
            public static string BuildLine(string value1, string separator1, string value2)
            {
                var hasValue1 = value1.IsNotEmpty();
                var hasValue2 = value2.IsNotEmpty();

                if (hasValue1 && hasValue2)
                    return value1 + separator1 + value2;

                if (hasValue1)
                    return value1;

                if (hasValue2)
                    return value2;

                return null;
            }

            public static string BuildLine(string value1, string separator1, string value2, string separator2, string value3)
            {
                return BuildLine(BuildLine(value1, separator1, value2), separator2, value3);
            }

            public static string BuildLineWithPrefix(string prefix1, string value1, string separator1, string prefix2, string value2)
            {
                var hasValue1 = value1.IsNotEmpty();
                var hasValue2 = value2.IsNotEmpty();

                if (hasValue1 && hasValue2)
                    return prefix1 + value1 + separator1 + prefix2 + value2;

                if (hasValue1)
                    return prefix1 + value1;

                if (hasValue2)
                    return prefix2 + value2;

                return null;
            }
        }

        #endregion
    }
}
