using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class BuildConditions : BaseUserControl
    {
        #region Events

        public event BuildViewChangedHandler ViewChanged;
        protected void OnViewChanged(BuildViewType view) =>
            ViewChanged?.Invoke(this, new BuildViewChangedArgs(view));

        #endregion

        #region Properties

        public Build.IData BuildData { get; set; }

        private int? EditIndex
        {
            get => (int?)ViewState[nameof(EditIndex)];
            set => ViewState[nameof(EditIndex)] = value;
        }

        private List<ReportCondition> Conditions
        {
            get => (List<ReportCondition>)ViewState[nameof(Conditions)];
            set => ViewState[nameof(Conditions)] = value;
        }

        public bool HasItems => Conditions.Count > 0;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ConditionNameUniqueValidator.ServerValidate += ConditionNameUniqueValidator_ServerValidate;

            ConditionRepeater.DataBinding += ConditionRepeater_DataBinding;
            ConditionRepeater.ItemCommand += ConditionRepeater_ItemCommand;
        }

        #endregion

        #region Event handlers

        private void ConditionRepeater_DataBinding(object sender, EventArgs e)
        {
            ConditionRepeater.DataSource = Conditions.Select(x => new
            {
                Name = x.Name,
                Sql = x.GetSql()
            });
        }

        private void ConditionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCondition")
            {
                Conditions.RemoveAt(e.Item.ItemIndex);

                ConditionRepeater.DataBind();
            }
            else if (e.CommandName == "EditCondition")
            {
                Edit(e.Item.ItemIndex);
            }
        }

        private void ConditionNameUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var name = ConditionName.Text;
            if (name.IsEmpty())
                return;

            var skipIndex = EditIndex ?? -1;

            for (var i = 0; i < Conditions.Count; i++)
            {
                if (i != skipIndex && string.Equals(Conditions[i].Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    args.IsValid = false;
                    break;
                }
            }
        }

        #endregion

        #region Methods

        public void LoadData()
        {
            Conditions = new List<ReportCondition>();

            if (BuildData.Conditions.IsNotEmpty())
                Conditions.AddRange(BuildData.Conditions);

            View();
        }

        public void View()
        {
            EditIndex = null;
            MultiView.SetActiveView(TableView);

            var hasData = Conditions.Count > 0;

            ConditionRepeater.Visible = hasData;
            ConditionRepeater.DataBind();

            if (!hasData)
                TableStatus.AddMessage(AlertType.Information, "Conditions are optional. You can add as many as you need.");

            OnViewChanged(BuildViewType.View);
        }

        public void Edit(int index)
        {
            if (index >= 0 && index < Conditions.Count)
            {
                EditIndex = index;

                var condition = Conditions[index];

                ConditionName.Text = condition.Name;

                ConditionWhere.LoadData(BuildData.Columns, condition.Where);
                ConditionAnd.LoadData(BuildData.Columns, condition.And);
                ConditionOr.LoadData(BuildData.Columns, condition.Or);
            }
            else
            {
                ConditionName.Text = Conditions.Count == 0 ? "Default" : null;

                ConditionWhere.LoadData(BuildData.Columns, null);
                ConditionAnd.LoadData(BuildData.Columns, null);
                ConditionOr.LoadData(BuildData.Columns, null);
            }

            MultiView.SetActiveView(EditorView);

            OnViewChanged(BuildViewType.Edit);
        }

        public void Save()
        {
            var isNew = !EditIndex.HasValue;
            var condition = isNew ? new ReportCondition() : Conditions[EditIndex.Value];

            condition.Name = ConditionName.Text;

            ConditionWhere.GetCondition(BuildData.Columns, condition.Where);
            ConditionAnd.GetCondition(BuildData.Columns, condition.And);
            ConditionOr.GetCondition(BuildData.Columns, condition.Or);

            if (isNew)
                Conditions.Add(condition);
            else
                Conditions[EditIndex.Value] = condition;

            View();
        }

        public ReportCondition[] GetConditions() => Conditions.ToArray();

        protected override void SetupValidationGroup(string groupName)
        {
            ConditionNameRequiredValidator.ValidationGroup = groupName;
            ConditionNameUniqueValidator.ValidationGroup = groupName;
        }

        #endregion
    }
}