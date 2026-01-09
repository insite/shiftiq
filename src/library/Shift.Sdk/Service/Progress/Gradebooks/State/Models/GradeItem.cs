using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Records
{
    [Serializable]
    public class GradeItem
    {
        public Guid Identifier { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Hook { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public GradeItemAchievement Achievement { get; set; }
        public Guid[] Competencies { get; set; }
        public GradeItemFormat Format { get; set; }
        public GradeItemType Type { get; set; }
        public GradeItemWeighting Weighting { get; set; }
        public bool IsReported { get; set; }
        public string Reference { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal? PassPercent { get; set; }

        public Notification[] Notifications { get; set; }

        public GradeItem Parent { get; set; }
        public List<GradeItem> Children { get; set; }
        public CalculationPart[] Parts { get; set; }

        private static readonly Regex ReplaceNonAlpha = new Regex("[^a-zA-Z0-9 ]");
        public string Abbreviation
        {
            get
            {
                var name = ReplaceNonAlpha.Replace(Name ?? "", " ");
                var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var result = new StringBuilder();

                foreach (var part in parts)
                    result.Append(char.ToUpper(part[0]));

                return result.ToString();
            }
        }

        public bool IsEqualWeighting => Weighting == GradeItemWeighting.Equally
                                     || Weighting == GradeItemWeighting.EquallyWithNulls;

        public GradeItem()
        {
            Children = new List<GradeItem>();
            Parts = new CalculationPart[0];
            Notifications = new Notification[0];
        }

        public GradeItem FindItem(Guid key)
        {
            if (Children == null)
                return null;

            foreach (var child in Children)
            {
                if (child.Identifier == key)
                    return child;

                var subItem = child.FindItem(key);

                if (subItem != null)
                    return subItem;
            }

            return null;
        }

        public bool ShouldSerializeChildren() => Children.IsNotEmpty();
        public bool ShouldSerializeFormat() => Type == GradeItemType.Score && Format != GradeItemFormat.None;
        public bool ShouldSerializeNotifications() => Notifications.IsNotEmpty();
        public bool ShouldSerializeParent() => false;
        public bool ShouldSerializeParts() => Type == GradeItemType.Calculation && Parts.IsNotEmpty();
        public bool ShouldSerializeWeighting() => Weighting != GradeItemWeighting.None;
        public bool ShouldSerializeIsEqualWeighting() => false;
    }
}