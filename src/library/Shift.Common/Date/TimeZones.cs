using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using NodaTime;
using NodaTime.Text;

namespace Shift.Common
{
    public static class TimeZones
    {
        #region Fields and properties

        private static Dictionary<string, TimeZoneAbbreviation> Aliases;
        private static ReadOnlyDictionary<string, string> ProvincesCanada;
        private static ReadOnlyDictionary<string, string> ProvincesUs;

        public static class Ids
        {
            public const string Pacific = "America/Vancouver";
            public const string Mountain = "America/Edmonton";
            public const string Eastern = "America/Toronto";
            public const string Utc = "UTC";
        }

        private static class PrivateIds
        {
            public const string Hawaiian = "Pacific/Honolulu";
            public const string Alaskan = "America/Anchorage";
            public const string Pacific = "America/Vancouver";
            public const string Mountain = "America/Edmonton";
            public const string Central = "America/Winnipeg";
            public const string Eastern = "America/Toronto";
            public const string Newfoundland = "America/St_Johns";
            public const string Atlantic = "America/Halifax";
            public const string Utc = "UTC";
        }

        private static readonly TimeZoneAbbreviation Utc = new TimeZoneAbbreviation("UTC", GetStandardUtcOffset("UTC"), "UTC", "UTC", "UTC", "UTC", true);

        private static TimeZoneAbbreviation[] Definitions = { Utc };

        public static string[] Supported { get; private set; } = { Utc.TimeZoneId };

        #endregion

        #region Initialization

        public static void Initialize(IEnumerable<TimeZoneDescription> timeZones)
        {
            Definitions = timeZones
                .Select(x => new TimeZoneAbbreviation(
                    x.Id,
                    GetStandardUtcOffset(x.Id),
                    x.Name,
                    x.Generic,
                    x.Standard,
                    x.Daylight,
                    x.IsIANACompatible
                ))
                .Union(new TimeZoneAbbreviation[] { Utc })
                .ToArray();

            Supported = Definitions.Select(x => x.TimeZoneId).ToArray();

            Aliases = new Dictionary<string, TimeZoneAbbreviation>(StringComparer.OrdinalIgnoreCase);

            foreach (var tz in Definitions)
            {
                AddAlias(tz.TimeZoneId, tz);
                AddAlias(tz.DisplayName, tz);
                AddAlias(tz.Generic, tz);
                AddAlias(tz.Daylight, tz);
                AddAlias(tz.Standard, tz);
            }

            var provincesCanada = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var provincesUs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            InitProvinces(provincesCanada, provincesUs);

            ProvincesCanada = new ReadOnlyDictionary<string, string>(provincesCanada);
            ProvincesUs = new ReadOnlyDictionary<string, string>(provincesUs);

            void AddAlias(string alias, TimeZoneAbbreviation abbrev)
            {
                if (!string.IsNullOrWhiteSpace(alias) && !Aliases.ContainsKey(alias))
                    Aliases.Add(alias, abbrev);
            }
        }

        private static string GetStandardUtcOffset(string timeZoneId)
        {
            var pattern = OffsetPattern.CreateWithInvariantCulture("+HH:mm");
            var instant = Instant.FromUtc(DateTime.Now.Year, 12, 31, 0, 0);

            var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
            var offset = zone
                .GetZoneInterval(instant)
                .StandardOffset;

            return $"UTC{pattern.Format(offset)}";
        }

        #endregion

        #region Format

        public static string Format(DateTime date)
        {
            return date == DateTime.MinValue
                ? string.Empty
                : $"{date:MMM d, yyyy}";
        }

