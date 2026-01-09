using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.File;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public class FormFileUpload : Control
    {
        private class FileQueueItemTemplate : ITemplate
        {
            void ITemplate.InstantiateIn(Control container)
            {
                var div = new HtmlGenericControl("div") { ID = "Div" };
                container.Controls.Add(div);
                div.Attributes["class"] = "file-row";

                var fileName = new System.Web.UI.WebControls.Literal { ID = "FileName" };
                div.Controls.Add(fileName);

                var a = new HtmlAnchor { ID = "A", HRef = "#" };
                div.Controls.Add(a);
                a.Attributes["style"] = "margin-left:5px;";
                a.Attributes["title"] = "Delete";
                a.InnerHtml = "<i class='far fa-trash-alt'></i>";
            }
        }

        public event EventHandler RemoveClicked;

        public Guid? SessionIdentifier
        {
            get => (Guid?)ViewState[nameof(SessionIdentifier)];
            set => ViewState[nameof(SessionIdentifier)] = value;
        }

        public Guid QuestionIdentifier
        {
            get => (Guid)ViewState[nameof(QuestionIdentifier)];
            set => ViewState[nameof(QuestionIdentifier)] = value;
        }

        public string ExistingRelativeUrls
        {
            get => (string)ViewState[nameof(ExistingRelativeUrls)];
            set => ViewState[nameof(ExistingRelativeUrls)] = value;
        }

        public bool IsRequired
        {
            get => ViewState[nameof(IsRequired)] as bool? ?? false;
            set => ViewState[nameof(IsRequired)] = value;
        }

        private FileUploadV2 _fileUpload;
        public FileUploadV2 FileUpload
        {
            get
            {
                EnsureChildControls();
                return _fileUpload;
            }
        }

        private Panel _linkPanel;
        private Panel LinkPanel
        {
            get
            {
                EnsureChildControls();
                return _linkPanel;
            }
        }

        private Repeater _fileQueueRepeater;
        private Repeater FileQueueRepeater
        {
            get
            {
                EnsureChildControls();
                return _fileQueueRepeater;
            }
        }

        private System.Web.UI.WebControls.Literal _existingFileLinks;
        private System.Web.UI.WebControls.Literal ExistingFileLinks
        {
            get
            {
                EnsureChildControls();
                return _existingFileLinks;
            }
        }

        private LinkButton _removeButton;
        private LinkButton RemoveButton
        {
            get
            {
                EnsureChildControls();
                return _removeButton;
            }
        }

        private Panel _fileQueue;
        private Panel FileQueuePanel
        {
            get
            {
                EnsureChildControls();
                return _fileQueue;
            }
        }

        private HiddenField _uploadedFilesField;
        private HiddenField UploadedFilesField
        {
            get
            {
                EnsureChildControls();
                return _uploadedFilesField;
            }
        }

        public UploadFileInfo[] UploadedFiles
        {
            get
            {
                var none = new UploadFileInfo[0];
                var json = UploadedFilesField.Value;

                try
                {
                    if (json.HasValue())
                        return Deserialize(json);
                }
                catch (Newtonsoft.Json.JsonSerializationException ex)
                {
                    AppSentry.SentryWarning(ex.Message + "\r\nJSON: " + (json ?? "None"));
                }

                return none;
            }
        }

        private UploadFileInfo[] Deserialize(string json)
        {
            var result = new List<UploadFileInfo>();
            var parts = json.Split(new[] { '\t' });
            foreach (var part in parts)
                result.AddRange(ServiceLocator.Serializer.Deserialize<UploadFileInfo[]>(part));
            return result.ToArray();
        }

        public void ClearUploadedFiles()
        {
            UploadedFilesField.Value = null;
            FileUpload.ClearFiles();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(_fileQueue = new Panel
            {
                ID = nameof(FileQueuePanel),
                CssClass = "file-queue"
            });

            _fileQueue.Controls.Add(_uploadedFilesField = new HiddenField
            {
                ID = nameof(UploadedFilesField)
            });

            _fileQueue.Controls.Add(_fileQueueRepeater = new Repeater
            {
                ID = nameof(FileQueueRepeater),
                ItemTemplate = new FileQueueItemTemplate(),
            });

            _fileQueueRepeater.ItemDataBound += FileQueueRepeater_ItemDataBound;

            Controls.Add(_fileUpload = new FileUploadV2
            {
                ID = nameof(FileUpload),
                LabelText = string.Empty,
                OnClientFileUploaded = "answerPage.onFileUploaded",
                MaxFileSize = 20 * 1024 * 1024,
                ResponseSessionIdentifier = SessionIdentifier
            });

            Controls.Add(_linkPanel = new Panel { ID = nameof(LinkPanel) });
            _linkPanel.Style.Add(HtmlTextWriterStyle.PaddingTop, "5px");

            _linkPanel.Controls.Add(new LiteralControl("Uploaded File(s): "));

            _linkPanel.Controls.Add(_existingFileLinks = new System.Web.UI.WebControls.Literal { ID = nameof(ExistingFileLinks) });

            _linkPanel.Controls.Add(_removeButton = new LinkButton { ID = nameof(RemoveButton), ToolTip = "Delete File(s)", Text = "<i class='far fa-trash-alt ms-3'></i>" });
            _removeButton.Click += RemoveButton_Click;
            _removeButton.OnClientClick = "return confirm('Are you sure you want to delete the uploaded file(s)?');";
        }

        protected override void OnPreRender(EventArgs e)
        {
            EnsureChildControls();

            base.OnPreRender(e);

            FileQueueRepeater.DataSource = UploadedFiles;
            FileQueueRepeater.DataBind();
        }

        public override void DataBind()
        {
            EnsureChildControls();

            LinkPanel.Visible = ExistingRelativeUrls.HasValue();

            if (ExistingRelativeUrls.HasValue())
            {
                var links = new StringBuilder();
                var urls = StringHelper.Split(ExistingRelativeUrls);

                foreach (var url in urls)
                {
                    var name = Path.GetFileName(url);
                    links.Append($"<a target='_blank' href='{url}' class='ms-3'>{name}</a>");
                }

                ExistingFileLinks.Text = links.ToString();
            }

            // RequiredValidator.Visible = IsRequired && string.IsNullOrEmpty(ExistingRelativeUrls);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RemoveClicked?.Invoke(this, new EventArgs());
        }

        private void FileQueueRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var metadata = (UploadFileInfo)e.Item.DataItem;
            var div = (HtmlGenericControl)e.Item.FindControl("Div");
            var fileName = (System.Web.UI.WebControls.Literal)e.Item.FindControl("FileName");

            div.Attributes["data-upload"] = ServiceLocator.Serializer.Serialize(metadata);

            fileName.Text = metadata.FileName;
        }
    }
}