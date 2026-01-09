using System;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Records.Credentials.Learners.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VCredentialFilter>
    {
        public override VCredentialFilter Filter
        {
            get
            {
                var query = new VCredentialFilter
                {
                    UserIdentifier = User.UserIdentifier,
                    IsGranted = true,
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    AchievementIdentifiers = AchievementIdentifier.Values.ToArray()
                };

                if (ServiceLocator.Partition.IsE03())
                    query.AchievementLabels = new[] { "Orientation", "Time-Sensitive Safety Certificate" };

                GetCheckedShowColumns(query);

                return query;
            }
            set
            {
                AchievementIdentifier.Values = value.AchievementIdentifiers;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var query = AchievementIdentifier.Filter;

            query.OrganizationIdentifiers.Add(Organization.Identifier);

            if (ServiceLocator.Partition.IsE03())
            {
                query.AchievementLabels.Add("Orientation");
                query.AchievementLabels.Add("Time-Sensitive Safety Certificate");
                AchievementIdentifier.Filter.AchievementLabels = query.AchievementLabels;
            }
        }

        public override void Clear()
        {
            AchievementIdentifier.Values = null;
        }
    }
}