        public static void Append(StringBuilder builder, DateTimeOffset time, string timeZoneId = null, bool isHtml = false, bool wrap = false, bool showSeconds = false, CultureInfo culture = null)
        {
            // Convert the input time to the desired time zone.
            if (!string.IsNullOrEmpty(timeZoneId))
                time = ConvertTime(time, timeZoneId);
            else
                timeZoneId = FindTimeZoneId(time);

            // Get the abbreviations for the desired time zone.
            var abbreviation = GetAbbreviation(timeZoneId);

            // Determine whether or not daylight savings is in effect for the input time.
            var zz = abbreviation.GetAbbreviation(time);

            // Format the output string.

            if (culture == null)
                builder.AppendFormat(@"{0:MMM d, yyyy}", time);
            else
                builder.AppendFormat(culture, "{0:MMM d, yyyy}", time);

            var timeAsString = showSeconds ? $"{time:h:mm:ss tt}" : $"{time:h:mm tt}";

            if (isHtml && wrap)
                builder.AppendFormat(@"<div class=""form-text text-body-secondary"">{0} {1}</div>", timeAsString, zz);
            else if (isHtml)
                builder.AppendFormat(@"<span class=""form-text text-body-secondary""> - {0} {1}</span>", timeAsString, zz);
            else
                builder.AppendFormat(@" - {0} {1}", timeAsString, zz);
        }

        public static string Format(DateTimeOffset time, string zone, string language) => Format(time, zone, false, false, false, CultureInfo.GetCultureInfo(language));

        public static string Format(DateTimeOffset time, string zone, bool isHtml = false, bool wrap = false, bool showSeconds = false, CultureInfo culture = null)
        {
            var builder = new StringBuilder();

            Append(builder, time, zone, isHtml, wrap, showSeconds, culture);

            return builder.ToString();
        }

        public static string FormatDateOnly(DateTimeOffset date, string outputZoneId, CultureInfo culture = null, string format = "{0:MMM d, yyyy}")
        {
            var converted = !string.IsNullOrEmpty(outputZoneId) ? ConvertTime(date, outputZoneId) : date;

            return culture == null
                ? string.Format(format ?? "{0:MMM d, yyyy}", converted)
                : string.Format(culture, format ?? "{0:MMM d, yyyy}", converted);
        }

        public static string FormatTimeOnly(DateTimeOffset date, string outputZoneId)
        {
            var zoneId = !string.IsNullOrEmpty(outputZoneId) ? outputZoneId : FindTimeZoneId(date);
            var converted = !string.IsNullOrEmpty(outputZoneId) ? ConvertTime(date, zoneId) : date;
            var abbreviation = GetAbbreviation(zoneId);
            var zz = abbreviation.GetAbbreviation(converted);
            return $@"{converted:h:mm tt} {zz}";
        }

        #endregion

        #region Convert

        public static DateTimeOffset ConvertTime(DateTimeOffset date, string outputZoneId)
        {
            var outputZone = GetZone(outputZoneId);
            var instant = OffsetDateTime.FromDateTimeOffset(date).ToInstant();
            var converted = instant.InZone(outputZone);

            if (converted.Year < 1)
                return date;

            return converted.ToDateTimeOffset();
        }

        public static DateTime ConvertTime(DateTime date, string inputZoneId, string outputZoneId)
        {
            var inputZone = GetZone(inputZoneId);
            var outputZone = GetZone(outputZoneId);
            var local = LocalDateTime.FromDateTime(DateTime.SpecifyKind(date, DateTimeKind.Unspecified));
            var instant = local.InZoneLeniently(inputZone).ToInstant();

            return instant.InZone(outputZone).LocalDateTime.ToDateTimeUnspecified();
        }

        public static DateTime ConvertTimeFromUtc(DateTime date, string outputZoneId)
        {
            var outputZone = GetZone(outputZoneId);
            var utc = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return Instant.FromDateTimeUtc(utc).InZone(outputZone).LocalDateTime.ToDateTimeUnspecified();
        }

        public static DateTimeOffset ToDateTimeOffset(DateTime date, string timeZoneId)
        {
            var unspecified = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            var zoned = LocalDateTime.FromDateTime(unspecified).InZoneLeniently(GetZone(timeZoneId));

            return new DateTimeOffset(unspecified, zoned.Offset.ToTimeSpan());
        }

