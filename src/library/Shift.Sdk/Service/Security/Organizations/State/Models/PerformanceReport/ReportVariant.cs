using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations.PerformanceReport
{
    [Serializable]
    public class ReportVariant
    {
        public string Name { get; set; }
        public string FileSuffix { get; set; }
        public string Language { get; set; }
        public decimal EmergentScore { get; set; }
        public decimal ConsistentScore { get; set; }
        public string RequiredRole { get; set; }
        public string NursingRoleText { get; set; }
        public string Description { get; set; }
        public ItemWeight[] RoleWeights { get; set; }
        public ItemWeight[] AssessmentTypeWeights { get; set; }

        public ReportVariant()
        {
            RoleWeights = new ItemWeight[0];
            AssessmentTypeWeights = new ItemWeight[0];
        }

        public bool ShouldSerializeRoleWeights() => RoleWeights.IsNotEmpty();

        public bool IsEqual(ReportVariant other)
        {
            return IsShallowEqual(other)
                && ItemWeight.IsEqual(RoleWeights, other.RoleWeights)
                && ItemWeight.IsEqual(AssessmentTypeWeights, other.AssessmentTypeWeights);
        }

        public static bool IsEqual(ICollection<ReportVariant> collection1, ICollection<ReportVariant> collection2)
        {
            return collection1.Count == collection2.Count
                && collection1.Zip(collection2, (a, b) => a.IsEqual(b)).All(x => x);
        }

        public bool IsShallowEqual(ReportVariant other)
        {
            return Name == other.Name
                && FileSuffix == other.FileSuffix
                && Language == other.Language
                && EmergentScore == other.EmergentScore
                && ConsistentScore == other.ConsistentScore
                && RequiredRole == other.RequiredRole
                && NursingRoleText == other.NursingRoleText
                && Description == other.Description;
        }

        public ReportVariant Clone()
        {
            return new ReportVariant
            {
                Name = Name.Trim().NullIfEmpty(),
                FileSuffix = FileSuffix.NullIfWhiteSpace(),
                Language = Language.NullIfWhiteSpace(),
                EmergentScore = EmergentScore,
                ConsistentScore = ConsistentScore,
                RequiredRole = RequiredRole.NullIfWhiteSpace(),
                NursingRoleText = NursingRoleText,
                Description = Description,
                RoleWeights = RoleWeights.Select(x => x.Clone()).ToArray(),
                AssessmentTypeWeights = AssessmentTypeWeights.Select(x => x.Clone()).ToArray(),
            };
        }
    }
}
