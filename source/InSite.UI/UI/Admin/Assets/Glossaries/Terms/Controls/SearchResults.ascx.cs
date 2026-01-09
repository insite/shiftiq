using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;
using InSite.Application.Glossaries.Write;
using InSite.Common.Web.UI;

using Shift.Common.Events;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Assets.Glossaries.Terms.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<GlossaryTermFilter>
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Classes

        public class DataItem
        {
            public Guid TermIdentifier => Term.TermIdentifier;
            public string TermTitleText => Content?.Title.GetText();
            public string TermDefinitionText => Content?.Description.GetText();
            public string TermDefinitionHtml => Content?.Description.GetHtml();
            public bool IsTranslated => Content != null && Content.Languages.Length > 1;

            public QGlossaryTerm Term { get; set; }
            public Shift.Common.ContentContainer Content { get; set; }

            public bool ShowTitle => !string.Equals(TermTitleText, Term.TermName, StringComparison.OrdinalIgnoreCase);
        }

        public class ExportDataItem
        {
            public Guid TermIdentifier { get; set; }
            public string TermName { get; set; }
            public string TermTitle { get; set; }
            public string TermDefinition { get; set; }
            public string TermStatus { get; set; }
            public DateTimeOffset Proposed { get; set; }
            public string ProposedBy { get; set; }
            public DateTimeOffset? Approved { get; set; }
            public string ApprovedBy { get; set; }
            public DateTimeOffset? LastRevised { get; set; }
            public string LastRevisedBy { get; set; }
            public int RevisionCount { get; set; }
            public bool IsTranslated { get; set; }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        #endregion

        #region Event handlers

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var termIdentifier = Grid.GetDataKey<Guid>(e);

            switch (e.CommandName)
            {
                case "Approve":
                    ServiceLocator.SendCommand(new ApproveGlossaryTerm(GlossaryHelper.GlossaryIdentifier, termIdentifier));
                    SearchWithCurrentPageIndex(Filter);
                    OnAlert(AlertType.Success, "Term has been approved");
                    break;

                case "Reject":
                    ServiceLocator.SendCommand(new RejectGlossaryTerm(GlossaryHelper.GlossaryIdentifier, termIdentifier));
                    SearchWithCurrentPageIndex(Filter);
                    OnAlert(AlertType.Success, "Term has been rejected");
                    break;
            }
        }

        #endregion

        protected override int SelectCount(GlossaryTermFilter filter)
            => ServiceLocator.GlossarySearch.CountTerms(filter);

        protected override IListSource SelectData(GlossaryTermFilter filter)
        {
            var data = ServiceLocator.GlossarySearch.GetTerms(Filter);

            var contents = ServiceLocator.ContentSearch.GetBlocks(
                data.Select(x => x.TermIdentifier), labels: new[] { ContentLabel.Title, ContentLabel.Description });

            return data
                .Select(x => new DataItem
                {
                    Term = x,
                    Content = contents.TryGetValue(x.TermIdentifier, out var content)
                        ? content
                        : null
                })
                .ToList()
                .ToSearchResult();
        }

        public override IListSource GetExportData(GlossaryTermFilter filter, bool empty)
        {
            var query = base.GetExportData(filter, empty).GetList().Cast<DataItem>().AsQueryable();

            if (empty)
                query = query.Take(0);

            return query.Select(x => new ExportDataItem
            {
                TermIdentifier = x.Term.TermIdentifier,
                TermName = x.Term.TermName,
                TermTitle = x.TermTitleText,
                TermDefinition = x.TermDefinitionText,
                TermStatus = x.Term.TermStatus,
                IsTranslated = x.IsTranslated,

                Proposed = x.Term.Proposed,
                ProposedBy = x.Term.ProposedBy,
                Approved = x.Term.Approved,
                ApprovedBy = x.Term.ApprovedBy,
                LastRevised = x.Term.LastRevised,
                LastRevisedBy = x.Term.LastRevisedBy,
                RevisionCount = x.Term.RevisionCount,
            }).ToList().ToSearchResult();
        }

        #region Helper methods

        protected string GetTimestamp()
        {
            var item = (DataItem)Page.GetDataItem();
            string on, by;

            switch (item.Term.TermStatus)
            {
                case "Approved":
                    on = item.Term.Approved.Humanize();
                    by = item.Term.ApprovedBy;
                    break;
                case "Proposed":
                    on = item.Term.Proposed.Humanize();
                    by = item.Term.ProposedBy;
                    break;
                case "Revised":
                    on = item.Term.LastRevised.Humanize();
                    by = item.Term.LastRevisedBy;
                    break;
                default:
                    return string.Empty;
            }

            return $"{item.Term.TermStatus} by {by} {on}";
        }

        protected string GetTermStatusHtml()
        {
            var item = (DataItem)Page.GetDataItem();

            switch (item.Term.TermStatus)
            {
                case "Approved":
                    return "<span class=\"badge bg-success\">Approved</span>";
                case "Proposed":
                    return "<span class=\"badge bg-danger\">Proposed</span>";
                case "Revised":
                    return "<span class=\"badge bg-primary\">Revised</span>";
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}