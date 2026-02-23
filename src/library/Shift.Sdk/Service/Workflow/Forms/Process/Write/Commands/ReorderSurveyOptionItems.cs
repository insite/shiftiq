using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class ReorderSurveyOptionItems : Command
    {
        /// <summary>
        /// Container for the reordered objects.
        /// </summary>
        public Guid List { get; set; }

        /// <summary>
        /// Dictionary Key   = new sequence value
        /// Dictionary Value = object to which the new sequence value is assigned
        /// </summary>
        public Dictionary<Guid, int> Sequences { get; }

        public ReorderSurveyOptionItems(Guid form, Guid list, Dictionary<Guid, int> sequences)
        {
            AggregateIdentifier = form;
            List = list;
            Sequences = sequences;
        }
    }
}