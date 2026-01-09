using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Humanizer;

using InSite.Domain.Organizations;
using InSite.Domain.Reports;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;
using Shift.Common.Events;

namespace InSite.Common.Web.UI
{
    public abstract class BaseSearchDownload : BaseUserControl
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class JsonColumnState
        {
            [JsonProperty(PropertyName = "number")]
            public int Sequence { get; private set; }

            [JsonProperty(PropertyName = "visible")]
            public bool Visible { get; private set; }

            [JsonConstructor]
            private JsonColumnState()
            {

            }

            public JsonColumnState(DownloadColumnState state)
            {
                Sequence = state.Sequence;
                Visible = !state.Hidden;
            }
        }

        private class ActionFieldKey : MultiKey<Guid, Guid>
        {
            public ActionFieldKey(Guid organizationId, Guid actionId)
                : base(organizationId, actionId)
            {

            }
        }

        private class DataCache
        {
            #region Properties

            public DownloadColumnGroup[] ColumnGroups { get; }

            #endregion

            #region Fields

            private static readonly AutoUpdateCache<DownloadColumnGroup[]> _columnGroupCache = new AutoUpdateCache<DownloadColumnGroup[]>(new TimeSpan(0, 1, 0), GetColumnGroups);

            #endregion

            #region Construction

            public DataCache(OrganizationState organization, WebRoute route)
            {
                ColumnGroups = _columnGroupCache.GetData();
            }

            #endregion

            #region Methods

            private static DownloadColumnGroup[] GetColumnGroups()
            {
                var data = new List<DownloadColumnGroup>();

                AppendType(1, "Basic", CollectionName.Actions_Fields_FieldGroup_Basic);
                AppendType(2, "Advanced", CollectionName.Actions_Fields_FieldGroup_Advanced);

                return data.ToArray();

                void AppendType(int typeSequence, string typeName, string collectionName)
                {
                    var items = TCollectionItemCache
                        .Query(new TCollectionItemFilter { CollectionName = collectionName })
                        .OrderBy(x => x.ItemSequence).ThenBy(x => x.ItemName);
                    var sequence = 1;

                    foreach (var item in items)
                        data.Add(new DownloadColumnGroup(typeSequence, typeName, item.ItemNumber, sequence++, item.ItemName));
                }
            }

            #endregion
        }

        private class StateData
        {
            public Guid? ReportIdentifier { get; set; }
            public Download Default { get; set; }

            private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            public string Serialize() => JsonConvert.SerializeObject(this, _serializerSettings);

            public static StateData Deserialize(string json) =>
                json.IsEmpty() ? null : JsonConvert.DeserializeObject<StateData>(json, _serializerSettings);
        }

