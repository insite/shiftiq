using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Toolbox;

using ContentLabel = Shift.Constant.ContentLabel;
using GroupByColumn = Shift.Constant.GroupByColumn;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class QuestionCompetencyRatioRepeater : BaseUserControl
    {
        #region Classes

        public class QuestionDataItem
        {
            public Guid? OccupationIdentifier { get; set; }
            public string OccupationTitle { get; set; }

            public Guid? FrameworkIdentifier { get; set; }
            public string FrameworkTitle { get; set; }

            public Guid? CompetencyAreaIdentifier { get; set; }
            public string CompetencyAreaCode { get; set; }
            public string CompetencyAreaTitle { get; set; }

            public Guid AttemptIdentifier { get; set; }
            public string AttemptTag { get; set; }
            public DateTimeOffset? AttemptStarted { get; internal set; }
            public DateTimeOffset? AttemptGraded { get; internal set; }
            public bool AttemptIsPassing { get; set; }
            public string EventFormat { get; set; }

            public Guid FormIdentifier { get; set; }
            public int FormAsset { get; set; }
            public string FormName { get; set; }
            public string FormTitle { get; set; }

            public Guid LearnerUserIdentifier { get; set; }

            public Guid QuestionIdentifier { get; set; }
            public decimal? AnswerPoints { get; set; }
        }

        private abstract class BaseDataItem
        {
            #region Properties

            public Guid ID { get; }

            #endregion

            #region Construction

            public BaseDataItem(Guid id)
            {
                ID = id;
            }

            #endregion

            #region Methods

            public abstract void AppendWrites(HashSet<Guid> collection);
            public abstract void AppendFails(HashSet<Guid> collection);
            public abstract void AppendScores(Dictionary<MultiKey<Guid, Guid>, double> scores);

            #endregion
        }

        private class NodeDataItem : BaseDataItem, IEnumerable<BaseDataItem>
        {
            #region Properties

            public string Type { get; }

            public string Code { get; set; }
            public string Title { get; set; }

            public string TitleText => ID == Guid.Empty ? "N/A" : (string.IsNullOrEmpty(Code) ? string.Empty : $"{Code}. ") + Title;
            public string TitleHtml => ID == Guid.Empty
                ? "<strong>N/A</strong>"
                : string.IsNullOrEmpty(Title)
                    ? "<i>(No Title)</i>"
                    : WebUtility.HtmlEncode(TitleText);

            public int Writes
            {
                get
                {
                    var collection = new HashSet<Guid>();

                    AppendWrites(collection);

                    return collection.Count;
                }
            }

            public int Fails
            {
                get
                {
                    var collection = new HashSet<Guid>();

                    AppendFails(collection);

                    return collection.Count;
                }
            }

            public double Score
            {
                get
                {
                    var scores = new Dictionary<MultiKey<Guid, Guid>, double>();

                    AppendScores(scores);

                    return scores.Count > 0 ? scores.Values.Sum() / scores.Count : 0;
                }
            }

            public int TotalWrites => _parent == null ? Writes : _parent.TotalWrites;

            #endregion

            #region Fields

            private NodeDataItem _parent;
            private Dictionary<Guid, BaseDataItem> _children = new Dictionary<Guid, BaseDataItem>();

            #endregion

            #region Construction

            public NodeDataItem(Guid id, string type)
                : base(id)
            {
                Type = type;
            }

            #endregion

            #region Methods

            protected bool SetParent(NodeDataItem parent)
            {
                var isValid = _parent == null;

                if (isValid)
                    _parent = parent;

                return isValid;
            }

            public void Add(NodeDataItem item)
            {
                if (!_children.ContainsKey(item.ID) && item.SetParent(this))
                    _children.Add(item.ID, item);
            }

            public void Add(LeafDataItem item)
            {
                if (!_children.ContainsKey(item.ID))
                    _children.Add(item.ID, item);
            }

            public BaseDataItem Get(Guid id) => _children[id];

            public bool Contains(Guid id) => _children.ContainsKey(id);

            public override void AppendWrites(HashSet<Guid> collection)
            {
                foreach (var c in _children.Values)
                    c.AppendWrites(collection);
            }

            public override void AppendFails(HashSet<Guid> collection)
            {
                foreach (var c in _children.Values)
                    c.AppendFails(collection);
            }

            public override void AppendScores(Dictionary<MultiKey<Guid, Guid>, double> scores)
            {
                foreach (var c in _children.Values)
                    c.AppendScores(scores);
            }

            #endregion

            #region IEnumerable

            public IEnumerator<BaseDataItem> GetEnumerator() => _children.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        private class LeafDataItem : BaseDataItem
        {
            #region Fields

            private bool _isPassing;
            private List<Tuple<Guid, double>> _scores = new List<Tuple<Guid, double>>();

            #endregion

            #region Construction

            public LeafDataItem(QuestionDataItem question)
                : base(question.AttemptIdentifier)
            {
                _isPassing = question.AttemptIsPassing;
            }

            #endregion

            #region Methods

            public void AddScore(Guid id, double value)
            {
                _scores.Add(new Tuple<Guid, double>(id, value));
            }

            public override void AppendWrites(HashSet<Guid> collection)
            {
                if (!collection.Contains(ID))
                    collection.Add(ID);
            }

            public override void AppendFails(HashSet<Guid> collection)
            {
                if (!_isPassing && !collection.Contains(ID))
                    collection.Add(ID);
            }

            public override void AppendScores(Dictionary<MultiKey<Guid, Guid>, double> scores)
            {
                foreach (var score in _scores)
                {
                    var key = new MultiKey<Guid, Guid>(ID, score.Item1);
                    if (!scores.ContainsKey(key))
                        scores.Add(key, score.Item2);
                }
            }

            #endregion
        }

        public interface IRatioDataItem
        {
            string Type { get; }
            string TitleText { get; }
            string TitleHtml { get; }

            int Writes { get; }
            int Fails { get; }
            double Score { get; }

            double WritesRatio { get; }
            double PassRate { get; }
            double PassRateRatio { get; }
            double ScoreRatio { get; }
        }

        [Serializable]
        private class RatioDataItem : IRatioDataItem
        {
            #region Properties

            public string Type { get; }
            public string TitleText { get; }
            public string TitleHtml { get; }

            public int Writes { get; }
            public int Fails { get; }
            public double Score { get; }

            public double WritesRatio { get; }
            public double PassRate { get; }
            public double PassRateRatio { get; }
            public double ScoreRatio { get; }

            public int Depth { get; }

            #endregion

            #region Construction

            public RatioDataItem(NodeDataItem node, int depth)
            {
                Type = node.Type;
                TitleText = node.TitleText;
                TitleHtml = node.TitleHtml;

                Writes = node.Writes;
                Fails = node.Fails;
                Score = node.Score;

                WritesRatio = node.TotalWrites > 0 ? node.Writes / (double)node.TotalWrites : 0;
                PassRate = node.Writes > 0 && node.Fails <= node.Writes ? (node.Writes - node.Fails) / (double)node.Writes : 0;
                PassRateRatio = PassRate * WritesRatio;
                ScoreRatio = node.Score * WritesRatio;

                Depth = depth;
            }

            #endregion
        }

        #endregion

        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) =>
            Alert?.Invoke(this, new AlertArgs(type, message));

        #endregion

        #region Properties

        public IRatioDataItem[] DataItems
        {
            get => (IRatioDataItem[])ViewState[nameof(DataItems)];
            private set => ViewState[nameof(DataItems)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.ItemDataBound += Repeater_ItemDataBound;

            ExportButton.Click += ExportButton_Click;
        }

        #endregion

        #region Event handlers

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var rootItem = DataItems.First();

                var writes = (ITextControl)e.Item.FindControl("Writes");
                var writesRatio = (ITextControl)e.Item.FindControl("WritesRatio");
                var fails = (ITextControl)e.Item.FindControl("Fails");
                var passRate = (ITextControl)e.Item.FindControl("PassRate");
                var passRateRatio = (ITextControl)e.Item.FindControl("PassRateRatio");
                var score = (ITextControl)e.Item.FindControl("Score");
                var scoreRatio = (ITextControl)e.Item.FindControl("ScoreRatio");

                writes.Text = rootItem.Writes.ToString("n0");
                writesRatio.Text = rootItem.WritesRatio.ToString("n4");
                fails.Text = rootItem.Fails.ToString("n0");
                passRate.Text = rootItem.PassRate.ToString("n4");
                passRateRatio.Text = rootItem.PassRateRatio.ToString("n4");
                score.Text = rootItem.Score.ToString("n2");
                scoreRatio.Text = rootItem.ScoreRatio.ToString("n4");
            }
        }

        private void ExportButton_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "default" || e.CommandName == "XLSX")
                ExportXlsx();
            else if (e.CommandName == "CSV")
                ExportCsv();
        }

        #endregion

        #region Methods (data binding)

        public void LoadData(IEnumerable<QuestionDataItem> questions, GroupByColumn[] groupBy)
        {
            if (!questions.Any())
            {
                OnAlert(AlertType.Information, "No Data.");
                Repeater.Visible = false;
                ExportButton.Visible = false;
                return;
            }

            Repeater.Visible = true;
            ExportButton.Visible = true;

            var dataSource = GetDataSource(questions, groupBy);

            DataItems = dataSource.ToArray();

            Repeater.DataSource = dataSource.Skip(1);
            Repeater.DataBind();
        }

        private static List<RatioDataItem> GetDataSource(IEnumerable<QuestionDataItem> questions, GroupByColumn[] groupBy)
        {
            var root = new NodeDataItem(Guid.Empty, "Root");
            var tagPool = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
            var formatPool = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            var groupGetters = new List<Func<QuestionDataItem, NodeDataItem, NodeDataItem>>();

            if (groupBy.IsEmpty())
                groupBy = new[] { GroupByColumn.Occupation, GroupByColumn.Framework, GroupByColumn.Tag, GroupByColumn.Gac, GroupByColumn.Form, GroupByColumn.Format };

            foreach (var groupId in groupBy)
            {
                if (groupId == GroupByColumn.Occupation)
                {
                    groupGetters.Add((question, parent) =>
                    {
                        var occupationId = question.OccupationIdentifier ?? Guid.Empty;
                        if (parent.Contains(occupationId))
                            return (NodeDataItem)parent.Get(occupationId);

                        var result = new NodeDataItem(occupationId, StandardType.Profile)
                        {
                            Title = question.OccupationTitle
                        };

                        parent.Add(result);

                        return result;
                    });
                }
                else if (groupId == GroupByColumn.Framework)
                {
                    groupGetters.Add((question, parent) =>
                    {
                        var frameworkId = question.FrameworkIdentifier ?? Guid.Empty;
                        if (parent.Contains(frameworkId))
                            return (NodeDataItem)parent.Get(frameworkId);

                        var result = new NodeDataItem(frameworkId, "Framework")
                        {
                            Title = question.FrameworkTitle
                        };

                        parent.Add(result);

                        return result;
                    });
                }
                else if (groupId == GroupByColumn.Tag)
                {
                    groupGetters.Add((question, parent) =>
                    {
                        Guid tagId;

                        if (string.IsNullOrEmpty(question.AttemptTag))
                            tagId = Guid.Empty;
                        else if (tagPool.ContainsKey(question.AttemptTag))
                            tagId = tagPool[question.AttemptTag];
                        else
                            tagPool.Add(question.AttemptTag, tagId = UniqueIdentifier.Create());

                        if (parent.Contains(tagId))
                            return (NodeDataItem)parent.Get(tagId);

                        var result = new NodeDataItem(tagId, "Tag")
                        {
                            Title = question.AttemptTag
                        };

                        parent.Add(result);

                        return result;
                    });
                }
                else if (groupId == GroupByColumn.Gac)
                {
                    groupGetters.Add((question, parent) =>
                    {
                        var gacId = question.CompetencyAreaIdentifier ?? Guid.Empty;
                        if (parent.Contains(gacId))
                            return (NodeDataItem)parent.Get(gacId);

                        var result = new NodeDataItem(gacId, Shift.Constant.StandardType.Area)
                        {
                            Code = question.CompetencyAreaCode,
                            Title = question.CompetencyAreaTitle
                        };

                        parent.Add(result);

                        return result;
                    });
                }
                else if (groupId == GroupByColumn.Form)
                {
                    groupGetters.Add((question, parent) =>
                    {
                        var formId = question.FormIdentifier;
                        if (parent.Contains(formId))
                            return (NodeDataItem)parent.Get(formId);

                        var result = new NodeDataItem(formId, "Form")
                        {
                            Title = question.FormName ?? $"{question.FormAsset}: {question.FormTitle}"
                        };

                        parent.Add(result);

                        return result;
                    });
                }
                else if (groupId == GroupByColumn.Format)
                {
                    groupGetters.Add((question, parent) =>
                    {
                        Guid formatId;

                        if (question.EventFormat.IsEmpty())
                            formatId = Guid.Empty;
                        else if (formatPool.ContainsKey(question.EventFormat))
                            formatId = formatPool[question.EventFormat];
                        else
                            formatPool.Add(question.EventFormat, formatId = UniqueIdentifier.Create());

                        if (parent.Contains(formatId))
                            return (NodeDataItem)parent.Get(formatId);

                        var result = new NodeDataItem(formatId, "Format")
                        {
                            Title = question.EventFormat
                        };

                        parent.Add(result);

                        return result;
                    });
                }
            }

            var assetItems = new Dictionary<Guid, NodeDataItem>();

            foreach (var question in questions)
            {
                var group = root;
                foreach (var getGroup in groupGetters)
                {
                    group = getGroup(question, group);

                    if (!assetItems.ContainsKey(group.ID) && (group.Type == Shift.Constant.StandardType.Profile || group.Type == StandardType.Framework || group.Type == StandardType.Area))
                        assetItems.Add(group.ID, group);
                }

                LeafDataItem submissionItem;
                if (!group.Contains(question.AttemptIdentifier))
                    group.Add(submissionItem = new LeafDataItem(question));
                else
                    submissionItem = (LeafDataItem)group.Get(question.AttemptIdentifier);

                submissionItem.AddScore(question.QuestionIdentifier, question.AnswerPoints.HasValue ? (double)question.AnswerPoints.Value : 0);
            }

            var assets = StandardSearch.Bind(x => new
            {
                ID = x.StandardIdentifier,
                Subtype = x.StandardType,
                Code = x.Code,
                Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title),
            }, x => assetItems.Keys.Contains(x.StandardIdentifier));

            foreach (var asset in assets)
            {
                var item = assetItems[asset.ID];

                item.Code = asset.Code;
                item.Title = asset.Title;
            }

            var dataSource = new List<RatioDataItem>();

            FlattenDataSource(new[] { root }, dataSource, 0);

            return dataSource;
        }

        private static void FlattenDataSource(IEnumerable<NodeDataItem> input, List<RatioDataItem> output, int depth)
        {
            foreach (var inputItem in input.OrderBy(x => x.ID != Guid.Empty).ThenBy(x => x.TitleText))
            {
                output.Add(new RatioDataItem(inputItem, depth));
                if (inputItem.Any(x => x is NodeDataItem))
                    FlattenDataSource(inputItem.Cast<NodeDataItem>(), output, depth + 1);
            }
        }

        #endregion

        #region Methods (export)

        private IEnumerable GetExportData()
        {
            var dataItems = DataItems;
            if (dataItems.Length <= 1)
                return null;

            return dataItems.Skip(1).Select(x => new
            {
                Name = x.TitleText,
                Type = x.Type,
                Writes = x.Writes,
                WritesRatio = Math.Round(x.WritesRatio, 4),
                Fails = x.Fails,
                PassRate = Math.Round(x.PassRate, 4),
                PassRateRatio = Math.Round(x.PassRateRatio, 4),
                Score = Math.Round(x.Score, 4),
                ScoreRatio = Math.Round(x.ScoreRatio, 4),
            });
        }

        private void ExportXlsx()
        {
            var dataItems = GetExportData();
            if (dataItems == null)
                return;

            var helper = new XlsxExportHelper();

            helper.Map("Name", "Name", 45, HorizontalAlignment.Left);
            helper.Map("Type", "Type", 15, HorizontalAlignment.Left);
            helper.Map("Writes", "Writes", 15, HorizontalAlignment.Right);
            helper.Map("WritesRatio", "Writes Ratio", 15, HorizontalAlignment.Right);
            helper.Map("Fails", "Fails", 15, HorizontalAlignment.Right);
            helper.Map("PassRate", "Pass Rate", 15, HorizontalAlignment.Right);
            helper.Map("PassRateRatio", "Pass Rate Ratio", 15, HorizontalAlignment.Right);
            helper.Map("Score", "Average", 15, HorizontalAlignment.Right);
            helper.Map("ScoreRatio", "Average Ratio", 15, HorizontalAlignment.Right);

            var xlsxBytes = helper.GetXlsxBytes(dataItems, "Ratio");
            var filename = string.Format("ratio_report-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            Page.Response.SendFile(filename, "xlsx", xlsxBytes);
        }

        private void ExportCsv()
        {
            var dataItems = GetExportData();
            if (dataItems == null)
                return;

            var helper = new CsvExportHelper(dataItems);

            helper.AddMapping("Name", "Name");
            helper.AddMapping("Type", "Type");
            helper.AddMapping("Writes", "Writes");
            helper.AddMapping("WritesRatio", "Writes Ratio");
            helper.AddMapping("Fails", "Fails");
            helper.AddMapping("PassRate", "Pass Rate");
            helper.AddMapping("PassRateRatio", "Pass Rate Ratio");
            helper.AddMapping("Score", "Average");
            helper.AddMapping("ScoreRatio", "Average Ratio");

            var csvBytes = helper.GetBytes(Encoding.UTF8);
            var filename = string.Format("ratio_report-{0:yyyyMMdd}-{0:HHmmss}", DateTime.UtcNow);

            Page.Response.SendFile(filename, "csv", csvBytes);
        }

        #endregion

        #region Methods (helpers)

        protected static string GetScoreText(decimal score)
        {
            var color = score > 0.75m ? "success" : (score > 0.5m ? "warning" : "danger");
            return $"<span class='text-{color}'>{score:p0}</span>";
        }

        protected static string GetScoreLabel(decimal score)
        {
            var color = score > 0.75m ? "success" : (score > 0.50m ? "warning" : "danger");
            return $"<span class='badge bg-{color}'>{score:p0}</span>";
        }

        protected static string GetPoints(decimal points, int count) =>
            $"{points:n2} / {count:n2}";

        #endregion
    }
}