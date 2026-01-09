using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Custom.CMDS.Reports.Controls
{
    public partial class DepartmentUserStatusCriteria : SearchCriteriaController<TUserStatusFilter>
    {
        #region Classes

        [Serializable]
        private class InnerFilter : TUserStatusFilter
        {
            public string SnapshotCriteriaType { get; set; }
        }

        #endregion

        #region Properties

        private FindEntity.DataItem[] UserItems
        {
            get => (FindEntity.DataItem[])ViewState[nameof(UserItems)];
            set => ViewState[nameof(UserItems)] = value;
        }

        public override TUserStatusFilter Filter
        {
            get
            {
                var filter = new InnerFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    SnapshotCriteriaType = SnapshotCriteriaType.Value,
                    Departments = GetDepartmentIdentifiers(),
                    DepartmentRole = DepartmentRole.Value.IsEmpty()
                        ? null
                        : DepartmentRole.Value == "None"
                            ? string.Empty
                            : DepartmentRole.Value,
                    DepartmentLabel = DepartmentLabel.Value,
                    UserIdentifier = !UserIdentifier.Enabled
                        ? new[] { Guid.Empty }
                        : !UserIdentifier.HasValue
                            ? UserItems.Select(x => x.Value).ToArray()
                            : UserIdentifier.Values,
                    ExcludeAchievementTypes = AchievementType.FlattenOptions().Where(x => !x.Selected).Select(x => int.Parse(x.Value)).ToArray(),
                    ExcludeStandardTypes = StandardType.FlattenOptions().Where(x => !x.Selected).Select(x => int.Parse(x.Value)).ToArray(),
                    ScoreFrom = ScoreFrom.ValueAsDecimal,
                    ScoreThru = ScoreThru.ValueAsDecimal,
                };

                if (filter.SnapshotCriteriaType == "Range")
                {
                    filter.AsAtRange.Since = SnapshotDateSince.Value;
                    filter.AsAtRange.Before = SnapshotDateBefore.Value;
                }
                else if (!string.IsNullOrEmpty(SnapshotDate.Value))
                {
                    var date = DateTimeOffsetFromString(SnapshotDate.Value);
                    filter.AsAt = date;
                }

                // The user must filter by a valid date range or by a selected snapshot.
                filter.Enabled = (filter.AsAtRange.Since.HasValue && filter.AsAtRange.Before.HasValue)
                    || filter.AsAt.HasValue;

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

                OnSnapshotCriteriaTypeChanged();

                if (SnapshotCriteriaType.Value == "Range")
                {
                    SnapshotDateSince.Value = filter.AsAtRange.Since;
                    SnapshotDateBefore.Value = filter.AsAtRange.Before;
                }
                else
                {
                    SnapshotDate.ClearSelection();

                    if (filter.AsAt != null)
                    {
                        var dtoStr = DateTimeOffsetToString(filter.AsAt.Value);
                        var option = SnapshotDate.FindOptionByValue(dtoStr);
                        if (option != null)
                            option.Selected = true;
                    }
                }

                if (filter.DepartmentRole == null)
                    DepartmentRole.Value = string.Empty;
                else if (filter.DepartmentRole.Length == 0)
                    DepartmentRole.Value = "None";
                else
                    DepartmentRole.Value = filter.DepartmentRole;

                DepartmentLabel.Value = filter.DepartmentLabel;

                LoadDepartments();

                if (DepartmentIdentifier.Enabled)
                {
                    if (filter.Departments.IsEmpty())
                        DepartmentIdentifier.Values = null;
                    else if (filter.Departments.Length == 1 && filter.Departments[0] == Guid.Empty)
                        DepartmentIdentifier.Values = null;
                    else if (filter.Departments.Length == DepartmentIdentifier.GetCount() && DepartmentIdentifier.GetDataItems().All(x => filter.Departments.Contains(x.Value)))
                        DepartmentIdentifier.Values = null;
                    else
                        DepartmentIdentifier.Values = filter.Departments;
                }

                LoadUsers();

                if (UserIdentifier.Enabled)
                {
                    if (filter.UserIdentifier.IsEmpty())
                        UserIdentifier.Values = null;
                    else if (filter.UserIdentifier.Length == 1 && filter.UserIdentifier[0] == Guid.Empty)
                        UserIdentifier.Values = null;
                    else if (filter.UserIdentifier.Length == UserItems.Length && UserItems.All(x => filter.UserIdentifier.Contains(x.Value)))
                        UserIdentifier.Values = null;
                    else
                        UserIdentifier.Values = filter.UserIdentifier;
                }

                var excludedAchievements = new HashSet<int>(filter.ExcludeAchievementTypes ?? new int[0]);
                foreach (var option in AchievementType.FlattenOptions())
                    option.Selected = !excludedAchievements.Contains(int.Parse(option.Value));

                var excludedStandards = new HashSet<int>(filter.ExcludeStandardTypes ?? new int[0]);
                foreach (var option in StandardType.FlattenOptions())
                    option.Selected = !excludedStandards.Contains(int.Parse(option.Value));

                ScoreFrom.ValueAsDecimal = filter.ScoreFrom;
                ScoreThru.ValueAsDecimal = filter.ScoreThru;
            }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SnapshotCriteriaType.AutoPostBack = true;
            SnapshotCriteriaType.ValueChanged += (s, a) => OnSnapshotCriteriaTypeChanged();

            DepartmentLabel.AutoPostBack = true;
            DepartmentLabel.ValueChanged += (s, a) => LoadDepartments();

            DepartmentIdentifier.AutoPostBack = true;
            DepartmentIdentifier.ValueChanged += (s, a) => LoadUsers();

            UserIdentifier.NeedDataCount += UserIdentifier_NeedDataCount;
            UserIdentifier.NeedDataSource += UserIdentifier_NeedDataSource;
            UserIdentifier.NeedSelectedItems += UserIdentifier_NeedSelectedItems;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var isScoreFieldVisible = Identity.IsInRole(CmdsRole.Programmers)
                || Identity.IsInRole(CmdsRole.SystemAdministrators);

            ScoreFromField.Visible = isScoreFieldVisible;
            ScoreThruField.Visible = isScoreFieldVisible;

            LoadSnapshotSelector();
            OnSnapshotCriteriaTypeChanged();
            LoadDepartments();
            LoadUsers();
            LoadAchievementTypes();
            LoadStandardTypes();
        }

        #endregion

        #region Event handlers

        private void OnSnapshotCriteriaTypeChanged()
        {
            var byRange = SnapshotCriteriaType.Value == "Range";

            SnapshotDateField.Visible = !byRange;
            SnapshotDateSinceField.Visible = byRange;
            SnapshotDateBeforeField.Visible = byRange;

            if (byRange)
            {
                SnapshotDate.ClearSelection();
            }
            else
            {
                SnapshotDateSince.Value = null;
                SnapshotDateBefore.Value = null;
            }
        }

        private void UserIdentifier_NeedDataCount(object sender, FindEntity.CountArgs args)
        {
            args.Count = UserItems.Length;
        }

        private void UserIdentifier_NeedDataSource(object sender, FindEntity.DataArgs args)
        {
            var items = UserItems.AsQueryable().ApplyPaging(args.Paging);

            if (args.Keyword.IsNotEmpty())
                items = items.Where(x => x.Text.IndexOf(args.Keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            args.Items = items.ToArray();
        }

        private void UserIdentifier_NeedSelectedItems(object sender, FindEntity.ItemsArgs args)
        {
            args.Items = UserItems.Where(x => args.Identifiers.Contains(x.Value)).ToArray();
        }

        #endregion

        #region Methods (criteria)

        public override void Clear()
        {
            SnapshotCriteriaType.ClearSelection();
            SnapshotDate.ClearSelection();
            SnapshotDateSince.Value = null;
            SnapshotDateBefore.Value = null;
            LoadDepartments();
            LoadUsers();
            LoadAchievementTypes();
            LoadStandardTypes();
            DepartmentRole.Value = "Department";
            DepartmentLabel.ClearSelection();
            ScoreFrom.ValueAsDecimal = null;
            ScoreThru.ValueAsDecimal = null;
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

        private void LoadDepartments()
        {
            DepartmentIdentifier.Value = null;
            DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Key;
            DepartmentIdentifier.Filter.DepartmentLabel = DepartmentLabel.Value;

            if (!Identity.HasAccessToAllCompanies)
                DepartmentIdentifier.Filter.UserIdentifier = User.UserIdentifier;

            var hasDepartments = DepartmentIdentifier.GetCount() > 0;

            DepartmentIdentifier.Enabled = hasDepartments;
            DepartmentIdentifier.EmptyMessage = hasDepartments ? "All Departments" : "None";
        }

        private void LoadUsers()
        {
            UserItems = new TUserStatusSearch()
                .SelectUsers(Organization.OrganizationIdentifier, GetDepartmentIdentifiers())
                .Select(x => new FindEntity.DataItem
                {
                    Value = x.UserIdentifier,
                    Text = x.UserName,
                })
                .ToArray();

            var hasData = UserItems.Length > 0;

            UserIdentifier.Value = null;
            UserIdentifier.Enabled = hasData;
            UserIdentifier.EmptyMessage = hasData ? "All Users" : "None";
        }

        private void LoadAchievementTypes()
        {
            var achievementTypeMapping = Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code);
            var items = new TUserStatusSearch()
                .SelectStatisticInfo(Organization.OrganizationIdentifier, "Resource")
                .Select(x => new ListItem
                {
                    Value = x.Number.ToString(),
                    Text = achievementTypeMapping.GetOrDefault(x.Name, x.Name)
                })
                .OrderBy(x => x.Text)
                .ToArray();

            AchievementType.LoadItems(items);
            AchievementType.SelectAll();
        }

        private void LoadStandardTypes()
        {
            var items = new TUserStatusSearch()
                .SelectStatisticInfo(Organization.OrganizationIdentifier, "Standard")
                .Select(x => new ListItem
                {
                    Value = x.Number.ToString(),
                    Text = x.Name
                })
                .OrderBy(x => x.Text)
                .ToArray();

            StandardType.LoadItems(items);
            StandardType.SelectAll();
        }

        private Guid[] GetDepartmentIdentifiers()
        {
            return !DepartmentIdentifier.Enabled
                ? new[] { Guid.Empty }
                : !DepartmentIdentifier.HasValue || DepartmentIdentifier.Values.Length == DepartmentIdentifier.GetCount()
                    ? null
                    : DepartmentIdentifier.Values;
        }

        #endregion

        #region Methods (helpers)

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

        #endregion
    }
}