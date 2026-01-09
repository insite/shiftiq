using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public abstract partial class BaseReportManager : BaseUserControl
    {
        #region Events

        public class ReportArgs : EventArgs
        {
            public ISearchReport Report { get; set; }
        }

        public class NeedReportArgs : ReportArgs
        {
            public bool Cancelled { get; set; }
        }

        public delegate void ReportHandler(object sender, ReportArgs args);

        public delegate void NeedReportHandler(object sender, NeedReportArgs args);

        public event NeedReportHandler NeedReport;

        public event ReportHandler ReportSelected;

        protected ISearchReport OnNeedReport()
        {
            var args = new NeedReportArgs();

            NeedReport?.Invoke(this, args);

            return args.Cancelled
                ? null
                : args.Report ?? throw ApplicationError.Create("Report is null");
        }

        private void OnReportSelected(ISearchReport report)
        {
            ReportSelected?.Invoke(this, new ReportArgs
            {
                Report = report
            });
        }

        #endregion

        #region Classes

        protected sealed class ControlsInfo
        {
            public ITextControl ReportName { get; set; }
            public ComboBox ReportSelector { get; set; }
            public IButtonControl SaveButton { get; set; }
            public IButtonControl RemoveButton { get; set; }
            public IButtonControl CreateButton { get; set; }
        }

        #endregion

        #region Properties

        private bool IsInited
        {
            get => (bool)(ViewState[nameof(IsInited)] ?? false);
            set => ViewState[nameof(IsInited)] = value;
        }

        private Dictionary<Guid, ISearchReport> SavedReports
        {
            get => (Dictionary<Guid, ISearchReport>)ViewState[nameof(SavedReports)];
            set => ViewState[nameof(SavedReports)] = value;
        }

        #endregion

        #region Fields

        private ControlsInfo _controls;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _controls = GetControls();

            _controls.ReportSelector.AutoPostBack = true;
            _controls.ReportSelector.ValueChanged += ReportSelector_ValueChanged;

            _controls.CreateButton.Click += CreateButton_Click;
            _controls.SaveButton.Click += SaveButton_Click;
            _controls.RemoveButton.Click += RemoveButton_Click;
        }

        protected abstract ControlsInfo GetControls();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack || IsInited)
                return;

            BindSavedReports();

            IsInited = true;
        }

        #endregion

        #region Event handlers

        private void CreateButton_Click(object sender, EventArgs e)
        {
            var name = _controls.ReportName.Text.Trim();

            if (name.IsEmpty())
            {
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(BaseReportManager),
                    "invalid_name",
                    "$(document).ready(function () { alert('Enter a name of saved report.'); });",
                    true);

                return;
            }

            var report = OnNeedReport();
            if (report == null)
                return;

            report.Identifier = UniqueIdentifier.Create();
            report.Name = name;

            var items = LoadReportsInternal();
            items.RemoveAll(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            items.Add(report);
            SaveReports(items);

            _controls.ReportName.Text = null;

            BindSavedReports();

            _controls.ReportSelector.ValueAsGuid = report.Identifier;

            OnReportSelected(report);
        }

        private void ReportSelector_ValueChanged(object sender, EventArgs e)
        {
            var id = _controls.ReportSelector.ValueAsGuid;

            var report = GetSelectedReport(id);

            OnReportSelected(report);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var report = OnNeedReport();
            if (report == null)
                return;

            var selected = GetSelectedReport();
            if (selected == null)
                return;

            var items = LoadReportsInternal();
            var index = items.FindIndex(x => x.Identifier == selected.Identifier);
            var isNew = index == -1;
            var isChanged = isNew || report.Identifier != selected.Identifier;

            report.Name = selected.Name;

            if (isNew)
            {
                report.Identifier = UniqueIdentifier.Create();
                items.Add(report);
            }
            else
            {
                report.Identifier = selected.Identifier;
                items[index] = report;
            }

            SaveReports(items);

            if (isNew)
            {
                BindSavedReports();

                _controls.ReportSelector.ValueAsGuid = report.Identifier;
            }
            else
            {
                SavedReports[report.Identifier.Value] = report;
            }

            if (isChanged)
                OnReportSelected(report);
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            var report = GetSelectedReport();
            if (report == null)
                return;

            var items = LoadReportsInternal();

            var itemsRemoved = items.RemoveAll(x => x.Identifier == report.Identifier.Value);
            if (itemsRemoved > 0)
                SaveReports(items);

            BindSavedReports();

            if (itemsRemoved > 0)
                OnReportSelected(null);
        }

        #endregion

        #region Methods (public)

        public ISearchReport GetSelectedReport() =>
            GetSelectedReport(_controls.ReportSelector.ValueAsGuid);

        public void SetSelectedReport(Guid? reportId)
        {
            _controls.ReportSelector.ValueAsGuid = reportId;
        }

        #endregion

        #region Methods (other)

        private ISearchReport GetSelectedReport(Guid? id) =>
            id.HasValue && SavedReports.TryGetValue(id.Value, out var report) ? report : null;

        #endregion

        #region Methods (personalization)

        protected virtual bool BindSavedReports()
        {
            _controls.ReportSelector.Items.Clear();
            SavedReports = null;

            var reports = LoadReportsInternal()
                .OrderBy(x => x.Name).ThenBy(x => x.Identifier.Value)
                .ToArray();

            var hasReports = reports.Length > 0;

            if (hasReports)
            {
                SavedReports = reports.ToDictionary(x => x.Identifier.Value);
                _controls.ReportSelector.Items.Add(new ComboBoxOption());

                foreach (var report in reports)
                    _controls.ReportSelector.Items.Add(new ComboBoxOption(
                        report.Name,
                        report.Identifier.Value.ToString()));
            }

            return hasReports;
        }

        protected abstract List<ISearchReport> LoadReports();
        protected abstract void SaveReports(List<ISearchReport> value);

        private List<ISearchReport> LoadReportsInternal()
        {
            var reports = LoadReports();
            if (reports == null)
                return new List<ISearchReport>();

            return reports
                .Where(x => x.Identifier.HasValue && !string.IsNullOrEmpty(x.Name))
                .GroupBy(x => x.Identifier.Value)
                .Select(x => x.OrderBy(y => y.Identifier.Value).First())
                .ToList();
        }

        #endregion
    }
}