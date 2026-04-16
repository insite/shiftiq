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
                .Bind(x => new { Identifier = x.UploadIdentifier, NavigateUrl = "/files" + x.NavigateUrl }, attachments);

            var files = ServiceLocator.FileSearch
                .GetModels(attachments.Where(x => x.FileIdentifier.HasValue).Select(x => x.FileIdentifier.Value).ToArray(), false)
                .Select(x => new { Identifier = x.FileIdentifier, NavigateUrl = ServiceLocator.StorageService.GetFileUrl(x) })
                .ToList();

            files.AddRange(uploads);

            var dictionary = files.ToDictionary(x => x.Identifier);

            var data = attachments.Where(x => dictionary.ContainsKey(x.FileIdentifier ?? x.Upload)).Select(x =>
            {
                var upload = dictionary[x.FileIdentifier ?? x.Upload];

                return new
                {
                    TypeName = x.Type.GetName(),
                    AssetNumber = x.Asset,
                    AssetVersion = x.AssetVersion,
                    Title = (x.Content?.Title.Default).IfNullOrEmpty("(Untitled)"),
                    Url = upload.NavigateUrl,
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