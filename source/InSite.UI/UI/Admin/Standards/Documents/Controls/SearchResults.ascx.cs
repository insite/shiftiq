using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Standards.Documents.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<StandardDocumentFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public Guid StandardIdentifier { get; set; }
            public string DocumentType { get; set; }
            public string Title { get; set; }
            public string Level { get; set; }
            public DateTimeOffset? Posted { get; set; }
            public string IsTemplate { get; set; }
            public string PrivacyScope { get; set; }
            public string CreatedBy { get; set; }
        }

        #endregion

        #region Select Data

        protected override int SelectCount(StandardDocumentFilter filter)
        {
            return StandardSearch.Count(filter);
        }

        protected override IListSource SelectData(StandardDocumentFilter filter)
        {
            return StandardSearch.SelectSearchResults(filter);
        }

        #endregion

        #region Data Binding

        protected string GetPrivacyScope() => GetPrivacyScope(Page.GetDataItem());

        protected string GetPrivacyScope(object dataItem)
        {
            var scope = (string)DataBinder.Eval(dataItem, "StandardPrivacyScope");

            if (scope.IsEmpty() || scope == "Tenant")
                scope = "Organization";

            return scope;
        }

        private Dictionary<Guid, string> _createdBy = new Dictionary<Guid, string>();

        protected string GetCreatedBy() => GetCreatedBy(Page.GetDataItem());

        private string GetCreatedBy(object dataItem)
        {
            var userId = (Guid)DataBinder.Eval(dataItem, "CreatedBy");

            if (!_createdBy.TryGetValue(userId, out var userName))
                _createdBy.Add(userId, userName = UserSearch.GetFullName(userId));

            return userName;
        }

        #endregion

        #region Data Export

        public override IListSource GetExportData(StandardDocumentFilter filter, bool empty)
        {
            var data = SelectData(filter).GetList();
            var result = new List<ExportDataItem>();

            foreach (object row in data)
            {
                var item = new ExportDataItem
                {
                    StandardIdentifier = (Guid)DataBinder.Eval(row, "StandardIdentifier"),
                    DocumentType = Translate((string)DataBinder.Eval(row, "DocumentType")),
                    Title = (string)DataBinder.Eval(row, "ContentTitle"),
                    Level = (string)DataBinder.Eval(row, "LevelType"),
                    Posted = (DateTimeOffset?)DataBinder.Eval(row, "DatePosted"),
                    IsTemplate = (bool)DataBinder.Eval(row, "IsTemplate") ? "Yes" : "No",
                    PrivacyScope = GetPrivacyScope(row),
                    CreatedBy = GetCreatedBy(row)
                };

                result.Add(item);
            }

            return result.ToSearchResult();
        }

        #endregion
    }
}