using System;
using System.Linq;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

namespace InSite.UI.Portal.Records.Credentials.Learners.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<VCredentialFilter>
    {
        // CMDS has always listed orientations and safety certificates here. Any other achievement
        // that issues a certificate is listed too, since a learner can upload one from the Add New
        // Certificate tab.
        private static readonly string[] CmdsAchievementLabels = { "Orientation", "Time-Sensitive Safety Certificate" };

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
                {
                    query.AchievementLabels = CmdsAchievementLabels.ToArray();
                    query.IncludeCertificateIssuingAchievements = true;

                    // The Add New Certificate tab offers achievements another organization shares
                    // with this one, so a certificate uploaded for one must be listed here too.
                    query.IncludeSharedAchievements = true;
                }

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

            if (ServiceLocator.Partition.IsE03())
            {
                // A CMDS learner filters by the same kinds of achievement the results grid lists,
                // available to their organization: its own, plus global ones shared with it.
                // Ownership alone would list every global achievement, including ones never shared
                // with the organization.
                query.OrganizationIdentifiers.Clear();
                query.AvailableToOrganizationIdentifier = Organization.Identifier;
                query.AchievementLabels.AddRange(CmdsAchievementLabels);
                query.IncludeCertificateIssuingAchievements = true;
            }
            else
            {
                query.OrganizationIdentifiers.Add(Organization.Identifier);
            }
        }

        public override void Clear()
        {
            AchievementIdentifier.Values = null;
        }
    }
}
