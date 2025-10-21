using System.Web.UI;

using InSite.Application.Glossaries.Read;

namespace InSite.UI.Admin.Assets.Glossaries.Terms.Controls
{
    public partial class ApproveDetail : UserControl
    {
        public void SetInputValues(QGlossaryTerm term)
        {
            TermName.Text = term.TermName;

            var content = ServiceLocator.ContentSearch.GetBlock(term.TermIdentifier);

            TermTitle.Text = content.Title.GetText();

            TermDefinition.Text = content.Description.GetHtml();
            TermStatus.Text = term.TermStatus;
        }
    }
}