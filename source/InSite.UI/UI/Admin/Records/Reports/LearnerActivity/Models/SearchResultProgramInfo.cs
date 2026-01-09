using System;

using InSite.Persistence;

namespace InSite.Admin.Records.Reports.LearnerActivity.Models
{
    [Serializable]
    internal class SearchResultProgramInfo
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }

        public SearchResultProgramInfo(VProgramEnrollment entity)
        {
            Identifier = entity.ProgramIdentifier;
            Name = entity.ProgramName;
        }
    }
}