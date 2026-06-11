using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

namespace InSite.Application.Standards.Write
{
    public class AddStandardCategory : Command
    {
        public StandardCategory[] Categories { get; set; }

        public AddStandardCategory(Guid standardId, Guid categoryId, int? sequence)
            : this(standardId, new[] { new StandardCategory(categoryId, sequence) })
        {

        }

        public AddStandardCategory(Guid standardId, StandardCategory[] categories)
        {
            AggregateIdentifier = standardId;
            Categories = categories;
        }
    }
}
