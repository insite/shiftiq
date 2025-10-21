using System;
using System.Linq;
using System.Web.UI;

using Shift.Common;

namespace InSite.Cmds.Controls.Reporting.Report
{
    public partial class DepartmentCriteriaSelector : UserControl
    {
        #region Events

        public event EventHandler Refreshed;

        #endregion

        #region Properties

        public bool AutoPostBack
        {
            get => (bool)(ViewState[nameof(AutoPostBack)] ?? true);
            set => ViewState[nameof(AutoPostBack)] = value;
        }

        public int SelectionLimit
        {
            get => (int)(ViewState[nameof(SelectionLimit)] ?? 0);
            set => ViewState[nameof(SelectionLimit)] = Number.CheckRange(value, 0);
        }

        public bool IsAllSelected
        {
            get
            {
                var selectedCount = Departments.GetSelectedDepartments()?.Length ?? 0;

                return (SelectionLimit == 0 && selectedCount == 0) || selectedCount == Departments.GetAllDepartments().Length;
            }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            Departments.OnClientItemsChanged = AutoPostBack
                ? $"__doPostBack('{RefreshButton.UniqueID}', '');"
                : string.Empty;

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void RefreshButton_Click(object sender, EventArgs e) =>
            Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Methods

        public void LoadData(Guid organizationId, string departmentLabel) =>
            Departments.LoadData(organizationId, departmentLabel);

        public Guid[] GetSelectedDepartments()
        {
            var result = Departments.GetSelectedDepartments();

            if (result == null)
            {
                if (SelectionLimit == 0)
                    result = Departments.GetAllDepartments();
            }
            else if (SelectionLimit > 0 && result.Length > SelectionLimit)
            {
                result = result.Take(SelectionLimit).ToArray();
            }

            return result;
        }


        #endregion
    }
}