        #endregion

        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType type, string message) => Alert?.Invoke(this, new AlertArgs(type, message));

        public class NeedDataArgs : EventArgs
        {
            public string DataName { get; set; }
            public IList DataSource { get; set; }
            public int? MaximumRows { get; set; }
        }

        public delegate void NeedDataHandler(object sender, NeedDataArgs args);

        public event NeedDataHandler NeedData;

        protected NeedDataArgs OnNeedData()
        {
            var args = new NeedDataArgs();
            args.MaximumRows = MaxRowCount;

            NeedData?.Invoke(this, args);

            if (args.DataSource == null)
                throw ApplicationError.Create("DataSource is null.");

            return args;
        }

        public class NeedVisibleColumnsArgs : EventArgs
        {
            public IList DataSource { get; set; }
            public DownloadColumnState[] Columns { get; set; }
            public DownloadColumn[] VisibleColumns { get; set; }
        }

        public delegate void NeedVisibleColumnsHandler(object sender, NeedVisibleColumnsArgs args);

        public event NeedVisibleColumnsHandler NeedVisibleColumns;

        protected DownloadColumn[] OnNeedVisibleColumns(IList dataSource)
        {
            var args = new NeedVisibleColumnsArgs();
            args.DataSource = dataSource;
            args.Columns = CurrentReport.Columns;

            NeedVisibleColumns?.Invoke(this, args);

            return args.VisibleColumns == null
                ? GetVisibleColumns(dataSource).ToArray()
                : args.VisibleColumns;
        }

        #endregion

        #region Properties

        public IFinder Finder { get; set; }

        protected Download CurrentReport
        {
            get => (Download)ViewState[nameof(CurrentReport)];
            private set => ViewState[nameof(CurrentReport)] = value;
        }

        private Dictionary<string, int> OriginalColumnsOrder
        {
            get => (Dictionary<string, int>)ViewState[nameof(OriginalColumnsOrder)];
            set => ViewState[nameof(OriginalColumnsOrder)] = value;
        }

        public int? RowCount
        {
            get => (int?)ViewState[nameof(RowCount)];
            set => ViewState[nameof(RowCount)] = value;
        }

        public int? MaxRowCount
        {
            get => (int?)ViewState[nameof(MaxRowCount)];
            set => ViewState[nameof(MaxRowCount)] = value;
        }

        private string ReportName => ((IHasWebRoute)Page).Route.Name + ":Default";

        private bool IsColumnLoaded
        {
            get => (bool)(ViewState[nameof(IsColumnLoaded)] ?? false);
            set => ViewState[nameof(IsColumnLoaded)] = value;
        }

        private bool IsReportLoaded
        {
            get => (bool)(ViewState[nameof(IsReportLoaded)] ?? false);
            set => ViewState[nameof(IsReportLoaded)] = value;
        }

        #endregion

        #region Fields

        private static readonly Type _baseReportType = typeof(Download);
        private Type _reportType = _baseReportType;

        private DataCache _dataCache;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MaxRowCount = ServiceLocator.AppSettings.Platform.Search.Download.MaximumRows;

            _dataCache = new DataCache(Organization, Route);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack && IsColumnLoaded && IsReportLoaded)
                GetInputValues();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsColumnLoaded && !IsReportLoaded)
                LoadState(false);
        }

        #endregion

        #region Loading

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            var report = CurrentReport;
            if (report == null || report.Columns.Length == 0)
                return;

            var groups = _dataCache.ColumnGroups.ToDictionary(x => x.GroupKey);

            foreach (var c in report.Columns)
            {
                c.Group = groups.TryGetValue(c.GroupKey, out var group)
                    ? group
                    : DownloadColumnGroup.Empty;
            }
        }

        #endregion

        #region Event handlers

        protected void OnDownload()
        {
            var report = CurrentReport;

            if (!ValidateMaxRowCount(report.Format))
                return;

            if (report.Columns.Length == 0)
            {
                OnAlert(AlertType.Error, "ColumnState is empty.");
                return;
            }

            var dataArgs = OnNeedData();
            if (dataArgs.DataSource.Count == 0)
            {
                OnAlert(AlertType.Error, "Data list is empty.");
                return;
            }

            var visibleColumns = OnNeedVisibleColumns(dataArgs.DataSource);
            if (visibleColumns.Length == 0)
            {
                OnAlert(AlertType.Error, "At least one column must be included to output file.");
                return;
            }

            byte[] bytes = null;

            if (report.Format == "xlsx")
            {
                var helper = new XlsxExportHelper();

                foreach (var column in visibleColumns)
                {
                    var title = Translate(column.Title);

                    if (report.RemoveSpaces && title.IsNotEmpty())
                        title = title.Replace(" ", string.Empty);

                    helper.Map(column.Name, title, column.Format, column.Width, column.Align);
                }

                bytes = helper.GetXlsxBytes(dataArgs.DataSource, Translate("Sheet 1"));
            }
            else if (report.Format == "csv")
            {
                var helper = new CsvExportHelper(dataArgs.DataSource);

                foreach (var column in visibleColumns)
                {
                    var format = column.Format;
                    if (format.IsNotEmpty())
                        format = $"{{0:{format}}}";

                    var title = Translate(column.Title);

                    if (report.RemoveSpaces && title.IsNotEmpty())
                        title = title.Replace(" ", string.Empty);

                    helper.AddMapping(column.Name, title, format);
                }

                bytes = helper.GetBytes(Encoding.UTF8);
            }
            else
            {
                throw ApplicationError.Create("Unexpected file format: " + report.Format);
            }

            SaveState();

            var filename = "result";
            if (dataArgs.DataName.IsNotEmpty())
                filename = dataArgs.DataName;

            filename = StringHelper.Sanitize(filename.Pluralize(), '-') + "-" + DateTime.UtcNow.ToString("yyyyMMdd'-'HHmmss");

            Page.Response.SendFile(filename, report.Format, bytes);
        }

        protected virtual List<DownloadColumn> GetVisibleColumns(IList dataSource)
        {
            var dataProperties = new Dictionary<string, PropertyDescriptor>();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(dataSource[0]))
                dataProperties.Add(prop.Name, prop);

            return CurrentReport.Columns
                .Where(x => x.Info.Visible && !x.Hidden && dataProperties.ContainsKey(x.Info.Name))
                .Select(x => x.Info)
                .ToList();
        }

        protected bool ValidateMaxRowCount(string format)
        {
            if (format == "csv")
                return true;

            if (MaxRowCount == null || RowCount == null || MaxRowCount.Value >= RowCount.Value)
                return true;

            OnAlert(
                AlertType.Error,
                $"The maximum number of rows permitted for a {format} report download on this page is <b>{MaxRowCount:n0}</b>." +
                $"<br>Use the Search filter to decrease the number of rows in your search results from {RowCount:n0}, or select the Data Export (.csv) option instead.");

            return false;
        }

        protected abstract void OnColumnsChanged();

        #endregion

        #region Methods (columns)

        public void LoadColumns(object dataItem)
        {
            LoadColumns(GetColumns(dataItem).OrderBy(x => x.Name));
        }

        public void LoadColumns(IEnumerable<DownloadColumn> columns)
        {
            if (columns == null)
                throw new ArgumentNullException(nameof(columns));

            var report = CurrentReport;

            if (report == null)
            {
                CurrentReport = report = CreateReport();

                GetInputValues();
            }

            var states = new List<DownloadColumnState>();

            var number = 1;

            foreach (var column in columns)
            {
                states.Add(new DownloadColumnState(column)
                {
                    Sequence = number++
                });
            }

            if (states.Count == 0)
                return;

            var order = states.ToDictionary(x => x.Info.Name, x => x.Sequence);

            if (IsColumnLoaded && report.Columns.Length == states.Count && report.Columns.All(x => order.ContainsKey(x.Info.Name)))
                return;

            if (report.Columns.IsNotEmpty())
                MergeColumns(states, report.Columns);

            OriginalColumnsOrder = order;
            report.Columns = states.ToArray();

            IsColumnLoaded = true;
        }

        private void ResetColumns()
        {
            var report = CurrentReport;

            foreach (var column in report.Columns)
            {
                column.Hidden = false;
                column.Sequence = OriginalColumnsOrder.ContainsKey(column.Info.Name)
                    ? OriginalColumnsOrder[column.Info.Name]
                    : -1;
            }
        }

        private static void MergeColumns(IEnumerable<DownloadColumnState> source, IEnumerable<DownloadColumnState> update)
        {
            var loadedColumns = update.ToDictionary(x => x.Info.Name);

            if (source.Any(x => loadedColumns.ContainsKey(x.Info.Name)))
            {
                foreach (var column in source)
                {
                    if (loadedColumns.TryGetValue(column.Info.Name, out var loadedColumn))
                    {
                        column.Sequence = loadedColumn.Sequence;
                        column.Hidden = loadedColumn.Hidden;
                    }
                    else
                    {
                        column.Sequence = -1;

                        if (column.Info.Visible)
                            column.Hidden = true;
                    }
                }
            }
            else
            {
                var number = 1;

                foreach (var column in source)
                {
                    column.Sequence = number++;
                    column.Hidden = !column.Info.Visible;
                }
            }
        }

        #endregion

        #region Methods (settings)

        protected abstract void SetInputValues(Download report);

        protected abstract void GetInputValues();

        protected void LoadState(bool forceDefault)
        {
            var stateJson = TReportSearch.BindFirst(
                x => x.ReportData,
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Download
                  && x.ReportDescription == ReportName);
            var state = StateData.Deserialize(stateJson);

            Download report = null;

            if (state != null)
            {
                if (!forceDefault && state.ReportIdentifier.HasValue)
                {
                    var page = (IHasWebRoute)Page;
                    var reportJson = TReportSearch.BindFirst(
                        x => x.ReportData,
                        x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                          && x.UserIdentifier == User.UserIdentifier
                          && x.ReportType == ReportType.Download
                          && x.ReportDescription == page.Route.Name
                          && x.ReportIdentifier == state.ReportIdentifier.Value);

                    report = DeserializeReport(reportJson);
                }

                if (report == null)
                    report = state.Default;
            }

            if (report == null)
                report = CreateReport();

            LoadReport(report);
        }

        protected void SaveState()
        {
            if (!IsColumnLoaded || !IsReportLoaded)
                return;

            var isNew = false;
            var page = (IHasWebRoute)Page;
            var entity = TReportSearch.SelectFirst(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.UserIdentifier == User.UserIdentifier
                  && x.ReportType == ReportType.Download
                  && x.ReportDescription == ReportName);

            if (entity == null)
            {
                isNew = true;
                entity = new TReport
                {
                    ReportIdentifier = UniqueIdentifier.Create(),
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    UserIdentifier = User.UserIdentifier,
                    ReportType = ReportType.Download,
                    ReportTitle = page.Route.Title.IfNullOrEmpty(page.Route.Name),
                    ReportDescription = ReportName,
                    Created = DateTimeOffset.Now,
                    CreatedBy = User.UserIdentifier,
                };
            }

            var state = StateData.Deserialize(entity.ReportData) ?? new StateData();
            state.ReportIdentifier = CurrentReport.Identifier;

            if (!CurrentReport.Identifier.HasValue || CurrentReport.Identifier.Value == Guid.Empty)
                state.Default = CurrentReport;

            entity.ReportData = state.Serialize();
            entity.Modified = DateTimeOffset.Now;
            entity.ModifiedBy = User.UserIdentifier;

            if (isNew)
                TReportStore.Insert(entity);
            else
                TReportStore.Update(entity);
        }

        protected void LoadReport(Download inputReport)
        {
            var curReport = CurrentReport;

            curReport.Identifier = inputReport.Identifier;

            SetInputValues(inputReport);
            GetInputValues();

            if (inputReport.Columns.IsNotEmpty())
                MergeColumns(curReport.Columns, inputReport.Columns);
            else
                ResetColumns();

            curReport.SortColumns();

            OnColumnsChanged();

            IsReportLoaded = true;
        }

        #endregion

        #region Methods (state)

        protected void SetState(JsonColumnState[] state)
        {
            var report = CurrentReport;
            if (report == null || report.Columns.Length == 0 || state.IsEmpty())
                return;

            if (state.Length != report.Columns.Length)
            {
                OnColumnsChanged();
                return;
            }

            var isSequenceChanged = false;

            for (var i = 0; i < report.Columns.Length; i++)
            {
                var clientItem = state[i];
                var serverItem = report.Columns[i];

                if (clientItem.Sequence != serverItem.Sequence)
                {
                    isSequenceChanged = true;
                    serverItem.Sequence = clientItem.Sequence;
                }

                serverItem.Hidden = !clientItem.Visible;
            }

            if (isSequenceChanged)
            {
                report.SortColumns();

                OnColumnsChanged();
            }
        }

        protected JsonColumnState[] GetState() =>
            CurrentReport.Columns.Select(x => new JsonColumnState(x)).ToArray();

        #endregion

        #region Methods (helpers)

        protected void SetReportType(Type type)
        {
            if (!type.IsSubclassOf(_baseReportType))
                throw ApplicationError.Create("The report type must be derived from " + _baseReportType.Name);

            _reportType = type;
        }

        protected virtual Download CreateReport() =>
            (Download)Activator.CreateInstance(_reportType);

        protected virtual Download DeserializeReport(string json) =>
            json.IsEmpty() ? null : (Download)JsonConvert.DeserializeObject(json, _reportType);

        internal static IEnumerable<DownloadColumn> GetColumns(object obj) => GetColumns(TypeDescriptor.GetProperties(obj));

        internal static IEnumerable<DownloadColumn> GetColumns(ITypedList list) => GetColumns(list.GetItemProperties(null));

        internal static IEnumerable<DownloadColumn> GetColumns(Type type) => GetColumns(TypeDescriptor.GetProperties(type));

        private static IEnumerable<DownloadColumn> GetColumns(PropertyDescriptorCollection props)
        {
            foreach (PropertyDescriptor prop in props)
            {
                var type = prop.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);

                if (type.IsPrimitive || type.IsValueType || type == typeof(string))
                    yield return new DownloadColumn(prop.Name);
            }
        }

        #endregion
    }
}