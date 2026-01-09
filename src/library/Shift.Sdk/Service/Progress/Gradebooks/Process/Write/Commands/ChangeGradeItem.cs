using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItem : Command
    {
        public ChangeGradeItem(
            Guid record, 
            Guid item,
            string code,
            string name,
            string shortName,
            bool isReported,
            GradeItemFormat format,
            GradeItemType type,
            GradeItemWeighting weighting,
            Guid? parent
            )
        {
            AggregateIdentifier = record;
            Item = item;
            Code = code;
            Name = name;
            ShortName = shortName;
            IsReported = isReported;

            Format = format;
            Type = type;
            Weighting = weighting;

            Parent = parent;
        }

        public Guid Item { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsReported { get; set; }

        public GradeItemFormat Format { get; set; }
        public GradeItemType Type { get; set; }
        public GradeItemWeighting Weighting { get; set; }

        public Guid? Parent { get; set; }
    }
}
