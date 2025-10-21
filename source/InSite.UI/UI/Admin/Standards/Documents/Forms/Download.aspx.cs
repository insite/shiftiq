using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Standards.Documents.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private class StandardInfo
        {
            public Guid Key { get; set; }
            public Guid Identifier { get; set; }
            public string Title { get; set; }
            public int Sequence { get; set; }
            public ConnectionDirection Direction { get; set; }
        }

        protected Guid? AssetId => Guid.TryParse(Request.QueryString["asset"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var entity = AssetId.HasValue ? StandardSearch.SelectFirst(x => x.StandardIdentifier == AssetId.Value) : null;
                if (entity == null)
                {
                    HttpResponseHelper.Redirect("/ui/admin/standards/documents/search");
                    return;
                }

                PageHelper.AutoBindHeader(this, null,
                    $"{entity.ContentTitle ?? entity.ContentName ?? "Untitled"} <span class='form-text'>{entity.StandardType} Asset #{entity.AssetNumber}</span>");

                SetupDownloadSection(entity);

                CancelLink.NavigateUrl = $"/ui/admin/standards/documents/outline?asset={AssetId}";
            }
        }

        private void SetupDownloadSection(Standard document)
        {
            FileName.Text = string.Format("document-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            var content = ServiceLocator.ContentSearch.GetBlock(document.StandardIdentifier);
            var settings = ContentSettings.Deserialize(document.ContentSettings);

            TitleOutput.Text = content.Title.Text.Default.IfNullOrEmpty("None");

            var sections = SectionInfo.GetDocumentSections(document.DocumentType).Select(x => new
            {
                Title = x.GetTitle(),
                Content = content.GetHtml(x.ID).IfNullOrEmpty("None"),
            });

            SectionRepeater.DataSource = sections;
            SectionRepeater.DataBind();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
            {
                SendJson();
            }
        }

        private void SendJson()
        {
            var competencyData = StandardContainmentSearch.Bind(
            LinqExtensions1.Expr((StandardContainment x) =>
            InSite.Admin.Standards.Occupations.Utilities.Competencies.StandardInfo.Binder.Invoke(x.Child)).Expand(),
            x => x.ParentStandardIdentifier == AssetId.Value
              && x.Child.StandardType == StandardType.Competency);

            var outgoingRelationshipData = StandardContainmentSearch
                .Bind(
                    x => new StandardInfo
                    {
                        Key = x.Child.StandardIdentifier,
                        Identifier = x.Child.StandardIdentifier,
                        Title = x.Child.ContentTitle ?? CoreFunctions.GetContentTextEn(x.Child.StandardIdentifier, ContentLabel.Title),
                        Sequence = x.ChildSequence,
                        Direction = ConnectionDirection.Outgoing
                    },
                    x => x.ParentStandardIdentifier == AssetId.Value
                      && x.Child.StandardType == StandardType.Document,
                    "Sequence,Title").Select(x => new StandardContainment
                    {
                        ParentStandardIdentifier = AssetId.Value,
                        ChildStandardIdentifier = x.Identifier
                    }).ToList();

            var relationshipsSearch = new List<StandardContainment2Search>();

            foreach (var info in competencyData)
            {
                relationshipsSearch.Add(new StandardContainment2Search
                {
                    AssetNumber = info.AssetNumber,
                    Code = info.Code,
                    Title = info.Title,
                    Language = info.Language,
                    Description = info.Description,
                    Sequence = info.Sequence
                });
            }

            var data2 = StandardHelper.Serialize(
                StandardSearch.SelectFirst(x => x.StandardIdentifier == AssetId.Value),
                relationshipsSearch, ServiceLocator.ContentSearch.GetBlock(AssetId.Value));

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data2, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data2);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={AssetId}"
                : null;
        }
    }
}
