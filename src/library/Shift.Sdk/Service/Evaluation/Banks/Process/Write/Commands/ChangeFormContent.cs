using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ChangeFormContent : Command
    {
        public Guid Form { get; set; }
        public ContentExamForm Content { get; set; }
        public bool HasDiagrams { get; set; }
        public ReferenceMaterialType HasReferenceMaterials { get; set; }

        public ChangeFormContent(Guid bank, Guid form, ContentExamForm content, bool hasDiagrams, ReferenceMaterialType hasReferenceMaterials)
        {
            AggregateIdentifier = bank;
            Form = form;
            Content = content;
            HasDiagrams = hasDiagrams;
            HasReferenceMaterials = hasReferenceMaterials;
        }
    }
}
