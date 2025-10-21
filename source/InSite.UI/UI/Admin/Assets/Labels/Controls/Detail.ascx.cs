using System.Collections.Generic;
using System.Web.UI;

using InSite.Application.Contents.Read;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Utilities.Labels.Controls
{
    public partial class Detail : UserControl
    {
        public void GetInputValues(List<TContent> contents)
        {
            var labelName = LabelName.Text;
            var translation = LabelTranslation.GetTranslation();

            var text = MultilingualString.Deserialize(translation);

            foreach (var lang in text.Languages)
            {
                var contentText = text[lang];
                if (contentText.IsEmpty())
                    continue;

                var parts = lang.Split(new[] { ':' });
                var contentLanguage = parts[0];
                var organizationId = OrganizationIdentifiers.Global;

                if (parts.Length > 1)
                {
                    var organization = OrganizationSearch.Select(parts[1]);
                    if (organization == null)
                        continue;

                    organizationId = organization.OrganizationIdentifier;
                }

                contents.Add(new TContent
                {
                    ContentIdentifier = UniqueIdentifier.Create(),

                    OrganizationIdentifier = organizationId,
                    ContainerType = ContentContainerType.Application,
                    ContainerIdentifier = LabelSearch.ContainerIdentifier,
                    ContentLabel = labelName,
                    ContentLanguage = contentLanguage,
                    ContentText = contentText
                });
            }
        }

        public void SetDefaultValues(bool allowOrganizationSpecific = false)
        {
            LabelName.Enabled = true;

            LabelTranslation.AllowOrganizationSpecific = !allowOrganizationSpecific;
            LabelTranslation.SetDefault();
        }

        public void SetInputValues(TContent[] contents)
        {
            if (contents.Length == 0)
                return;

            var unknownCounter = 1;
            var text = new MultilingualString();

            foreach (var content in contents)
            {
                string tag;

                if (content.OrganizationIdentifier == OrganizationIdentifiers.Global)
                {
                    tag = content.ContentLanguage;
                }
                else
                {
                    var organization = OrganizationSearch.Select(content.OrganizationIdentifier);
                    var orgCode = organization == null ? $"unknown{unknownCounter++}" : organization.OrganizationCode;
                    tag = $"{content.ContentLanguage}:{orgCode}";
                }

                text[tag] = content.ContentText;
            }

            var translation = text.Serialize();

            LabelTranslation.ShowExcludedLanguage = true;
            LabelName.Text = contents[0].ContentLabel;
            LabelTranslation.SetTranslation(translation, null);

            LabelName.Enabled = false;
        }
    }
}