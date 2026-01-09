using System;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class IssueSettings
    {
        public bool EnableWorkflowManagement { get; set; }
        public Guid[] PortalUploadClaimGroups { get; set; }
        public bool DisplayOnlyConnectedCases { get; set; }
        public bool DefaultCandidateUploadFileView { get; set; }
        public bool DefaultAdministratorUploadFileView { get; set; }

        public IssueSettings()
        {
            PortalUploadClaimGroups = new Guid[0];
            DefaultCandidateUploadFileView = true;
            DefaultAdministratorUploadFileView = false;
        }

        public bool ShouldSerializePortalUploadClaimGroups() => PortalUploadClaimGroups.IsNotEmpty();

        public bool IsEqual(IssueSettings other)
        {
            if (EnableWorkflowManagement != other.EnableWorkflowManagement
                || DisplayOnlyConnectedCases != other.DisplayOnlyConnectedCases
                || DefaultCandidateUploadFileView != other.DefaultCandidateUploadFileView
                || DefaultAdministratorUploadFileView != other.DefaultAdministratorUploadFileView
                )
            {
                return false;
            }

            var groups1 = PortalUploadClaimGroups.EmptyIfNull();
            var groups2 = other.PortalUploadClaimGroups.EmptyIfNull();
            return groups1.Length == groups2.Length
                && groups1.Zip(groups2, (a, b) => a == b).All(x => x);
        }
    }
}
