using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Persistence;

using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Courses.Activities.Controls
{
    public class StandardInfo
    {
        public Guid StandardIdentifier { get; set; }
        public Guid? ParentStandardIdentifier { get; set; }
        public string Title { get; set; }
        public int AssetNumber { get; set; }
        public string Code { get; set; }
        public int Sequence { get; set; }
        public string StandardType { get; set; }

        public int Depth => Parent == null ? 0 : Parent.Depth + 1;

        public StandardInfo Parent { get; set; }
        public List<StandardInfo> Children { get; } = new List<StandardInfo>();

        public static readonly Expression<Func<Standard, StandardInfo>> Binder = LinqExtensions1.Expr((Standard standard) => new StandardInfo
        {
            StandardIdentifier = standard.StandardIdentifier,
            ParentStandardIdentifier = standard.ParentStandardIdentifier,
            Title = CoreFunctions.GetContentText(standard.StandardIdentifier, ContentLabel.Title, CookieTokenModule.Current.Language)
                            ?? CoreFunctions.GetContentTextEn(standard.StandardIdentifier, ContentLabel.Title),
            AssetNumber = standard.AssetNumber,
            Code = standard.Code,
            Sequence = standard.Sequence,
            StandardType = standard.StandardType
        });

        public IEnumerable<StandardInfo> EnumerateChildrenFlatten()
        {
            foreach (var item in Children)
            {
                yield return item;

                foreach (var innerItem in item.EnumerateChildrenFlatten())
                    yield return innerItem;
            }
        }
    }
}