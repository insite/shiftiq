using System;
using System.Collections.Specialized;

namespace Shift.Constant
{
    /// <summary>
    /// Flat color palette (full) downloaded from
    /// - https://htmlcolorcodes.com/color-chart/
    /// 
    /// Check out our powerful color picker
    /// - https://htmlcolorcodes.com/color-picker/
    /// 
    /// Want to learn more about colors in SCSS?
    /// - http://htmlcolorcodes.com/tutorials/
    /// </summary>
    public static class FlatColorCodes
    {
        private static readonly StringDictionary _dictionary = new StringDictionary();

        public static string GetCode(int index)
        {
            var value = (FlatColorNames)index;

            if (!Enum.IsDefined(typeof(FlatColorNames), value))
                throw new ArgumentOutOfRangeException(nameof(index));

            return GetCode(value);
        }

        public static string GetCode(FlatColorNames name)
        {
            return _dictionary[GetKey(name)];
        }

        private static string GetKey(FlatColorNames name)
        {
            switch (name)
            {
                case FlatColorNames.Alizarin: return "alizarin";
                case FlatColorNames.Amethyst: return "amethyst";
                case FlatColorNames.Asbestos: return "asbestos";
                case FlatColorNames.BelizeHole: return "belize-hole";
                case FlatColorNames.Carrot: return "carrot";
                case FlatColorNames.Clouds: return "clouds";
                case FlatColorNames.Concrete: return "concrete";
                case FlatColorNames.Emerald: return "emerald";
                case FlatColorNames.GreenSea: return "green-sea";
                case FlatColorNames.MidnightBlue: return "midnight-blue";
                case FlatColorNames.Nephritis: return "nephritis";
                case FlatColorNames.Orange: return "orange";
                case FlatColorNames.PeterRiver: return "peter-river";
                case FlatColorNames.Pomegranate: return "pomegranate";
                case FlatColorNames.Pumpkin: return "pumpkin";
                case FlatColorNames.Silver: return "silver";
                case FlatColorNames.Sunflower: return "sunflower";
                case FlatColorNames.Turquoise: return "turquoise";
                case FlatColorNames.WetAsphalt: return "wet-asphalt";
                case FlatColorNames.Wisteria: return "wisteria";
                default: throw new ArgumentOutOfRangeException(nameof(name));
            }
        }

        public static string GetCode(ToolkitName toolkit)
        {
            return _dictionary[GetCode(toolkit.ToString())];
        }

        public static string GetCode(string toolkit)
        {
            switch (toolkit.ToLower())
            {
                case "accounts": case "identities": return GetCode(0);
                case "assessments": case "banks": return GetCode(2);
                case "assets": case "contents": case "glossaries": return GetCode(5);
                case "contacts": case "locations": return GetCode(4);
                case "courses": case "resources": return GetCode(6);
                case "databases": case "backups": case "toolbox": return GetCode(3);
                case "events": case "registrations": return GetCode(8);
                case "issues": return GetCode(10);
                case "logs": return GetCode(11);
                case "messages": return GetCode(12);
                case "records": case "achievements": return GetCode(1);
                case "reports": return GetCode(15);
                case "sales": case "invoices": case "payments": case "jobs": return GetCode(9);
                case "settings": case "utilities": return GetCode(16);
                case "sites": return GetCode(17);
                case "standards": return GetCode(18);
                case "surveys": return GetCode(19);

                case "addons": 
                case "custom": 
                case "custom_bch": 
                case "custom_cmds": 
                case "custom_iecbc": 
                case "custom_ita": 
                case "custom_ncsha": 
                case "custom_rcabc": return GetCode(7);

                default: throw new ArgumentOutOfRangeException(nameof(toolkit), "Unexpected toolkit name: " + toolkit);
            }
        }

