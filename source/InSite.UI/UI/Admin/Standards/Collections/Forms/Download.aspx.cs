using System;
using System.Collections.Generic;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Standards.Collections.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? StandardId => Guid.TryParse(Request.QueryString["asset"], out var value) ? value : (Guid?)null;
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
                var standard = StandardId.HasValue ? StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardId) : null;
                if (standard == null || standard.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
                {
                    HttpResponseHelper.Redirect("/ui/admin/standards/collections/search");
                    return;
                }

                var title =
                    $"{standard.ContentTitle ?? standard.ContentName ?? "Untitled"} " +
                    $"<span class='form-text'>{standard.StandardType} Asset #{standard.AssetNumber}</span>";

                PageHelper.AutoBindHeader(this, null, title);

                SetupDownloadSection(standard);

                CancelLink.NavigateUrl = $"/ui/admin/standards/collections/outline?asset={StandardId.Value}";
            }
        }

        private void SetupDownloadSection(Standard entity)
        {
            FileName.Text = string.Format("collection-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);
            StandardCollectionDetails.SetInputValues(entity);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            //TODO: For now we only have one format hence i am grabbing JSON value which is always selected like that
            //TODO: If we decide on more download types then we can extend this RadioButton selection
            var fileFormat = JSONFileFormat.Value;

            if (fileFormat == "JSON")
                SendJson();
        }

        private void SendJson()
        {
            var competencyData = StandardContainmentSearch.Bind(
                LinqExtensions1.Expr((StandardContainment x) =>
                InSite.Admin.Standards.Occupations.Utilities.Competencies.StandardInfo.Binder.Invoke(x.Child)).Expand(),
                x => x.ParentStandardIdentifier == StandardId.Value
                  && x.Child.StandardType == StandardType.Competency);

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

            var data = StandardHelper.Serialize(
                StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardId), relationshipsSearch);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(data, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", data);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={StandardId.Value}"
                : null;
        }
    }
}