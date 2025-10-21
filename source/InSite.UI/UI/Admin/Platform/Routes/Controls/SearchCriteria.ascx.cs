using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Utilities.Actions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TActionFilter>
    {
        public override TActionFilter Filter
        {
            get
            {
                var filter = new TActionFilter
                {
                    ActionList = ActionList.Text,
                    ActionName = ActionName.Text,
                    ActionUrl = ActionUrl.Text,

                    IncludeActionType = ActionType.Value,
                    ControllerPath = ControllerPath.Text,

                    HasHelpUrl = IsLinkedToHelp.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ActionList.Text = value.ActionList;
                ActionName.Text = value.ActionName;
                ActionUrl.Text = value.ActionUrl;

                ActionType.Value = value.IncludeActionType;
                ControllerPath.Text = value.ControllerPath;

                IsLinkedToHelp.ValueAsBoolean = value.HasHelpUrl;
            }
        }

        public override void Clear()
        {
            ActionList.Text = string.Empty;
            ActionName.Text = string.Empty;
            ActionUrl.Text = string.Empty;

            ActionType.Value = null;
            ControllerPath.Text = null;

            IsLinkedToHelp.ValueAsBoolean = null;
        }
    }
}