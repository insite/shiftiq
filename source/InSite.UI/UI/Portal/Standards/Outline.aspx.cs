using System;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Standards.Controls;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Standards
{
    public partial class Outline : PortalBasePage
    {
        protected Guid? StandardIdentifier => Guid.TryParse(Request["standard"], out var id) ? id : (Guid?)null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            AutoBindFolderHeader(null, Translate("Standard Outline"));
        }

        private void LoadData()
        {
            try
            {
                ContentContainer.Visible = StandardIdentifier.HasValue;

                var glossary = new GlossaryHelper(Identity.Language);

                var standard = StandardIdentifier.HasValue
                    ? StandardSearch.BindFirst(
                        x => new
                        {
                            x.StandardIdentifier,
                            x.Icon
                        },
                        x => x.StandardIdentifier == StandardIdentifier.Value)
                    : null;

                if (standard == null)
                {
                    StatusAlert.AddMessage(
                        AlertType.Error,
                        $"{Translator.Translate("Standard Not Found")}: {StandardIdentifier}. " +
                        Translator.Translate("Please contact your administrator with the steps you followed to arrive at this error message."));
                    return;
                }

                var content = ServiceLocator.ContentSearch.GetBlock(
                    standard.StandardIdentifier,
                    string.Empty,
                    new[] { ContentLabel.Title, ContentLabel.Summary });

                var title = glossary.Process(
                    standard.StandardIdentifier,
                    ContentLabel.Title,
                    content.Title.GetText(Identity.Language));

                AccordionPanel.Title = standard.Icon.IsNotEmpty()
                    ? $"<i class='fa {standard.Icon}'></i> {title}"
                    : title;
                PageSubtitle.InnerText = string.Empty;

                SummaryHtml.InnerHtml = glossary.Process(standard.StandardIdentifier, ContentLabel.Title, content.Summary.GetHtml(Identity.Language));

                OutlineTree.LoadData(StandardIdentifier.Value, glossary);
                OutlineTreeScript.TermsData = glossary.GetJavaScriptDictionary();
            }
            catch (Exception ex)
            {
                StatusAlert.AddMessage(AlertType.Error, ex.Message);
            }
        }
    }
}