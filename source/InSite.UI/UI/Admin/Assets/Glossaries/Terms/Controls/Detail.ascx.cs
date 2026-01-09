using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Glossaries.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assets.Glossaries.Terms.Controls
{
    public partial class Detail : UserControl
    {
        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DefinitionRequiredValidator.ServerValidate += DefinitionRequiredValidator_ServerValidate;
        }

        #endregion

        #region Event handlers

        private void DefinitionRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var text = TermDefinition.Translation;

            args.IsValid = !text.IsEmpty && text.Default.IsNotEmpty();
        }

        #endregion

        #region Setting and getting input values

        internal void SetDefaultInputValues(Guid glossaryId, Guid termId)
        {
            SetInputValues(glossaryId, termId, null);
        }

        public void SetInputValues(QGlossaryTerm term, Shift.Common.ContentContainer content)
        {
            TermName.Text = term.TermName;

            SetInputValues(term.GlossaryIdentifier, term.OrganizationIdentifier, content);
        }

        private void SetInputValues(Guid glossaryId, Guid termId, Shift.Common.ContentContainer content)
        {
            if (content == null)
                content = new Shift.Common.ContentContainer();

            TermTitle.SetOptions(new AssetContentSection.SingleLine(ContentLabel.Title)
            {
                Value = content.Title.Text,
            });
            TermTitle.InputLanguage = CurrentSessionState.Identity.Language;

            TermDefinition.SetOptions(new AssetContentSection.Markdown(ContentLabel.Description)
            {
                Value = content.Description.Text,
                AllowUpload = true,
                UploadFolderPath = string.Format(OrganizationRelativePath.GlossaryPathTemplate, glossaryId, "terms", termId)
            });
            TermDefinition.InputLanguage = CurrentSessionState.Identity.Language;
        }

        public void GetInputValues(QGlossaryTerm term, Shift.Common.ContentContainer content)
        {
            term.TermName = TermName.Text;

            content.Title.Text = TermTitle.Translation;
            content.Description.Text = TermDefinition.Translation;
        }

        #endregion
    }
}