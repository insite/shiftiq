using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    internal static class ReactQuestionFilterDeserializer
    {
        private class ReactFilter
        {
            public Guid? CompetencyId { get; set; }
            public FlagType[] Flags { get; set; }
            public string[] Conditions { get; set; }
            public int? Taxonomy { get; set; }
            public bool? HasLig { get; set; }
            public bool? HasReference { get; set; }
        }

        public static QuestionFilter Deserialize(string serializedFilter)
        {
            if (string.IsNullOrEmpty(serializedFilter) || !serializedFilter.StartsWith("."))
                return null;

            var index = serializedFilter.IndexOf("&filter=");
            if (index < 0)
                return null;

            try
            {
                var onlyFilterPart = serializedFilter.Substring(index + "&filter=".Length);
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(onlyFilterPart));
                var filter = JsonConvert.DeserializeObject<ReactFilter>(json);

                return new QuestionFilter
                {
                    StandardIdentifier = filter.CompetencyId,
                    Flag = new HashSet<FlagType>(filter.Flags),
                    Condition = new HashSet<string>(filter.Conditions),
                    Taxonomy = filter.Taxonomy,
                    HasLig = filter.HasLig,
                    HasReference = filter.HasReference
                };
            }
            catch
            {
                return null;
            }
        }
    }
}