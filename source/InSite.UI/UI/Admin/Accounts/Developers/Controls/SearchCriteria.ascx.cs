using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Accounts.Developers.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QPersonSecretFilter>
    {
        public override QPersonSecretFilter Filter
        {
            get
            {
                var filter = new QPersonSecretFilter
                {
                    UserName = DeveloperName.Text,
                    OrganizationCode = OrganizationCode.Text,
                    TokenIssuedSince = TokenIssuedSince.Value,
                    TokenIssuedBefore = TokenIssuedBefore.Value,
                    TokenExpiredSince = TokenExpiredSince.Value,
                    TokenExpiredBefore = TokenExpiredBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DeveloperName.Text = value.UserName;
                OrganizationCode.Text = value.OrganizationCode;
                TokenIssuedSince.Value = value.TokenIssuedSince;
                TokenIssuedBefore.Value = value.TokenIssuedBefore;
                TokenExpiredSince.Value = value.TokenExpiredSince;
                TokenExpiredBefore.Value = value.TokenExpiredBefore;
            }
        }

        public override void Clear()
        {
            DeveloperName.Text = null;
            OrganizationCode.Text = null;
            TokenIssuedSince.Value = null;
            TokenIssuedBefore.Value = null;
            TokenExpiredSince.Value = null;
            TokenExpiredBefore.Value = null;
        }
    }
}