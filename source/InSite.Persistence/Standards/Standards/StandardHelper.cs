using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence
{
    public static class StandardHelper
    {
        public class StandardJson
        {
            public ContentContainer Content { get; set; }
            public string AuthorName { get; set; }
            public string Code { get; set; }
            public string CompetencyScoreCalculationMethod { get; set; }
            public string CompetencyScoreSummarizationMethod { get; set; }
            public string ContentDescription { get; set; }
            public string ContentName { get; set; }
            public string ContentSettings { get; set; }
            public string ContentSummary { get; set; }
            public string ContentTitle { get; set; }
            public string CreditIdentifier { get; set; }
            public string DocumentType { get; set; }
            public string Icon { get; set; }
            public string Language { get; set; }
            public string LevelCode { get; set; }
            public string LevelType { get; set; }
            public string SourceDescriptor { get; set; }
            public string StandardLabel { get; set; }
            public string StandardPrivacyScope { get; set; }
            public string StandardTier { get; set; }
            public string StandardType { get; set; }
            public string Tags { get; set; }

            public int AssetNumber { get; set; }
            public int Sequence { get; set; }

            public List<StandardContainment2Search> Items { get; set; }
        }

        public static byte[] Serialize(Persistence.Standard standard,
            List<StandardContainment2Search> standardInfo, ContentContainer content = null)
        {
            var data = new StandardJson
            {
                Sequence = standard.Sequence,
                Language = standard.Language,
                ContentDescription = standard.ContentDescription,
                AssetNumber = standard.AssetNumber,
                AuthorName = standard.AuthorName,
                Code = standard.Code,
                CompetencyScoreCalculationMethod = standard.CompetencyScoreCalculationMethod,
                CompetencyScoreSummarizationMethod = standard.CompetencyScoreSummarizationMethod,
                ContentName = standard.ContentName,
                ContentSettings = standard.ContentSettings,
                ContentSummary = standard.ContentSummary,
                ContentTitle = standard.ContentTitle,
                CreditIdentifier = standard.CreditIdentifier,
                DocumentType = standard.DocumentType,
                Icon = standard.Icon,
                LevelCode = standard.LevelCode,
                LevelType = standard.LevelType,
                SourceDescriptor = standard.SourceDescriptor,
                StandardLabel = standard.StandardLabel,
                StandardPrivacyScope = standard.StandardPrivacyScope,
                StandardTier = standard.StandardTier,
                StandardType = standard.StandardType,
                Tags = standard.Tags,
                Items = standardInfo ?? new List<StandardContainment2Search>(),
            };

            if (content != null)
                data.Content = content;

            var json = JsonHelper.JsonExport(data);

            return Encoding.UTF8.GetBytes(json);

        }

        public static StandardJson DeserializeStandard(string json)
        {
            try
            {
                return JsonHelper.JsonImport<StandardJson>(json);
            }
            catch (JsonReaderException)
            {

            }
            catch (ApplicationError)
            {

            }

            return null;
        }
    }
}
