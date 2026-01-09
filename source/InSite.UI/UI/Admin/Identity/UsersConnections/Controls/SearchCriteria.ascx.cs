using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Admin.Identity.UsersConnections.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QUserConnectionFilter>
    {
        public override QUserConnectionFilter Filter
        {
            get
            {
                var filter = new QUserConnectionFilter
                {
                    FromUserOrganizationId = Organization.Identifier,
                    IsManager = IsManager.Checked,
                    IsSupervisor = IsSupervisor.Checked,
                    IsValidator = IsValidator.Checked,
                    FromUserName = FromUserName.Text,
                    ToUserName = ToUserName.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                IsManager.Checked = value.IsManager;
                IsSupervisor.Checked = value.IsSupervisor;
                IsValidator.Checked = value.IsValidator;
                FromUserName.Text = value.FromUserName;
                ToUserName.Text = value.ToUserName;
            }
        }

        public override void Clear()
        {
            IsManager.Checked = false;
            IsSupervisor.Checked = false;
            IsValidator.Checked = false;
            FromUserName.Text = null;
            ToUserName.Text = null;
        }
    }
}