using System;
using System.Collections.Generic;

namespace InSite.Application.Resources.Read
{
    public class VResource
    {
        public Guid ResourceIdentifier { get; set; }
        
        public int ResourceAsset { get; set; }
        
        public string ResourceTitle { get; set; }
        public string ResourceType { get; set; }
        public string ResourceLabel { get; set; }
        public Guid? ExamFormIdentifier { get; set; }

        public string ParentResourceLabel { get; set; }
        public string ParentResourcePlatform { get; set; }
    }
}
