using System;
using System.Collections.Generic;

using Shift.Common;
namespace InSite.Domain.Glossaries
{
    public class GlossaryItem
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public ContentContainer Content { get; set; }
        public List<GlossaryItemContent> Contents { get; } = new List<GlossaryItemContent>();
    }
}
