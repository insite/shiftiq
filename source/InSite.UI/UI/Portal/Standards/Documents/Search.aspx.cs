using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assets.Contents.Utilities;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Contract;

using DocumentTypeConst = Shift.Sdk.UI.DocumentType;

namespace InSite.UI.Portal.Standards.Documents
{
    public partial class Search : SearchPage<StandardDocumentFilter>
    {
        private string DefaultDocumentType => Request.QueryString["type"];

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var pageTitle = GetPageTitle(DefaultDocumentType, Translator);

            AutoBindFolderHeader(GetCreateItem(), null, pageTitle);

            PortalMaster.Breadcrumbs.BindTitleAndSubtitle(pageTitle, null);
        }

        public static string GetPageTitle(string documentType, InputTranslator translator)
        {
            var title = !string.IsNullOrEmpty(documentType) ? documentType + "s" : "Documents";
            return translator.Translate(title);
        }

        private BreadcrumbItem GetCreateItem()
        {
            if (string.Equals(DefaultDocumentType, DocumentTypeConst.NationalOccupationStandard, StringComparison.OrdinalIgnoreCase))
                return null;

            var createUrl = !string.IsNullOrEmpty(DefaultDocumentType)
                ? $"/ui/portal/standards/documents/create?type={DefaultDocumentType}"
                : "/ui/portal/standards/documents/create";

            return new BreadcrumbItem(Translate("Add New Document"), AddFolderToUrl(createUrl));
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return BaseSearchDownload.GetColumns(typeof(Controls.SearchResults.ExportDataItem)).OrderBy(x => x.Name);
        }
    }
}