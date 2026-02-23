using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionLikertOptionsChanged : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public LikertOption[] Options { get; set; }

        public QuestionLikertOptionsChanged(Guid questionIdentifier, IEnumerable<LikertOption> options)
        {
            QuestionIdentifier = questionIdentifier;
            Options = options.Select(x => x.Clone()).ToArray();
        }
    }
}
