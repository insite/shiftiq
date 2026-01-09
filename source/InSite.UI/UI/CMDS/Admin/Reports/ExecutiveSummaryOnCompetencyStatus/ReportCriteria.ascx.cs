using System;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Admin.Reports.Controls
{
    public partial class ExecutiveSummaryOnCompetencyStatusCriteria : SearchCriteriaController<ExecutiveSummaryOnCompetencyStatusFilter>
    {
        [Serializable]
        private class InnerFilter : ExecutiveSummaryOnCompetencyStatusFilter
        {
            public string SnapshotCriteriaType { get; set; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            DepartmentIdentifier.OrganizationIdentifier = Organization.Key;
            DivisionIdentifier.OrganizationIdentifier = Organization.Key;

            LoadSnapshotSelector();
        }

        public void LoadSnapshotSelector()
        {
            SnapshotDate.Items.Clear();
            SnapshotDate.Items.Add(new ComboBoxOption());

            var reader = new TUserStatusSearch();
            var data = reader.GetOrganizationSnapshots(Organization.OrganizationIdentifier);

            foreach (var item in data)
            {
                SnapshotDate.Items.Add(new ComboBoxOption
                {
                    Text = item.AsAt.Format(User.TimeZone),
                    Value = DateTimeOffsetToString(item.AsAt)
                });
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (SnapshotCriteriaType.Value == "Range")
            {
                SnapshotDateField.Style[HtmlTextWriterStyle.Display] = "none";
                SnapshotDateSinceField.Style[HtmlTextWriterStyle.Display] = string.Empty;
                SnapshotDateBeforeField.Style[HtmlTextWriterStyle.Display] = string.Empty;
            }
            else
            {
                SnapshotDateField.Style[HtmlTextWriterStyle.Display] = string.Empty;
                SnapshotDateSinceField.Style[HtmlTextWriterStyle.Display] = "none";
                SnapshotDateBeforeField.Style[HtmlTextWriterStyle.Display] = "none";
            }

            base.OnPreRender(e);
        }

        public override ExecutiveSummaryOnCompetencyStatusFilter Filter
        {
            get
            {
                var filter = new InnerFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    SnapshotCriteriaType = SnapshotCriteriaType.Value,
                    DivisionIdentifier = DivisionIdentifier.ValueAsGuid,
                    DepartmentIdentifier = DepartmentIdentifier.Value,
                    Criticality = Criticality.Value
                };

                if (filter.SnapshotCriteriaType == "Range")
                {
                    filter.AsAt.Since = SnapshotDateSince.Value;
                    filter.AsAt.Before = SnapshotDateBefore.Value;
                }
                else if (!string.IsNullOrEmpty(SnapshotDate.Value))
                {
                    var date = DateTimeOffsetFromString(SnapshotDate.Value);
                    filter.AsAt.Since = date;
                    filter.AsAt.Before = date;
                }

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                var filter = value;

                if (filter is InnerFilter innerFilter)
                    SnapshotCriteriaType.Value = innerFilter.SnapshotCriteriaType;
                else
                    SnapshotCriteriaType.Value = "Select";

                if (SnapshotCriteriaType.Value == "Range")
                {
                    SnapshotDateSince.Value = filter.AsAt.Since;
                    SnapshotDateBefore.Value = filter.AsAt.Before;
                }
                else
                {
                    SnapshotDate.ClearSelection();

                    if (filter.AsAt?.Since != null)
                    {
                        var dtoStr = DateTimeOffsetToString(filter.AsAt.Since.Value);
                        var option = SnapshotDate.FindOptionByValue(dtoStr);
                        if (option != null)
                            option.Selected = true;
                    }
                }

                DivisionIdentifier.ValueAsGuid = filter.DivisionIdentifier;
                DepartmentIdentifier.Value = filter.DepartmentIdentifier;
                Criticality.Value = filter.Criticality;
            }
        }

        public override void Clear()
        {
            SnapshotCriteriaType.ClearSelection();
            SnapshotDate.ClearSelection();
            SnapshotDateSince.Value = null;
            SnapshotDateBefore.Value = null;

            DivisionIdentifier.ValueAsGuid = null;
            DepartmentIdentifier.Value = null;
            Criticality.ClearSelection();
        }

        private static string DateTimeOffsetToString(DateTimeOffset value)
        {
            return $"{value.DateTime.Ticks}:{(int)value.Offset.TotalMinutes}";
        }

        private static DateTimeOffset DateTimeOffsetFromString(string value)
        {
            var parts = value.Split(':');
            var ticks = long.Parse(parts[0]);
            var offset = int.Parse(parts[1]);

            return new DateTimeOffset(ticks, TimeSpan.FromMinutes(offset));
        }
    }
}