        public static DateTimeOffset? ToDateTimeOffset(DateTime? date, string timeZoneId)
        {
            return date != null ? ToDateTimeOffset(date.Value, timeZoneId) : (DateTimeOffset?)null;
        }

        public static DateTime? GetDate(DateTimeOffset? date, string outputZoneId)
        {
            return date.HasValue ? GetDate(date.Value, outputZoneId) : (DateTime?)null;
        }

        public static DateTime GetDate(DateTimeOffset date, string outputZoneId)
        {
            var abbrev = GetAbbreviation(outputZoneId);
            if (abbrev == null)
                return date.DateTime;

            var converted = ConvertTime(date, abbrev.TimeZoneId);

            return converted.DateTime;
        }

        public static DateTime? GetDateUtc(DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToUniversalTime();

            return null;
        }

        public static DateTimeOffset GetDateTimeOffset(DateTime date, DateTime? time, string tzId)
        {
            if (time.HasValue)
                date = new DateTime(date.Year, date.Month, date.Day, time.Value.Hour, time.Value.Minute, time.Value.Second);

            return ToDateTimeOffset(date, tzId);
        }

        #endregion

        #region Helpers

        public static TimeZoneAbbreviation GetAbbreviation(string timeZoneAlias)
        {
            return !string.IsNullOrEmpty(timeZoneAlias)
                ? Aliases.TryGetValue(timeZoneAlias, out var abbrev) ? abbrev : null
                : Utc;
        }

        public static TimeSpan GetUtcOffset(DateTime date, string timeZoneId)
        {
            return LocalDateTime.FromDateTime(DateTime.SpecifyKind(date, DateTimeKind.Unspecified)).InZoneLeniently(GetZone(timeZoneId)).Offset.ToTimeSpan();
        }

        public static TimeSpan GetUtcOffset(DateTimeOffset date, string timeZoneAlias)
        {
            var instant = OffsetDateTime.FromDateTimeOffset(date).ToInstant();
            return GetZone(timeZoneAlias).GetUtcOffset(instant).ToTimeSpan();
        }

        public static string FindTimeZoneId(DateTimeOffset date, string currentTimeZoneId = null)
        {
            var offset = date.Offset;

            if (!string.IsNullOrEmpty(currentTimeZoneId)
                && GetUtcOffset(date, currentTimeZoneId).Equals(offset)
                )
            {
                return currentTimeZoneId;
            }

            foreach (var tzId in Supported)
            {
                if (GetUtcOffset(date, tzId).Equals(offset))
                    return tzId;
            }

            return PrivateIds.Utc;
        }

        public static bool IsDaylightSavingTime(DateTimeOffset date, string timeZoneAlias)
        {
            var instant = OffsetDateTime.FromDateTimeOffset(date).ToInstant();
            return GetZone(timeZoneAlias).GetZoneInterval(instant).Savings != Offset.Zero;
        }

        public static bool IsValidZone(string timeZoneAlias)
        {
            return !timeZoneAlias.IsEmpty() && GetAbbreviation(timeZoneAlias) != null;
        }

        public static string ValidateZone(string timeZoneId)
        {
            var abbrev = GetAbbreviation(timeZoneId);

            if (abbrev == null)
                throw new ArgumentException($"Invalid timeZoneId: {timeZoneId}");

            return abbrev.TimeZoneId;
        }

        private static DateTimeZone GetZone(string alias)
        {
            var zoneId = GetAbbreviation(alias)?.TimeZoneId;

            if (zoneId == null)
                throw new ArgumentException($"Invalid timeZoneId: {alias}");

            return DateTimeZoneProviders.Tzdb[zoneId];
        }

        #endregion

        #region Location

