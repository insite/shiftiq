using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.CourseObjects;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Learning.Controls
{
    public partial class UnitRepeater : BaseUserControl
    {
        private ProgressModel _progress { get; set; }

        public bool AreActivitiesUnlocked
        {
            get => ViewState[nameof(AreActivitiesUnlocked)] is bool value && value;
            set => ViewState[nameof(AreActivitiesUnlocked)] = value;
        }

        public bool AreModulesUnlocked
        {
            get => ViewState[nameof(AreModulesUnlocked)] is bool value && value;
            set => ViewState[nameof(AreModulesUnlocked)] = value;
        }

        protected bool AllowMultipleUnits => _progress.Course.AllowMultipleUnits;

        protected int ModuleListIndex;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(UnitRepeater).FullName;

            SidebarUnitRepeater.DataBinding += SidebarUnitRepeater_DataBinding;
            SidebarUnitRepeater.ItemCreated += SidebarUnitRepeater_ItemCreated;
            SidebarUnitRepeater.ItemDataBound += SidebarUnitRepeater_ItemDataBound;
        }

        public void BindModelToControls(ProgressModel model, bool areModulesUnlocked, bool areActivitiesUnlocked)
        {
            _progress = model;
            AreActivitiesUnlocked = areActivitiesUnlocked;
            AreModulesUnlocked = areModulesUnlocked;

            var units = GetUnits((areActivitiesUnlocked ? _progress.GetAllUnits() : _progress.GetVisibleUnits()));
            SidebarUnitRepeater.DataSource = units;
            SidebarUnitRepeater.DataBind();
        }

        private List<UnitRepeaterItem> GetUnits(List<InSite.Domain.CourseObjects.Unit> units)
        {
            var list = new List<UnitRepeaterItem>();

            foreach (var unit in units)
            {
                var result = _progress.Results[unit.Identifier];
                if (result.IsHidden)
                    continue;

                var item = new UnitRepeaterItem
                {
                    Identifier = unit.Identifier,
                    Code = unit.Code,
                    Name = unit.Content.Title.GetText(CurrentLanguage, true),
                    Type = unit.Type,
                    Asset = unit.Asset,
                    IsAdaptive = (AreModulesUnlocked ? false : unit.IsAdaptive),
                    IsLocked = (AreModulesUnlocked ? false : result.IsLocked),
                    Modules = GetModuleRepeaterItems(unit.Modules)
                };

                if (item.Modules.Count == 0)
                    continue;

                list.Add(item);
            }

            if (list.Count > 0 && !list.Any(x => x.IsActive))
            {
                var defaultActive = list.Where(x => !x.IsLocked)
                    .SelectMany(x => x.Modules).Where(x => !x.IsLocked)
                    .SelectMany(x => x.Activities).Where(x => x.ActivityUrl.IsNotEmpty()).FirstOrDefault();

                if (defaultActive != null)
                    defaultActive.IsActive = true;
            }

            return list;
        }

        private List<ModuleRepeaterItem> GetModuleRepeaterItems(IEnumerable<Module> modules)
        {
            var list = new List<ModuleRepeaterItem>();

            foreach (var module in modules)
            {
                var result = _progress.Results[module.Identifier];
                if (result.IsHidden && !AreModulesUnlocked)
                    continue;

                var item = new ModuleRepeaterItem
                {
                    Identifier = module.Identifier,
                    Code = module.Code,
                    Name = module.Content.Title.GetText(CurrentLanguage, true),
                    Type = module.Type,
                    Asset = module.Asset,
                    IsAdaptive = (AreModulesUnlocked ? false : module.IsAdaptive),
                    IsLocked = (AreModulesUnlocked ? false : result.IsLocked),
                    Activities = GetActivityRepeaterItems(module.GetSupportedActivites())
                };

                if (item.Activities.Count == 0)
                    continue;

                list.Add(item);
            }

            return list;
        }

        private List<ActivityRepeaterItem> GetActivityRepeaterItems(IEnumerable<Activity> activities)
        {
            var list = new List<ActivityRepeaterItem>();

            foreach (var activity in activities)
            {
                var result = _progress.Results[activity.Identifier];
                if (result.IsHidden && !AreModulesUnlocked)
                    continue;

                var url = (AreActivitiesUnlocked ? result.Navigation.Url : 
                    !result.IsLocked ? result.Navigation.Url : string.Empty);

                string activityClass;
                activityClass = SetActivityClass(result);

                list.Add(new ActivityRepeaterItem
                {
                    ActivityIdentifier = activity.Identifier,
                    ActivityClass = activityClass,
                    ActivityUrl = url,
                    ActivityIcon = SetActivityIcon(result.Icon),
                    ActivityName = activity.Content.Title.GetText(CurrentLanguage, true),
                    IsActive = result.Navigation.IsSelected
                });
            }

            return list;
        }

        private string SetActivityIcon(string icon)
            => AreActivitiesUnlocked ? "fas fa-circle text-primary" : icon;

        private string SetActivityClass(LearningResult result)
        {
            string activityClass;
            if (result.IsLocked)
                activityClass = "";
            else if (result.Navigation.IsSelected)
                activityClass = "active";
            else if (result.IsUnlocked)
                activityClass = "";
            else
                activityClass = "disabled";

            if (AreActivitiesUnlocked)
            {
                if (result.Navigation.IsSelected)
                    activityClass = "active";
                else
                    activityClass = "";
            }

            return activityClass;
        }

        private void SidebarUnitRepeater_DataBinding(object sender, EventArgs e)
        {
            ModuleListIndex = 0;
        }

        private void SidebarUnitRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var SidebarModuleRepeater = (Repeater)e.Item.FindControl("ModuleRepeater");
            SidebarModuleRepeater.ItemDataBound += SidebarModuleRepeater_ItemDataBound;
        }

        private void SidebarUnitRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var unit = (UnitRepeaterItem)e.Item.DataItem;
            var repeater = (Repeater)e.Item.FindControl("ModuleRepeater");

            repeater.DataSource = unit.IsLocked ? null : unit.Modules;
            repeater.DataBind();
        }

        private void SidebarModuleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var module = (ModuleRepeaterItem)e.Item.DataItem;
            var repeater = (Repeater)e.Item.FindControl("ActivityRepeater");

            var visibleActivities = (AreActivitiesUnlocked  || AreModulesUnlocked) ? _progress.GetAllActivities() : _progress.GetVisibleActivities();
            var items = module.Activities.Where(a => visibleActivities.Any(v => v.Identifier == a.ActivityIdentifier));

            repeater.ItemDataBound += ActivityRepeater_ItemDataBound;
            repeater.DataSource = module.IsLocked ? null : items;
            repeater.DataBind();
        }

        private void ActivityRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e.Item))
                return;

            var activity = (ActivityRepeaterItem)e.Item.DataItem;

            if (!string.IsNullOrEmpty(activity.ActivityUrl))
            {
                var activityLink = (HtmlAnchor)e.Item.FindControl("ActivityLink");
                activityLink.HRef = activity.ActivityUrl;
            }
        }
    }
}