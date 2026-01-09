using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class FormContentChanged : Change
    {
        public Guid Form { get; set; }
        public ContentExamForm Content { get; set; }
        public bool HasDiagrams { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ReferenceMaterialType HasReferenceMaterials { get; set; }

        public FormContentChanged(Guid form, ContentExamForm content, bool hasDiagrams, ReferenceMaterialType hasReferenceMaterials)
        {
            Form = form;
            Content = content;
            HasDiagrams = hasDiagrams;
            HasReferenceMaterials = hasReferenceMaterials;
        }
    }
}
