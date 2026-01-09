using System;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Shift.Common;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class BuildDataSet : BaseUserControl
    {
        public string SelectedValue
        {
            get => DataSetSelect.Value;
            set => DataSetSelect.Value = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected override void SetupValidationGroup(string groupName)
        {
            RequiredValidator.ValidationGroup = groupName;
        }

        private void LoadData()
        {
            var data = CreateDataSource();

            DataSetSelect.LoadItems(data);

            var defaultItem = DataSetSelect.FindOptionByValue("People", true);
            if (defaultItem != null)
                defaultItem.Selected = true;
        }

        private ListItemArray CreateDataSource()
        {
            var dataSourceNames = ReportDataSourceReader.GetDataSourceNames();
            var list = new ListItemArray();

            foreach (var dataSourceName in dataSourceNames)
                list.Add(new ListItem
                {
                    Text = dataSourceName,
                    Value = dataSourceName
                });

            return list;
        }

        public ReportDataSource GetSelectedDataSet()
        {
            return DataSetSelect.Value.IsNotEmpty()
                ? ReportDataSourceReader.ReadDataSource(DataSetSelect.Value)
                : null;
        }
    }
}