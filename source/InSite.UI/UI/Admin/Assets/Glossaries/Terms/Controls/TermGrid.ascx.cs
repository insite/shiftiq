using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;
using InSite.Application.Glossaries.Write;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Assets.Glossaries.Terms.Controls
{
    public partial class TermGrid : SearchResultsGridViewController<NullFilter>
    {
        #region Events

        public event EventHandler Updated;

        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Classes

        private class DataItem
        {
            public QGlossaryTermContent Entity => _entity;

            public Guid RelationshipIdentifier => _entity.RelationshipIdentifier;
            public Guid TermIdentifier => _entity.TermIdentifier;
            public string TermName => _entity.Term.TermName;

            public string TermTitleText { get; set; }
            public string TermDefinitionHtml { get; set; }
            public Tuple<string, int>[] References { get; set; }

            public bool ShowTitle => !string.Equals(TermTitleText, _entity.Term.TermName, StringComparison.OrdinalIgnoreCase);

            private QGlossaryTermContent _entity;

            public DataItem(QGlossaryTermContent entity)
            {
                _entity = entity;
            }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        private Guid ContainerID
        {
            get => (Guid)ViewState[nameof(ContainerID)];
            set => ViewState[nameof(ContainerID)] = value;
        }

        private string ContainerType
        {
            get => (string)ViewState[nameof(ContainerType)];
            set => ViewState[nameof(ContainerType)] = value;
        }

        private MultilingualString[] ContainerContents
        {
            get => (MultilingualString[])ViewState[nameof(ContainerContents)];
            set => ViewState[nameof(ContainerContents)] = value;
        }

        private string ContentLabel
        {
            get => (string)ViewState[nameof(ContentLabel)];
            set => ViewState[nameof(ContentLabel)] = value;
        }

        protected bool AllowEdit
        {
            get => (bool)ViewState[nameof(AllowEdit)];
            set => ViewState[nameof(AllowEdit)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FilterButton.Click += FilterButton_Click;
            AddButton.Click += AddButton_Click;

            Grid.RowCommand += Grid_RowCommand;
        }

        #endregion

        #region Event handlers

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ServiceLocator.SendCommand(new LinkGlossaryTerm(
                GlossaryHelper.GlossaryIdentifier,
                UniqueIdentifier.Create(),
                AddTermIdentifier.Value.Value,
                ContainerID,
                ContainerType,
                ContentLabel));

            AddTermIdentifier.Value = null;

            SearchWithCurrentPageIndex(new NullFilter());

            OnUpdated();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            Search(new NullFilter());
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var row = GridViewExtensions.GetRow(e);
            var relationshipId = Grid.GetDataKey<Guid>(row, "RelationshipIdentifier");
            var termId = Grid.GetDataKey<Guid>(row, "TermIdentifier");

            if (e.CommandName == "Remove")
            {
                ServiceLocator.SendCommand(new UnlinkGlossaryTerm(GlossaryHelper.GlossaryIdentifier, relationshipId, termId));
                SearchWithCurrentPageIndex(new NullFilter());
                OnUpdated();
            }
        }

        #endregion

        #region Public methods

        public void LoadData(Guid containerId, string containerType, IEnumerable<MultilingualString> contents, string contentLabel, bool allowEdit)
        {
            AllowEdit = allowEdit;

            AddPanel.Visible = allowEdit;

            ContainerID = containerId;
            ContainerType = containerType;
            ContainerContents = contents?.ToArray() ?? new MultilingualString[0];
            ContentLabel = contentLabel;
            FilterTextBox.Text = null;

            Search(new NullFilter());
        }

        public int GetTotalCount()
        {
            var filter = GetFilter(null);
            return ServiceLocator.GlossarySearch.CountTermContents(filter);
        }

        #endregion

        #region Search results

        protected override int SelectCount(NullFilter _)
        {
            var filter = GetFilter(FilterTextBox.Text);
            var count = ServiceLocator.GlossarySearch.CountTermContents(filter);

            FilterContainer.Visible = count > 0;

            return count;
        }

        protected override IListSource SelectData(NullFilter nullFilter)
        {
            var filter = GetFilter(FilterTextBox.Text);
            filter.Paging = nullFilter.Paging;

            var languages = ContainerContents.SelectMany(x => x.Languages).Append(ContentContainer.DefaultLanguage).Distinct().ToArray();
            var glossaries = new List<GlossaryHelper>();
            var data = ServiceLocator.GlossarySearch.GetTermContents(filter);

            {
                var terms = data.Select(x => x.Term).ToArray();

                foreach (var lang in languages)
                {
                    var glossaryHelper = new GlossaryHelper(lang);

                    foreach (var content in ContainerContents)
                        glossaryHelper.Process(terms, content[lang]);

                    glossaries.Add(glossaryHelper);
                }
            }

            var contents = ServiceLocator.ContentSearch.GetBlocks(
                data.Select(x => x.TermIdentifier),
                languages,
                new[] { Shift.Constant.ContentLabel.Title, Shift.Constant.ContentLabel.Description });

            AddTermIdentifier.Filter.ExcludeTermIdentifiers = new List<Guid>();

            var result = data
                .Select(x =>
                {
                    AddTermIdentifier.Filter.ExcludeTermIdentifiers.Add(x.TermIdentifier);

                    var item = new DataItem(x);
                    if (!contents.TryGetValue(x.TermIdentifier, out var content))
                        return item;

                    item.TermTitleText = content.GetText(Shift.Constant.ContentLabel.Title);
                    item.TermDefinitionHtml = content.GetHtml(Shift.Constant.ContentLabel.Description);
                    item.References = glossaries
                        .Select(g => new Tuple<string, int>(
                            g.Language,
                            g.Dictionary.Where(r => r.TermID == x.TermIdentifier).Sum(r => r.RefCount)))
                        .ToArray();

                    return item;
                }).ToList().ToSearchResult();

            AddTermIdentifier.Value = null;

            return result;
        }

        #endregion

        #region Helper methods

        private GlossaryTermContentFilter GetFilter(string keyword)
        {
            return new GlossaryTermContentFilter
            {
                ContainerIdentifier = ContainerID,
                ContentLabel = ContentLabel,
            };
        }

        protected string GetTimestamp()
        {
            var item = (DataItem)Page.GetDataItem();

            string status, on, by;

            switch (item.Entity.Term.TermStatus)
            {
                case "Approved":
                    status = "<span class=\"badge bg-success\">Approved</span>";
                    on = item.Entity.Term.Approved.Humanize();
                    by = item.Entity.Term.ApprovedBy;
                    break;
                case "Proposed":
                    status = "<span class=\"badge bg-danger\">Proposed</span>";
                    on = item.Entity.Term.Proposed.Humanize();
                    by = item.Entity.Term.ProposedBy;
                    break;
                case "Revised":
                    status = "<span class=\"badge bg-primary\">Revised</span>";
                    on = item.Entity.Term.LastRevised.Humanize();
                    by = item.Entity.Term.LastRevisedBy;
                    break;
                default:
                    return string.Empty;
            }

            return $"{status} by {by} {on}";
        }

        protected string GetReferencesHtml()
        {
            var item = (DataItem)Page.GetDataItem();

            if (item.References.Length == 0 || item.References.All(x => x.Item2 == 0))
                return "<strong>Not Found</strong>";

            var sb = new StringBuilder();

            sb.Append("<table class='table table-condensed table-refs'><tbody>");

            foreach (var r in item.References.OrderBy(x => x.Item1))
                sb.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>", r.Item1.ToUpper(), r.Item2);

            sb.Append("</tbody></table>");

            return sb.ToString();
        }

        #endregion   
    }
}