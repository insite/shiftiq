using System;
using System.Linq;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Documents.Utilities
{
    public sealed class SectionInfo
    {
        public string ID { get; private set; }
        public string LabelID => $"[{CollectionName}].[{FolderName}].[{ItemName}]";

        private string CollectionName { get; set; }
        private string FolderName { get; set; }
        private string ItemName { get; set; }

        private SectionInfo()
        {
        }

        public string GetTitle()
        {
            return LabelHelper.GetTranslation(LabelID);
        }

        public string GetTitle(string lang)
        {
            return LabelHelper.GetTranslation(LabelID, lang);
        }

        public static SectionInfo[] GetDocumentSections(string name)
        {
            var result = GetDocumentSections(CurrentSessionState.Identity.Organization.Identifier, name);
            if (result.Length == 0)
                result = GetDocumentSections(OrganizationIdentifiers.Global, name);

            if (name.Equals("Customized Occupation Profile"))
                result = result
                    .ToList()
                    .Where(x =>
                        !x.LabelID.Equals("[Standards/Document/Content].[Customized Occupation Profile].[Acknowledgements]")
                        && !x.LabelID.Equals("[Standards/Document/Content].[Customized Occupation Profile].[Standards Development Process]")
                        && !x.LabelID.Equals("[Standards/Document/Content].[Customized Occupation Profile].[Certification Development Process]"))
                    .ToArray();

            return result;
        }

        private static SectionInfo[] GetDocumentSections(Guid organizationId, string name)
        {
            return TCollectionItemCache
                .Query(new TCollectionItemFilter
                {
                    OrganizationIdentifier = organizationId,
                    CollectionName = Shift.Constant.CollectionName.Standards_Document_Content,
                    ItemFolder = name
                })
                .OrderBy(x => x.ItemFolder).ThenBy(x => x.ItemSequence).ThenBy(x => x.ItemNumber)
                .Select(x => new SectionInfo
                {
                    ID = x.ItemName,
                    CollectionName = x.Collection.CollectionName,
                    FolderName = x.ItemFolder,
                    ItemName = x.ItemName,
                })
                .ToArray();
        }

        public static void BindComboBox(ComboBox combo)
        {
            combo.LoadItems(new[] {
                DocumentType.CompetencyClustering,
                DocumentType.CustomizedOccupationProfile,
                DocumentType.CustomizedSkillsChecklist,
                DocumentType.JobDescription,
                DocumentType.OccupationProfile,
                DocumentType.NationalOccupationStandard,
                DocumentType.SkillsChecklist
            });
        }
    }
}