        static FlatColorCodes()
        {
            /* Turquoise */
            _dictionary.Add("turquoise", "#1abc9c");
            _dictionary.Add("turquoise-50", "#e8f8f5");
            _dictionary.Add("turquoise-100", "#d1f2eb");
            _dictionary.Add("turquoise-200", "#a3e4d7");
            _dictionary.Add("turquoise-300", "#76d7c4");
            _dictionary.Add("turquoise-400", "#48c9b0");
            _dictionary.Add("turquoise-500", "#1abc9c");
            _dictionary.Add("turquoise-600", "#17a589");
            _dictionary.Add("turquoise-700", "#148f77");
            _dictionary.Add("turquoise-800", "#117864");
            _dictionary.Add("turquoise-900", "#0e6251");

            /* Green Sea */
            _dictionary.Add("green-sea", "#16a085");
            _dictionary.Add("green-sea-50", "#e8f6f3");
            _dictionary.Add("green-sea-100", "#d0ece7");
            _dictionary.Add("green-sea-200", "#a2d9ce");
            _dictionary.Add("green-sea-300", "#73c6b6");
            _dictionary.Add("green-sea-400", "#45b39d");
            _dictionary.Add("green-sea-500", "#16a085");
            _dictionary.Add("green-sea-600", "#138d75");
            _dictionary.Add("green-sea-700", "#117a65");
            _dictionary.Add("green-sea-800", "#0e6655");
            _dictionary.Add("green-sea-900", "#0b5345");

            /* Emerald */
            _dictionary.Add("emerald", "#2ecc71");
            _dictionary.Add("emerald-50", "#eafaf1");
            _dictionary.Add("emerald-100", "#d5f5e3");
            _dictionary.Add("emerald-200", "#abebc6");
            _dictionary.Add("emerald-300", "#82e0aa");
            _dictionary.Add("emerald-400", "#58d68d");
            _dictionary.Add("emerald-500", "#2ecc71");
            _dictionary.Add("emerald-600", "#28b463");
            _dictionary.Add("emerald-700", "#239b56");
            _dictionary.Add("emerald-800", "#1d8348");
            _dictionary.Add("emerald-900", "#186a3b");

            /* Nephritis */
            _dictionary.Add("nephritis", "#27ae60");
            _dictionary.Add("nephritis-50", "#e9f7ef");
            _dictionary.Add("nephritis-100", "#d4efdf");
            _dictionary.Add("nephritis-200", "#a9dfbf");
            _dictionary.Add("nephritis-300", "#7dcea0");
            _dictionary.Add("nephritis-400", "#52be80");
            _dictionary.Add("nephritis-500", "#27ae60");
            _dictionary.Add("nephritis-600", "#229954");
            _dictionary.Add("nephritis-700", "#1e8449");
            _dictionary.Add("nephritis-800", "#196f3d");
            _dictionary.Add("nephritis-900", "#145a32");

            /* Peter River */
            _dictionary.Add("peter-river", "#3498db");
            _dictionary.Add("peter-river-50", "#ebf5fb");
            _dictionary.Add("peter-river-100", "#d6eaf8");
            _dictionary.Add("peter-river-200", "#aed6f1");
            _dictionary.Add("peter-river-300", "#85c1e9");
            _dictionary.Add("peter-river-400", "#5dade2");
            _dictionary.Add("peter-river-500", "#3498db");
            _dictionary.Add("peter-river-600", "#2e86c1");
            _dictionary.Add("peter-river-700", "#2874a6");
            _dictionary.Add("peter-river-800", "#21618c");
            _dictionary.Add("peter-river-900", "#1b4f72");

            /* Belize Hole */
            _dictionary.Add("belize-hole", "#2980b9");
            _dictionary.Add("belize-hole-50", "#eaf2f8");
            _dictionary.Add("belize-hole-100", "#d4e6f1");
            _dictionary.Add("belize-hole-200", "#a9cce3");
            _dictionary.Add("belize-hole-300", "#7fb3d5");
            _dictionary.Add("belize-hole-400", "#5499c7");
            _dictionary.Add("belize-hole-500", "#2980b9");
            _dictionary.Add("belize-hole-600", "#2471a3");
            _dictionary.Add("belize-hole-700", "#1f618d");
            _dictionary.Add("belize-hole-800", "#1a5276");
            _dictionary.Add("belize-hole-900", "#154360");

            /* Amethyst */
            _dictionary.Add("amethyst", "#9b59b6");
            _dictionary.Add("amethyst-50", "#f5eef8");
            _dictionary.Add("amethyst-100", "#ebdef0");
            _dictionary.Add("amethyst-200", "#d7bde2");
            _dictionary.Add("amethyst-300", "#c39bd3");
            _dictionary.Add("amethyst-400", "#af7ac5");
            _dictionary.Add("amethyst-500", "#9b59b6");
            _dictionary.Add("amethyst-600", "#884ea0");
            _dictionary.Add("amethyst-700", "#76448a");
            _dictionary.Add("amethyst-800", "#633974");
            _dictionary.Add("amethyst-900", "#512e5f");

            /* Wisteria */
            _dictionary.Add("wisteria", "#8e44ad");
            _dictionary.Add("wisteria-50", "#f4ecf7");
            _dictionary.Add("wisteria-100", "#e8daef");
            _dictionary.Add("wisteria-200", "#d2b4de");
            _dictionary.Add("wisteria-300", "#bb8fce");
            _dictionary.Add("wisteria-400", "#a569bd");
            _dictionary.Add("wisteria-500", "#8e44ad");
            _dictionary.Add("wisteria-600", "#7d3c98");
            _dictionary.Add("wisteria-700", "#6c3483");
            _dictionary.Add("wisteria-800", "#5b2c6f");
            _dictionary.Add("wisteria-900", "#4a235a");

            /* Wet Asphalt */
            _dictionary.Add("wet-asphalt", "#34495e");
            _dictionary.Add("wet-asphalt-50", "#ebedef");
            _dictionary.Add("wet-asphalt-100", "#d6dbdf");
            _dictionary.Add("wet-asphalt-200", "#aeb6bf");
            _dictionary.Add("wet-asphalt-300", "#85929e");
            _dictionary.Add("wet-asphalt-400", "#5d6d7e");
            _dictionary.Add("wet-asphalt-500", "#34495e");
            _dictionary.Add("wet-asphalt-600", "#2e4053");
            _dictionary.Add("wet-asphalt-700", "#283747");
            _dictionary.Add("wet-asphalt-800", "#212f3c");
            _dictionary.Add("wet-asphalt-900", "#1b2631");

            /* Midnight Blue */
            _dictionary.Add("midnight-blue", "#2c3e50");
            _dictionary.Add("midnight-blue-50", "#eaecee");
            _dictionary.Add("midnight-blue-100", "#d5d8dc");
            _dictionary.Add("midnight-blue-200", "#abb2b9");
            _dictionary.Add("midnight-blue-300", "#808b96");
            _dictionary.Add("midnight-blue-400", "#566573");
            _dictionary.Add("midnight-blue-500", "#2c3e50");
            _dictionary.Add("midnight-blue-600", "#273746");
            _dictionary.Add("midnight-blue-700", "#212f3d");
            _dictionary.Add("midnight-blue-800", "#1c2833");
            _dictionary.Add("midnight-blue-900", "#17202a");

            /* Sunflower */
            _dictionary.Add("sunflower", "#f1c40f");
            _dictionary.Add("sunflower-50", "#fef9e7");
            _dictionary.Add("sunflower-100", "#fcf3cf");
            _dictionary.Add("sunflower-200", "#f9e79f");
            _dictionary.Add("sunflower-300", "#f7dc6f");
            _dictionary.Add("sunflower-400", "#f4d03f");
            _dictionary.Add("sunflower-500", "#f1c40f");
            _dictionary.Add("sunflower-600", "#d4ac0d");
            _dictionary.Add("sunflower-700", "#b7950b");
            _dictionary.Add("sunflower-800", "#9a7d0a");
            _dictionary.Add("sunflower-900", "#7d6608");

            /* Orange */
            _dictionary.Add("orange", "#f39c12");
            _dictionary.Add("orange-50", "#fef5e7");
            _dictionary.Add("orange-100", "#fdebd0");
            _dictionary.Add("orange-200", "#fad7a0");
            _dictionary.Add("orange-300", "#f8c471");
            _dictionary.Add("orange-400", "#f5b041");
            _dictionary.Add("orange-500", "#f39c12");
            _dictionary.Add("orange-600", "#d68910");
            _dictionary.Add("orange-700", "#b9770e");
            _dictionary.Add("orange-800", "#9c640c");
            _dictionary.Add("orange-900", "#7e5109");

            /* Carrot */
            _dictionary.Add("carrot", "#e67e22");
            _dictionary.Add("carrot-50", "#fdf2e9");
            _dictionary.Add("carrot-100", "#fae5d3");
            _dictionary.Add("carrot-200", "#f5cba7");
            _dictionary.Add("carrot-300", "#f0b27a");
            _dictionary.Add("carrot-400", "#eb984e");
            _dictionary.Add("carrot-500", "#e67e22");
            _dictionary.Add("carrot-600", "#ca6f1e");
            _dictionary.Add("carrot-700", "#af601a");
            _dictionary.Add("carrot-800", "#935116");
            _dictionary.Add("carrot-900", "#784212");

            /* Pumpkin */
            _dictionary.Add("pumpkin", "#d35400");
            _dictionary.Add("pumpkin-50", "#fbeee6");
            _dictionary.Add("pumpkin-100", "#f6ddcc");
            _dictionary.Add("pumpkin-200", "#edbb99");
            _dictionary.Add("pumpkin-300", "#e59866");
            _dictionary.Add("pumpkin-400", "#dc7633");
            _dictionary.Add("pumpkin-500", "#d35400");
            _dictionary.Add("pumpkin-600", "#ba4a00");
            _dictionary.Add("pumpkin-700", "#a04000");
            _dictionary.Add("pumpkin-800", "#873600");
            _dictionary.Add("pumpkin-900", "#6e2c00");

            /* Alizarin */
            _dictionary.Add("alizarin", "#e74c3c");
            _dictionary.Add("alizarin-50", "#fdedec");
            _dictionary.Add("alizarin-100", "#fadbd8");
            _dictionary.Add("alizarin-200", "#f5b7b1");
            _dictionary.Add("alizarin-300", "#f1948a");
            _dictionary.Add("alizarin-400", "#ec7063");
            _dictionary.Add("alizarin-500", "#e74c3c");
            _dictionary.Add("alizarin-600", "#cb4335");
            _dictionary.Add("alizarin-700", "#b03a2e");
            _dictionary.Add("alizarin-800", "#943126");
            _dictionary.Add("alizarin-900", "#78281f");

            /* Pomegranate */
            _dictionary.Add("pomegranate", "#c0392b");
            _dictionary.Add("pomegranate-50", "#f9ebea");
            _dictionary.Add("pomegranate-100", "#f2d7d5");
            _dictionary.Add("pomegranate-200", "#e6b0aa");
            _dictionary.Add("pomegranate-300", "#d98880");
            _dictionary.Add("pomegranate-400", "#cd6155");
            _dictionary.Add("pomegranate-500", "#c0392b");
            _dictionary.Add("pomegranate-600", "#a93226");
            _dictionary.Add("pomegranate-700", "#922b21");
            _dictionary.Add("pomegranate-800", "#7b241c");
            _dictionary.Add("pomegranate-900", "#641e16");

            /* Clouds */
            _dictionary.Add("clouds", "#ecf0f1");
            _dictionary.Add("clouds-50", "#fdfefe");
            _dictionary.Add("clouds-100", "#fbfcfc");
            _dictionary.Add("clouds-200", "#f7f9f9");
            _dictionary.Add("clouds-300", "#f4f6f7");
            _dictionary.Add("clouds-400", "#f0f3f4");
            _dictionary.Add("clouds-500", "#ecf0f1");
            _dictionary.Add("clouds-600", "#d0d3d4");
            _dictionary.Add("clouds-700", "#b3b6b7");
            _dictionary.Add("clouds-800", "#979a9a");
            _dictionary.Add("clouds-900", "#7b7d7d");

            /* Silver */
            _dictionary.Add("silver", "#bdc3c7");
            _dictionary.Add("silver-50", "#f8f9f9");
            _dictionary.Add("silver-100", "#f2f3f4");
            _dictionary.Add("silver-200", "#e5e7e9");
            _dictionary.Add("silver-300", "#d7dbdd");
            _dictionary.Add("silver-400", "#cacfd2");
            _dictionary.Add("silver-500", "#bdc3c7");
            _dictionary.Add("silver-600", "#a6acaf");
            _dictionary.Add("silver-700", "#909497");
            _dictionary.Add("silver-800", "#797d7f");
            _dictionary.Add("silver-900", "#626567");

            /* Concrete */
            _dictionary.Add("concrete", "#95a5a6");
            _dictionary.Add("concrete-50", "#f4f6f6");
            _dictionary.Add("concrete-100", "#eaeded");
            _dictionary.Add("concrete-200", "#d5dbdb");
            _dictionary.Add("concrete-300", "#bfc9ca");
            _dictionary.Add("concrete-400", "#aab7b8");
            _dictionary.Add("concrete-500", "#95a5a6");
            _dictionary.Add("concrete-600", "#839192");
            _dictionary.Add("concrete-700", "#717d7e");
            _dictionary.Add("concrete-800", "#5f6a6a");
            _dictionary.Add("concrete-900", "#4d5656");

            /* Asbestos */
            _dictionary.Add("asbestos", "#7f8c8d");
            _dictionary.Add("asbestos-50", "#f2f4f4");
            _dictionary.Add("asbestos-100", "#e5e8e8");
            _dictionary.Add("asbestos-200", "#ccd1d1");
            _dictionary.Add("asbestos-300", "#b2babb");
            _dictionary.Add("asbestos-400", "#99a3a4");
            _dictionary.Add("asbestos-500", "#7f8c8d");
            _dictionary.Add("asbestos-600", "#707b7c");
            _dictionary.Add("asbestos-700", "#616a6b");
            _dictionary.Add("asbestos-800", "#515a5a");
            _dictionary.Add("asbestos-900", "#424949");
        }
    }
}