        private static bool IsCountryCanada(string name)
        {
            return
                string.Equals(name, "Canada", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "CA", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCountryUs(string name)
        {
            return
                string.Equals(name, "UnitedStatesOfAmerica", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "UnitedStates", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "USA", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "US", StringComparison.OrdinalIgnoreCase);
        }

        private static void InitProvinces(Dictionary<string, string> canada, Dictionary<string, string> us)
        {
            var hawaiian = GetAbbreviation(PrivateIds.Hawaiian);
            var alaskan = GetAbbreviation(PrivateIds.Alaskan);
            var pacific = GetAbbreviation(PrivateIds.Pacific);
            var mountain = GetAbbreviation(PrivateIds.Mountain);
            var central = GetAbbreviation(PrivateIds.Central);
            var eastern = GetAbbreviation(PrivateIds.Eastern);
            var newfoundland = GetAbbreviation(PrivateIds.Newfoundland);
            var atlantic = GetAbbreviation(PrivateIds.Atlantic);

            AddLocations(canada, mountain, "Alberta", "Alta", "Alb", "AB");
            AddLocations(canada, pacific, "BritishColumbia", "CB", "BC");
            AddLocations(canada, central, "Manitoba", "Man", "MB");
            AddLocations(canada, atlantic, "NewBrunswick", "NB");
            AddLocations(canada, newfoundland, "NewfoundlandAndLabrador", "NewfoundlandLabrador", "Newfoundland", "Labrador", "NfLd", "TNL", "NL");
            AddLocations(canada, mountain, "NorthwestTerritories", "NorthwestTerr", "NWTerritories", "NWTerr", "NWT", "TNO", "NT");
            AddLocations(canada, atlantic, "NovaScotia", "NE", "NS");
            AddLocations(canada, central, "Nunavut", "NVT", "NU");
            AddLocations(canada, eastern, "Ontario", "ONT", "ON");
            AddLocations(canada, atlantic, "PrinceEdwardIsland", "PEI", "IPE", "PE");
            AddLocations(canada, eastern, "Quebec", "QUE", "QC");
            AddLocations(canada, central, "Saskatchewan", "Sask", "SK");
            AddLocations(canada, mountain, "Yukon", "YN", "YT");

            AddLocations(us, central, "Alabama", "AL");
            AddLocations(us, alaskan, "Alaska", "AK");
            AddLocations(us, mountain, "Arizona", "AZ");
            AddLocations(us, central, "Arkansas", "AR");
            AddLocations(us, pacific, "California", "CA");
            AddLocations(us, mountain, "Colorado", "CO");
            AddLocations(us, eastern, "Connecticut", "CT");
            AddLocations(us, eastern, "Delaware", "DE");
            AddLocations(us, eastern, "District of Columbia", "Columbia", "DC");
            AddLocations(us, eastern, "Florida", "FL");
            AddLocations(us, eastern, "Georgia", "GA");
            AddLocations(us, hawaiian, "Hawaii", "HI");
            AddLocations(us, mountain, "Idaho", "ID");
            AddLocations(us, central, "Illinois", "IL");
            AddLocations(us, eastern, "Indiana", "IN");
            AddLocations(us, central, "Iowa", "IA");
            AddLocations(us, central, "Kansas", "KS");
            AddLocations(us, eastern, "Kentucky", "KY");
            AddLocations(us, central, "Louisiana", "LA");
            AddLocations(us, eastern, "Maine", "ME");
            AddLocations(us, eastern, "Maryland", "MD");
            AddLocations(us, eastern, "Massachusetts", "MA");
            AddLocations(us, eastern, "Michigan", "MI");
            AddLocations(us, central, "Minnesota", "MN");
            AddLocations(us, central, "Mississippi", "MS");
            AddLocations(us, central, "Missouri", "MO");
            AddLocations(us, mountain, "Montana", "MT");
            AddLocations(us, central, "Nebraska", "NE");
            AddLocations(us, pacific, "Nevada", "NV");
            AddLocations(us, eastern, "NewHampshire", "NH");
            AddLocations(us, eastern, "NewJersey", "NJ");
            AddLocations(us, mountain, "NewMexico", "NM");
            AddLocations(us, eastern, "NewYork", "NY");
            AddLocations(us, eastern, "NorthCarolina", "NC");
            AddLocations(us, central, "NorthDakota", "ND");
            AddLocations(us, eastern, "Ohio", "OH");
            AddLocations(us, central, "Oklahoma", "OK");
            AddLocations(us, pacific, "Oregon", "OR");
            AddLocations(us, eastern, "Pennsylvania", "PA");
            AddLocations(us, eastern, "RhodeIsland", "RI");
            AddLocations(us, eastern, "SouthCarolina", "SC");
            AddLocations(us, central, "SouthDakota", "SD");
            AddLocations(us, eastern, "Tennessee", "TN");
            AddLocations(us, central, "Texas", "TX");
            AddLocations(us, mountain, "Utah", "UT");
            AddLocations(us, eastern, "Vermont", "VT");
            AddLocations(us, eastern, "Virginia", "VA");
            AddLocations(us, pacific, "Washington", "WA");
            AddLocations(us, eastern, "WestVirginia", "WV");
            AddLocations(us, central, "Wisconsin", "WI");
            AddLocations(us, mountain, "Wyoming", "WY");

            void AddLocations(Dictionary<string, string> dict, TimeZoneAbbreviation tz, params string[] names)
            {
                foreach (var n in names)
                {
                    if (tz != null)
                        dict.Add(n, tz.TimeZoneId);
                }
            }
        }

        public static string FindTimeZoneIdByProvince(string province, string country)
        {
            if (province.IsEmpty())
                return null;

            province = StringHelper.RemoveNonAlphanumericCharacters(province);
            country = StringHelper.RemoveNonAlphanumericCharacters(country);

            if (IsCountryCanada(country))
                return ProvincesCanada.GetOrDefault(province);
            else if (IsCountryUs(country))
                return ProvincesUs.GetOrDefault(province);

            return null;
        }

        public static string FindTimeZoneIdByProvince(string province)
        {
            if (province.IsEmpty())
                return null;

            province = StringHelper.RemoveNonAlphanumericCharacters(province);

            return ProvincesCanada.GetOrDefault(province)
                ?? ProvincesUs.GetOrDefault(province);
        }

        private static readonly Regex CanadaPostalCodePattern = new Regex("^[A-Z][0-9][A-Z]([0-9][A-Z][0-9])?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex UsZipCodePattern = new Regex("^[0-9]{5}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string FindTimeZoneIdByPostalCode(string code, string country)
        {
            if (code.IsEmpty() || country.IsEmpty())
                return null;

            code = StringHelper.RemoveNonAlphanumericCharacters(code.ToLower());

            if (IsCountryCanada(country))
            {
                if (CanadaPostalCodePattern.IsMatch(code))
                    return FindTimeZoneIdByCanadaPostalCode(code);
            }
            else if (IsCountryUs(country))
            {
                if (UsZipCodePattern.IsMatch(code))
                    return FindTimeZoneIdByUsZipCode(code);
            }

            return null;
        }

        public static string FindTimeZoneIdByPostalCode(string code)
        {
            if (code.IsEmpty())
                return null;

            code = StringHelper.RemoveNonAlphanumericCharacters(code.ToLower());

            if (CanadaPostalCodePattern.IsMatch(code))
                return FindTimeZoneIdByCanadaPostalCode(code);
            else if (UsZipCodePattern.IsMatch(code))
                return FindTimeZoneIdByUsZipCode(code);

            return null;
        }

        private static string FindTimeZoneIdByCanadaPostalCode(string code)
        {
            var ch1 = code[0];

            var pacific = GetAbbreviation(PrivateIds.Pacific);
            var mountain = GetAbbreviation(PrivateIds.Mountain);
            var central = GetAbbreviation(PrivateIds.Central);
            var eastern = GetAbbreviation(PrivateIds.Eastern);
            var newfoundland = GetAbbreviation(PrivateIds.Newfoundland);
            var atlantic = GetAbbreviation(PrivateIds.Atlantic);

            if (ch1 == 'a')
            {
                return code == "a0p" || code == "a0r" || code == "a2v"
                    ? atlantic?.TimeZoneId
                    : newfoundland?.TimeZoneId;
            }
            else if (ch1 == 'b' || ch1 == 'c' || ch1 == 'e')
            {
                return atlantic?.TimeZoneId;
            }
            else if (ch1 == 'g')
            {
                return code == "g4t"
                    ? atlantic?.TimeZoneId
                    : eastern?.TimeZoneId;
            }
            else if (ch1 == 'h')
            {
                return eastern?.TimeZoneId;
            }
            else if (ch1 == 'j')
            {
                return code == "j4k"
                    ? atlantic?.TimeZoneId
                    : eastern?.TimeZoneId;
            }
            else if (ch1 == 'k' || ch1 == 'l' || ch1 == 'm' || ch1 == 'n')
            {
                return eastern?.TimeZoneId;
            }
            else if (ch1 == 'p')
            {
                return code == "p0v" || code == "p0w" || code == "p0x" || code == "p0y" || code == "p8n" || code == "p8t" || code == "p9a" || code == "p9n"
                    ? central?.TimeZoneId
                    : eastern?.TimeZoneId;
            }
            else if (ch1 == 'r')
            {
                return central?.TimeZoneId;
            }
            else if (ch1 == 's')
            {
                return code == "s9v"
                    ? mountain?.TimeZoneId
                    : central?.TimeZoneId;
            }
            else if (ch1 == 't')
            {
                return mountain?.TimeZoneId;
            }
            else if (ch1 == 'v')
            {
                return code == "v0a" || code == "v0b" || code == "v0c" || code == "v1a" || code == "v1c" || code == "v1g" || code == "v1j"
                    ? mountain?.TimeZoneId
                    : pacific?.TimeZoneId;
            }
            else if (ch1 == 'x')
            {
                return code == "x0a"
                    ? eastern?.TimeZoneId
                    : code == "x0c"
                        ? central?.TimeZoneId
                        : mountain?.TimeZoneId;
            }
            else if (ch1 == 'y')
            {
                return mountain?.TimeZoneId;
            }

            return null;
        }

        private static readonly HashSet<int> UsZipCode_Eastern = new HashSet<int>(new[] { 32456, 32457, 36863, 36867, 36868, 36869, 37302, 37303, 37304, 37314, 37315, 37316, 37317, 37320, 37321, 37322, 37323, 37325, 37326, 37329, 37331, 37332, 37333, 37336, 37337, 37341, 37343, 37350, 37351, 37353, 37354, 37361, 37362, 37363, 37364, 37369, 37370, 37371, 37373, 37377, 37379, 37381, 37384, 37385, 37391, 40117, 40118, 40121, 40122, 40129, 40142, 40150, 40175, 40176, 40177, 42631, 42633, 42634, 42635, 42638, 42647, 42649, 42653, 42701, 42702, 42716, 42718, 42719, 42724, 42732, 42733, 42740, 42748, 42758, 42776, 47516, 47519, 47521, 47522, 47524, 47527, 47528, 47529, 47532, 47535, 47553, 47575, 47578, 47580, 47581, 47584, 47585, 47590, 47591, 47596, 47597, 47598, 47944, 47946, 47949, 47950, 49805, 49806, 49807, 49808, 49814, 49816, 49817, 49818, 49819, 49820, 49822, 49825, 49826, 49827, 49829, 49833, 49849, 49853, 49854, 49855, 49861, 49862, 49864, 49865, 49866, 49868, 49871, 49872, 49878, 49879, 49880, 49883, 49884, 49885, 49891, 49894, 49895, 49901, 49905, 49908, 49910, 49912, 49913, 49916, 49917, 49918, 49919, 49921, 49922, 49925, 49929, 49930, 49931, 49934, 49942, 49945, 49946, 49960, 49961, 49962, 49963, 49965, 49967, 49970, 49971, 78597 });
        private static readonly HashSet<int> UsZipCode_Central = new HashSet<int>(new[] { 36865, 36866, 37305, 37306, 37313, 37318, 37324, 37327, 37328, 37330, 37334, 37335, 37338, 37339, 37340, 37342, 37345, 37347, 37348, 37349, 37352, 37355, 37356, 37357, 37359, 37360, 37365, 37366, 37367, 37374, 37375, 37376, 37378, 37380, 37382, 37383, 37387, 37388, 37389, 37394, 37396, 37397, 37398, 37501, 37544, 37723, 40111, 40115, 40119, 40140, 40143, 40144, 40145, 40146, 40152, 40153, 40170, 40171, 40178, 42602, 42603, 42629, 42642, 42712, 42713, 42715, 42717, 42720, 42721, 42722, 42726, 42728, 42729, 42741, 42742, 42743, 42746, 42749, 42753, 42754, 42755, 42757, 42759, 42762, 42764, 42765, 42782, 46531, 46532, 46534, 46968, 47514, 47515, 47520, 47523, 47525, 47531, 47536, 47537, 47550, 47551, 47552, 47556, 47574, 47576, 47577, 47579, 47586, 47588, 47922, 47943, 47948, 47951, 47963, 47964, 47977, 47978, 49801, 49802, 49812, 49815, 49821, 49831, 49834, 49845, 49847, 49848, 49852, 49858, 49863, 49870, 49873, 49874, 49876, 49877, 49881, 49886, 49887, 49892, 49893, 49896, 49902, 49903, 49911, 49915, 49920, 49927, 49935, 49938, 49947, 49959, 49964, 49968, 49969, 57538, 57540, 57541, 57544, 57548, 57568, 57569, 57570, 57571, 57572, 57576, 57579, 57580, 57584, 57585, 57601, 57631, 57632, 57646, 57648, 58530, 58531, 58532, 58563, 58565, 58566, 58568, 58631, 58638, 67734, 67736, 67737, 67738, 67739, 67740, 67764, 67801, 67831, 67834, 67835, 69022, 69024, 69025, 69026, 69028, 69029, 69032, 69042, 69043, 69044, 69046, 69101, 69103, 69120, 69123, 69130, 69132, 69135, 69138, 69142, 69143, 69151, 69157, 69161, 69163, 69165, 69166, 69167, 69169, 69170, 69171, 69201, 69210, 69212, 69214, 69217, 69220, 69221, 79830, 79831, 79832, 79834, 79842, 79843, 79845, 79846, 79848, 79852, 79854, 79855, 81090 });
        private static readonly HashSet<int> UsZipCode_Mountain = new HashSet<int>(new[] { 57521, 57537, 57543, 57547, 57551, 57552, 57553, 57567, 57574, 57577, 58529, 58533, 58535, 58562, 58564, 58569, 58632, 58634, 58636, 58838, 67733, 67735, 67741, 67758, 67761, 67762, 67836, 67857, 67878, 67879, 69021, 69023, 69027, 69030, 69033, 69041, 69045, 69121, 69122, 69125, 69127, 69128, 69129, 69131, 69133, 69134, 69140, 69141, 69152, 69153, 69154, 69155, 69156, 69160, 69162, 69168, 69190, 69211, 69216, 69218, 69219, 79314, 79821, 79835, 79836, 79837, 79838, 79839, 79847, 79849, 79851, 79853, 83547, 83549, 97901, 97902, 97903, 97906 });
        private static readonly HashSet<int> UsZipCode_Pacific = new HashSet<int>(new[] { 83548, 83552, 83553, 83554, 83555, 84753, 97904, 97905, 97907 });

        private static string FindTimeZoneIdByUsZipCode(string code)
        {
            var hawaiian = GetAbbreviation(PrivateIds.Hawaiian);
            var alaskan = GetAbbreviation(PrivateIds.Alaskan);
            var pacific = GetAbbreviation(PrivateIds.Pacific);
            var mountain = GetAbbreviation(PrivateIds.Mountain);
            var central = GetAbbreviation(PrivateIds.Central);
            var eastern = GetAbbreviation(PrivateIds.Eastern);

            var num = int.Parse(code);

            var isEasternRange =
                num >= 00501 && num <= 32399 ||
                num >= 32601 && num <= 34997 ||
                num >= 37307 && num <= 37312 ||
                num >= 37401 && num <= 37450 ||
                num >= 37601 && num <= 37722 ||
                num >= 37724 && num <= 37998 ||
                num >= 39813 && num <= 40110 ||
                num >= 40155 && num <= 40166 ||
                num >= 40201 && num <= 41862 ||
                num >= 42501 && num <= 42567 ||
                num >= 42784 && num <= 46298 ||
                num >= 46501 && num <= 46530 ||
                num >= 46536 && num <= 46967 ||
                num >= 46970 && num <= 47513 ||
                num >= 47541 && num <= 47549 ||
                num >= 47557 && num <= 47573 ||
                num >= 47801 && num <= 47921 ||
                num >= 47923 && num <= 47942 ||
                num >= 47952 && num <= 47962 ||
                num >= 47965 && num <= 47975 ||
                num >= 47980 && num <= 49799 ||
                num >= 49835 && num <= 49841 ||
                num >= 49948 && num <= 49958 ||
                num >= 56901 && num <= 56972;

            if (isEasternRange || UsZipCode_Eastern.Contains(num))
                return eastern?.TimeZoneId;

            var isCentralRange =
                num >= 32401 && num <= 32455 ||
                num >= 32459 && num <= 32591 ||
                num >= 35004 && num <= 36862 ||
                num >= 36870 && num <= 37301 ||
                num >= 38001 && num <= 39776 ||
                num >= 42001 && num <= 42464 ||
                num >= 46301 && num <= 46411 ||
                num >= 47601 && num <= 47750 ||
                num >= 50001 && num <= 56763 ||
                num >= 57001 && num <= 57520 ||
                num >= 57522 && num <= 57536 ||
                num >= 57555 && num <= 57566 ||
                num >= 58001 && num <= 58528 ||
                num >= 58538 && num <= 58561 ||
                num >= 58570 && num <= 58581 ||
                num >= 58701 && num <= 58835 ||
                num >= 58843 && num <= 58856 ||
                num >= 60001 && num <= 67732 ||
                num >= 67743 && num <= 67757 ||
                num >= 67837 && num <= 67855 ||
                num >= 67859 && num <= 67877 ||
                num >= 67880 && num <= 69020 ||
                num >= 69034 && num <= 69040 ||
                num >= 70001 && num <= 78596 ||
                num >= 78598 && num <= 79313 ||
                num >= 79316 && num <= 79789;

            if (isCentralRange || UsZipCode_Central.Contains(num))
                return central?.TimeZoneId;

            var isMountainRange =
                num >= 57620 && num <= 57630 ||
                num >= 57633 && num <= 57645 ||
                num >= 57649 && num <= 57799 ||
                num >= 58601 && num <= 58630 ||
                num >= 58639 && num <= 58656 ||
                num >= 59001 && num <= 59937 ||
                num >= 69144 && num <= 69150 ||
                num >= 69301 && num <= 69367 ||
                num >= 79901 && num <= 81089 ||
                num >= 81091 && num <= 83469 ||
                num >= 83601 && num <= 83799 ||
                num >= 84001 && num <= 84752 ||
                num >= 84754 && num <= 88595 ||
                num >= 97908 && num <= 97920;

            if (isMountainRange || UsZipCode_Mountain.Contains(num))
                return mountain?.TimeZoneId;

            var isPacificRange =
                num >= 83501 && num <= 83546 ||
                num >= 83801 && num <= 83877 ||
                num >= 88901 && num <= 96162 ||
                num >= 97001 && num <= 97886 ||
                num >= 98001 && num <= 99403;

            if (isPacificRange || UsZipCode_Pacific.Contains(num))
                return pacific?.TimeZoneId;

            if (num >= 99501 && num <= 99950)
                return alaskan?.TimeZoneId;

            if (num >= 96701 && num <= 96898)
                return hawaiian?.TimeZoneId;

            return null;
        }

        #endregion
    }
}
