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
                    ActionList = ActionList.Value,
                    ActionName = ActionName.Text,
                    ActionType = ActionType.Value,
                    ActionUrl = ActionUrl.Text,
                    AuthorityType = AuthorityType.Value,
                    AuthorizationRequirement = AuthorizationRequirement.Value,
                    ControllerPath = ControllerPath.Text,
                    ExtraBreadcrumb = ExtraBreadcrumb.Value,

                    HasHelpUrl = IsLinkedToHelp.ValueAsBoolean
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ActionList.Value = value.ActionList;
                ActionName.Text = value.ActionName;
                ActionType.Value = value.ActionType;
                ActionUrl.Text = value.ActionUrl;
                AuthorityType.Value = value.AuthorityType;
                AuthorizationRequirement.Value = value.AuthorizationRequirement;
                ControllerPath.Text = value.ControllerPath;
                ExtraBreadcrumb.Value = value.ExtraBreadcrumb;

                IsLinkedToHelp.ValueAsBoolean = value.HasHelpUrl;
            }
        }

        public override void Clear()
        {
            ActionName.Text = string.Empty;
            ActionUrl.Text = string.Empty;

            ActionList.Value = null;
            ActionList.Value = null;
            ActionType.Value = null;
            AuthorityType.Value = null;
            AuthorizationRequirement.Value = null;
            ExtraBreadcrumb.Value = null;
            ControllerPath.Text = null;

            IsLinkedToHelp.ValueAsBoolean = null;
        }
    }
}