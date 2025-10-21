using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

using static InSite.Persistence.LabelSearch;

namespace InSite.UI.Admin.Assets.Labels.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<LabelFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public string ContentLabel { get; set; }
            public string Values { get; set; }
        }

        private class SearchDataItem
        {
            public string ContentLabel { get; set; }
            public List<LanguageItem> Languages { get; set; }
        }

        #endregion

        protected override int SelectCount(LabelFilter filter)
        {
            return LabelSearch.Search(filter).Count;
        }

        protected override IListSource SelectData(LabelFilter filter)
        {
            var data = LabelSearch.Search(filter);

            return data.Select(x => new SearchDataItem
                {
                    ContentLabel = x.ContentLabel,
                    Languages = x.Languages
                })
                .ApplyPaging(filter)
                .ToList()
                .ToSearchResult();
        }

        public override IListSource GetExportData(LabelFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                ContentLabel = x.ContentLabel,
                Values = CreateExcelText(x.Languages)
            }).ToList().ToSearchResult();
        }


        protected string GetLabelIdentifier(object contentLabel)
        {
            return HttpUtility.UrlEncode((string)contentLabel);
        }

        protected string CreateExcelText(object obj)
        {
            var languages = (List<LanguageItem>)obj;

            var sb = new StringBuilder();
            foreach (var language in languages)
            {
                sb.AppendFormat("{0} {1} ", language.Tag, language.Text);
            }
            return sb.ToString();
        }

        protected string CreateHtmlTable(object obj)
        {
            var languages = (List<LanguageItem>)obj;

            var sb = new StringBuilder();
            sb.Append("<table>");
            foreach (var language in languages)
            {
                sb.Append("<tr>");
                sb.AppendFormat("<td style='font-weight:bold;width:100px;padding:5px;'>{0}</td><td style='padding:5px;'>{1}</td><td><span class='badge bg-primary'>{2}</span></td>", language.Tag, language.Text, language.Count);
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }
    }
}