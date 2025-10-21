using System;
using System.Web.UI;

using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class FastProgramSection : UserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        public void BindControlsToModel(Person person)
        {

        }

        public void SaveUploads(Person person)
        {
            UploadHelper.SaveContactUpload(person.UserIdentifier, "FastDocument", "fast-document", FastDocumentUpload);
        }

        public void BindModelToControls(Person person)
        {
            var file = TCandidateUploadSearch.SelectFirst(x => x.CandidateUserIdentifier == person.UserIdentifier && x.UploadType == "FastDocument");

            FastDocumentUrl.Text = file != null ? UploadHelper.GetFileRelativePath(file.FileIdentifier) : null;
        }
    }
}