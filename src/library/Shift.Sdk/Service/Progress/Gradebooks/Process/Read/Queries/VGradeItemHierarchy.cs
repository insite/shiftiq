using System;

namespace InSite.Application.Records.Read
{
    public class VGradeItemHierarchy
    {
        public Guid GradebookIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }
        public Guid? ParentGradeItemIdentifier { get; set; }

        public string GradeItemFormat { get; set; }
        public string GradeItemName { get; set; }
        public string GradeItemType { get; set; }

        public string PathCode { get; set; }
        public string PathIndent { get; set; }
        public string PathSequence { get; set; }

        public int PathDepth { get; set; }
    }
}
