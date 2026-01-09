using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Attachments.Controls
{
    public partial class QuestionAttachmentRepeater : BaseUserControl
    {
        #region Properties

        public int RowCount
        {
            get => (int)(ViewState[nameof(RowCount)] ?? 0);
            private set => ViewState[nameof(RowCount)] = value;
        }

        #endregion

        #region Methods

        public void LoadData(Question question)
        {
            var attachments = question.Set.Bank.EnumerateAllAttachments().Where(x => question.AttachmentIdentifiers.Contains(x.Identifier));
            var uploads = UploadSearch
                .Bind(x => new { x.UploadIdentifier, x.Name, x.NavigateUrl, x.ContentSize, x.Uploaded }, attachments)
                .ToDictionary(x => x.UploadIdentifier);

            var data = attachments.Where(x => uploads.ContainsKey(x.Upload)).Select(x =>
            {
                var upload = uploads[x.Upload];

                return new
                {
                    TypeName = x.Type.GetName(),
                    AssetNumber = x.Asset,
                    AssetVersion = x.AssetVersion,
                    Title = (x.Content?.Title.Default).IfNullOrEmpty("(Untitled)"),
                    Url = "/files" + upload.NavigateUrl,
                    PublicationStatus = x.PublicationStatus.GetDescription(),
                };
            }).OrderBy(x => x.Title).ThenBy(x => x.AssetNumber).ToArray();

            RowCount = data.Length;

            Repeater.DataSource = data;
            Repeater.DataBind();
        }

        #endregion
    }
}